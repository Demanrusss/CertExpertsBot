using CertExpertsBot.UpdateTypeHandlers.Managers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CertExpertsBot.UpdateTypeHandlers
{
    public static class CallbackQueryHandler
    {
        public static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery? callbackQuery,
            CancellationToken cancellationToken)
        {
            if (callbackQuery == null || callbackQuery.Message == null || callbackQuery.Message.Text == null)
                return;

            switch (callbackQuery.Data)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    var text = callbackQuery.Message.Text + callbackQuery.Data;
                    callbackQuery.Message.Text = text;
                    var replyMarkup = ReplyMarkupManager.ReplyMarkup_Numbers(text);
                    await botClient.EditMessageTextAsync(callbackQuery.Message.Chat,
                                                         callbackQuery.Message.MessageId,
                                                         text,
                                                         replyMarkup: replyMarkup);
                    if (text.Length == 11)
                        await MessageHandler.HandleMessageAsync(botClient, callbackQuery.Message, cancellationToken);
                    return;
                case "removeNumber":
                    text = callbackQuery.Message.Text;
                    if (text.Length == 1)
                        return;
                    
                    callbackQuery.Message.Text = text.Substring(0, text.Length - 1);
                    replyMarkup = ReplyMarkupManager.ReplyMarkup_Numbers(callbackQuery.Message.Text);
                    await botClient.EditMessageTextAsync(callbackQuery.Message.Chat,
                                                         callbackQuery.Message.MessageId,
                                                         callbackQuery.Message.Text,
                                                         replyMarkup: replyMarkup);
                    return;
                default:
                    Console.WriteLine($"{callbackQuery.Data} not implemented");
                    return;
            }
        }
    }
}
