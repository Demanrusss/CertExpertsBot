using CertExpertsBot.Models;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CertExpertsBot
{
    internal static class Program
    {
        private static readonly string ExecutableLocation = 
            Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!;
        private static readonly string AppSettingsPath = Path.Combine(ExecutableLocation, "appsettings.json");
        private static readonly AppSettings Settings = JsonConvert
            .DeserializeObject<AppSettings>(System.IO.File.ReadAllText(AppSettingsPath))!;

        private static async Task Main()
        {
            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                cts.Cancel();
            };

            await using var memConnection = new SqliteConnection(Settings.ConnectionStrings["MemoryConnection"]);
            memConnection.Open();
            UploadDbToMemory(memConnection);

            await StartBotAsync(cts.Token);

            memConnection.Close();
        }

        private static void UploadDbToMemory(SqliteConnection connection)
        {
            var dataSource = Settings.ConnectionStrings["DefaultConnection"].Split(';')[0];
            var dbName = dataSource.Split('=')[1];

            var command = connection.CreateCommand();
            command.CommandText = String.Format("ATTACH '{0}' AS disk;", dbName);
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE TechRegs
                                    AS SELECT * FROM disk.TechRegs;
                                        
                                    CREATE TABLE TNVEDCodes 
                                    AS SELECT * FROM disk.TNVEDCodes;
                                        
                                    CREATE TABLE TNVEDCodeTechReg 
                                    AS SELECT * FROM disk.TNVEDCodeTechReg";
            command.ExecuteNonQuery();
        }

        private static async Task StartBotAsync(CancellationToken cancellationToken)
        {
            ITelegramBotClient bot = new TelegramBotClient(Settings!.ConnectionStrings["CertExpertsBotToken"]);

            var receiverOptions = CreateReceiverOptions();

            bot.StartReceiving(
                HandleUpdateAsync,
                HandlePollingErrorAsync,
                receiverOptions,
                cancellationToken
            );

            Console.WriteLine("Bot started");

            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Bot stopping...");
            }
        }

        private static ReceiverOptions CreateReceiverOptions()
        {
            var receiverOptions = new ReceiverOptions();

            var allowedUpdates = new[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery
            };

            receiverOptions.AllowedUpdates = allowedUpdates;
            receiverOptions.ThrowPendingUpdates = true;

            return receiverOptions;
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, 
            CancellationToken cancellationToken) => await UpdateHandler.HandleUpdateAsync(botClient, update, cancellationToken);

        private static async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, 
            CancellationToken cancellationToken) => await UpdateHandler.HandlePollingErrorAsync(exception);
    }
}