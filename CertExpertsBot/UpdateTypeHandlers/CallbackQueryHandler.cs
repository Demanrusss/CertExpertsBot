using CertExpertsBot.Data;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CertExpertsBot.UpdateTypeHandlers
{
    public static class CallbackQueryHandler
    {
        private static readonly AppDbContext dbContext = new AppDbContext();

        public static InlineKeyboardMarkup ReplyMarkup_Numbers(CallbackQuery callbackQuery)
        {
            if (callbackQuery.Message == null || callbackQuery.Message.Text == null)
                return ReplyMarkupKB_AllNumbers();

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

            bool newPartCodeExists;
            for (int i = 0; i <= 9; i++)
            {
                newPartCodeExists = dbContext.TNVEDCodes.Any(c => c.Code.StartsWith(partCode + i.ToString()));
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
                if (i <= 4)
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
