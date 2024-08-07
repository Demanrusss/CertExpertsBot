using CertExpertsBot.Models;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CertExpertsBot
{
    class Program
    {
        private static readonly string executableLocation = 
            Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!;
        private static readonly string appSettingsPath = Path.Combine(executableLocation, "appsettings.json");
        private static readonly AppSettings settings = JsonConvert
            .DeserializeObject<AppSettings>(System.IO.File.ReadAllText(appSettingsPath))!;
        private static ITelegramBotClient bot = new TelegramBotClient(settings!.ConnectionStrings["CertExpertsBotToken"]);

        static void Main(string[] args)
        {
            using (var memConnection = new SqliteConnection(settings!.ConnectionStrings["MemoryConnection"]))
            {
                memConnection.Open();
                UploadDbToMemory(memConnection);

                StartBot();

                memConnection.Close();
            }
        }

        private static void UploadDbToMemory(SqliteConnection connection)
        {
            string dataSource = settings!.ConnectionStrings["DefaultConnection"].Split(';')[0];
            string dbName = dataSource.Split('=')[1];

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

        private static void StartBot()
        {
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = CreateReceiverOptions();

            bot.StartReceiving(
                HandleUpdateAsync,
                HandlePollingErrorAsync,
                receiverOptions,
                cancellationToken
            );

            Console.CancelKeyPress += (_, _) => cts.Cancel();

            Console.WriteLine("Bot started. Press ^C to stop");
            Console.ReadLine();
        }

        private static ReceiverOptions CreateReceiverOptions()
        {
            var receiverOptions = new ReceiverOptions();

            var allowedUpdates = new UpdateType[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery
            };

            receiverOptions.AllowedUpdates = allowedUpdates;
            receiverOptions.ThrowPendingUpdates = true;

            return receiverOptions;
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, 
            Update update, CancellationToken cancellationToken)
        {
            await UpdateHandler.HandleUpdateAsync(botClient, update, cancellationToken);
        }

        private static async Task HandlePollingErrorAsync(ITelegramBotClient botClient,
            Exception exception, CancellationToken cancellationToken)
        {
            await UpdateHandler.HandlePollingErrorAsync(botClient, exception, cancellationToken);
        }
    }
}