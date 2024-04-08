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
            if (botClient == null || update == null)
                return;

            switch (update.Type)
            {
                case UpdateType.Message:
                    var message = update.Message;
                    if (message != null)
                    {
                        var response = MessageHandler.Response(message);
                        if (!String.IsNullOrWhiteSpace(response))
                        {
                            if (response.Contains("Запустил обработчик кодов ТНВЭД"))
                                ;//TODO: Что-то хотел с этим сделать;
                              
                            await botClient.SendTextMessageAsync(message.Chat, response);
                        }    
                            
                    }
                    return;
                default:
                    Console.WriteLine("New Type was found. " + update.Type.ToString());
                    return;
            }
        }

        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, 
            Exception exception, CancellationToken cancellationToken)
        {
            Console.Error.WriteLine(exception);
            return Task.CompletedTask;
        }
    }
}
