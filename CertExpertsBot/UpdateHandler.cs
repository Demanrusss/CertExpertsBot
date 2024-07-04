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
            if (botClient == null || update == null || update.Message == null)
                return;

            var message = update.Message;

            switch (update.Type)
            {
                case UpdateType.Message:
                    var response = MessageHandler.Response(message);                        
                    await botClient.SendTextMessageAsync(chatId: message.Chat, 
                                                         text: response, 
                                                         cancellationToken: cancellationToken);
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
