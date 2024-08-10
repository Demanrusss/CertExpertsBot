using CertExpertsBot.UpdateTypeHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CertExpertsBot
{
    public static class UpdateHandler
    {
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, 
            Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await MessageHandler.HandleMessageAsync(botClient, update.Message, cancellationToken);
                    break;
                case UpdateType.CallbackQuery:
                    await CallbackQueryHandler.HandleCallbackQueryAsync(botClient, update.CallbackQuery, cancellationToken);
                    break;
                default:
                    Console.WriteLine("New Type was found. " + update.Type.ToString());
                    break;
            }
        }

        public static async Task HandlePollingErrorAsync(ITelegramBotClient botClient, 
            Exception exception, CancellationToken cancellationToken)
        {
            Console.Error.WriteLine(exception);
            await Task.CompletedTask;
        }
    }
}
