using CertExpertsBot.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CertExpertsBot
{
    class Program
    {
        private static readonly AppSettings settings = JsonConvert
            .DeserializeObject<AppSettings>(System.IO.File.ReadAllText("appsettings.json"))!;
        private static ITelegramBotClient bot = new TelegramBotClient(settings!.ConnectionStrings["CertExpertsBotToken"]);

        static void Main(string[] args)
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
                UpdateType.Message
            };

            receiverOptions.AllowedUpdates = allowedUpdates;
            receiverOptions.ThrowPendingUpdates = true;

            return receiverOptions;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, 
            Update update, CancellationToken cancellationToken)
        {
            await UpdateHandler.HandleUpdateAsync(botClient, update, cancellationToken);
        }

        public static async Task HandlePollingErrorAsync(ITelegramBotClient botClient, 
            Exception exception, CancellationToken cancellationToken)
        {
            await UpdateHandler.HandlePollingErrorAsync(botClient, exception, cancellationToken);
        }
    }
}