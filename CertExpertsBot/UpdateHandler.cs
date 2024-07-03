using CertExpertsBot.UpdateTypeHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
                            var keyBtns = new List<KeyboardButton>();
                            for (int i = 0; i <= 9; i++)
                                keyBtns.Add(new KeyboardButton(i.ToString()));
                            var replyKeyBoard = new ReplyKeyboardMarkup(keyBtns);
                            replyKeyBoard.ResizeKeyboard = true;

                            await botClient.SendTextMessageAsync(message.Chat, response, replyMarkup: replyKeyBoard);
                        }
                    }
                    return;
                default:
                    Console.WriteLine("New Type was found. " + update.Type.ToString());
                    return;
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
