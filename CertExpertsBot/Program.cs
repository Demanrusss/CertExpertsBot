using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace CertExpertsBot
{
    class Program
    {
        private static void Main(string[] args)
        {
            var bot = new TelegramBotClient("");

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var handler = new UpdateHandler();
            var receiverOptions = CreateReceiverOptions();

            bot.StartReceiving(
                handler.HandleUpdateAsync,
                handler.HandlePollingErrorAsync,
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

            return receiverOptions;
        }
    }
}