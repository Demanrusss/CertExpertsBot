using CertExpertsBot.Data;
using CertExpertsBot.UpdateTypeHandlers;
using Microsoft.EntityFrameworkCore;
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
                    var response = MessageHandler.Response(message);
                    if (response.Contains("Обработчик кодов ТНВЭД"))
                    {
                        await botClient.SendTextMessageAsync(chatId: message.Chat,
                                                             text: response,
                                                             cancellationToken: cancellationToken);
                        var replyMarkupDefault = CallbackQueryAnswer.ReplyMarkupKB_AllNumbers();
                        await botClient.SendTextMessageAsync(chatId: message.Chat,
                                                             text: ".",
                                                             replyMarkup: replyMarkupDefault,
                                                             cancellationToken: cancellationToken);
                    }
                    else
                        await botClient.SendTextMessageAsync(chatId: message.Chat,
                                                             text: response,
                                                             cancellationToken: cancellationToken);
                    break;
                case UpdateType.CallbackQuery:
                    var callbackQuery = update.CallbackQuery;
                    var replyMarkup = CallbackQueryAnswer.ReplyMarkup(callbackQuery);
                    var text = callbackQuery.Message.Text + callbackQuery.Data;
                    if (text.Length == 11)
                    {
                        response = MessageHandler.Response(new Message() { Text = text });
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat,
                                                             text: response,
                                                             cancellationToken: cancellationToken);
                    }
                    else
                        await botClient.EditMessageTextAsync(callbackQuery.Message.Chat,
                                                             callbackQuery.Message.MessageId,
                                                             text, 
                                                             replyMarkup: replyMarkup);
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

    public static class CallbackQueryAnswer
    {
        private static readonly AppDbContext dbContext = new AppDbContext();

        public static InlineKeyboardMarkup ReplyMarkup(CallbackQuery callbackQuery)
        {
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
                    var partCode = callbackQuery.Message.Text.Substring(1) + callbackQuery.Data;
                    var availableNumbers = AvailableNumbers(partCode);
                    return ReplyMarkupKB_AvailableNumbers(availableNumbers);
                default:
                    return ReplyMarkupKB_AllNumbers();
            }
        }

        private static IList<string> AvailableNumbers(string partCode)
        {
            var availableNumbers = new List<string>();

            for (int i = 0; i <= 9; i++)
            {
                bool newPartCodeExists = dbContext.TNVEDCodes.Any(c => c.Code.StartsWith(partCode + i.ToString()));
                if (newPartCodeExists)
                    availableNumbers.Add(i.ToString());
            }

            return availableNumbers;
        }

        private static InlineKeyboardMarkup ReplyMarkupKB_AvailableNumbers(IList<string> availableNumbers)
        {
            var keyBoard = new List<List<InlineKeyboardButton>>();

            var row1 = new List<InlineKeyboardButton>();
            var row2 = new List<InlineKeyboardButton>();
            for (int i = 0; i < availableNumbers.Count; i++)
            {
                if (i <= 7)
                    row1.Add(InlineKeyboardButton.WithCallbackData(availableNumbers[i], availableNumbers[i]));
                else
                    row2.Add(InlineKeyboardButton.WithCallbackData(availableNumbers[i], availableNumbers[i]));
            }

            keyBoard.Add(row1);
            if (row2.Count > 0)
                keyBoard.Add(row2);

            return new InlineKeyboardMarkup(keyBoard);
        }

        public static InlineKeyboardMarkup ReplyMarkupKB_AllNumbers()
        {
            var allNumbers = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            return ReplyMarkupKB_AvailableNumbers(allNumbers);
        }
    }
}
