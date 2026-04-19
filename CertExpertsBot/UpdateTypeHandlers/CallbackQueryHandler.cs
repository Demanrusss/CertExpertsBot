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
            if (callbackQuery?.Message?.Text == null)
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
                    await AddDigitHandler(botClient, callbackQuery, cancellationToken);
                    return;
                case "removeNumber":
                    await RemoveDigitHandler(botClient, callbackQuery, cancellationToken);
                    return;
                default:
                    return;
            }
        }
        
        private static async Task AddDigitHandler(ITelegramBotClient botClient, CallbackQuery callbackQuery,
            CancellationToken cancellationToken)
        {
            var text = callbackQuery.Message!.Text + callbackQuery.Data;
            callbackQuery.Message.Text = text;
            var replyMarkup = ReplyMarkupManager.ReplyMarkup_Numbers(text);
            await botClient.EditMessageTextAsync(callbackQuery.Message.Chat,
                callbackQuery.Message.MessageId,
                text,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
            
            if (text.Length == 11)
                await MessageHandler.HandleMessageAsync(botClient, callbackQuery.Message, cancellationToken);
        }
        
        private static async Task RemoveDigitHandler(ITelegramBotClient botClient, CallbackQuery callbackQuery,
            CancellationToken cancellationToken)
        {
            var text = callbackQuery.Message!.Text!;
            if (text.Length == 1)
                return;
                    
            callbackQuery.Message.Text = text[..^1];
            var replyMarkup = ReplyMarkupManager.ReplyMarkup_Numbers(callbackQuery.Message.Text);
            await botClient.EditMessageTextAsync(callbackQuery.Message.Chat,
                callbackQuery.Message.MessageId,
                callbackQuery.Message.Text,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
        }
    }
}
