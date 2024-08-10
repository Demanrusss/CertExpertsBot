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
                    await HandleMessageAsync(botClient, update.Message, cancellationToken);
                    break;
                case UpdateType.CallbackQuery:
                    await HandleCallbackQueryAsync(botClient, update.CallbackQuery, cancellationToken);
                    break;
                default:
                    Console.WriteLine("New Type was found. " + update.Type.ToString());
                    break;
            }
        }

        private static async Task HandleMessageAsync(ITelegramBotClient botClient, Message? message, 
            CancellationToken cancellationToken)
        {
            if (message == null)
                return;
            string response = MessageHandler.Response(message);
            if (response.Contains("Обработчик кодов ТНВЭД"))
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat,
                text: response,
                                                     cancellationToken: cancellationToken);
                var replyMarkupDefault = CallbackQueryHandler.ReplyMarkupKB_AllNumbers();
                await botClient.SendTextMessageAsync(chatId: message.Chat,
                                                     text: ".",
                replyMarkup: replyMarkupDefault,
                                                     cancellationToken: cancellationToken);
            }
            else
                await botClient.SendTextMessageAsync(chatId: message.Chat,
                                                     text: response,
                                                     cancellationToken: cancellationToken);
        }

        private static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery? callbackQuery,
            CancellationToken cancellationToken)
        {
            if (callbackQuery == null || callbackQuery.Message == null)
                return;
            var replyMarkup = CallbackQueryHandler.ReplyMarkup_Numbers(callbackQuery);
            var text = callbackQuery.Message.Text + callbackQuery.Data;
            if (text.Length == 11)
            {
                var response = MessageHandler.Response(new Message() { Text = text });
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat,
                text: response,
                                                     cancellationToken: cancellationToken);
            }
            else
                await botClient.EditMessageTextAsync(callbackQuery.Message.Chat,
                                                     callbackQuery.Message.MessageId,
                                                     text,
                                                     replyMarkup: replyMarkup);
        }

        public static async Task HandlePollingErrorAsync(ITelegramBotClient botClient, 
            Exception exception, CancellationToken cancellationToken)
        {
            Console.Error.WriteLine(exception);
            await Task.CompletedTask;
        }
    }
}
