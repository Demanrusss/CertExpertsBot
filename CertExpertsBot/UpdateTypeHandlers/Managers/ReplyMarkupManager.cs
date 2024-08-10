using CertExpertsBot.Data;
using Telegram.Bot.Types.ReplyMarkups;

namespace CertExpertsBot.UpdateTypeHandlers.Managers
{
    public static class ReplyMarkupManager
    {
        private static readonly AppDbContext dbContext = new AppDbContext();

        public static InlineKeyboardMarkup ReplyMarkupKB_AllNumbers()
        {
            var allNumbers = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            return ReplyMarkupKB_AvailableNumbers(allNumbers);
        }

        public static InlineKeyboardMarkup ReplyMarkup_Numbers(string text)
        {
            if (text.Length == 1)
                return ReplyMarkupKB_AllNumbers();

            var availableNumbers = AvailableNumbers(text.Substring(1));
            return ReplyMarkupKB_AvailableNumbers(availableNumbers);
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

            keyBoard.Add(RemoveBtnRow());

            return new InlineKeyboardMarkup(keyBoard);
        }

        private static List<InlineKeyboardButton> RemoveBtnRow()
        {
            return new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("⬅️", "removeNumber")
            };
        }
    }
}
