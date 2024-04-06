using CertExpertsBot.Data;
using System.Text;
using Telegram.Bot.Types;

namespace CertExpertsBot.UpdateTypeHandlers
{
    public static class MessageHandler
    {
        private static readonly DbContext dbContext = new DbContext();

        public static string Response(Message message)
        {
            if (message == null || String.IsNullOrWhiteSpace(message.Text))
                return "Странно, но пришло пустое сообщение";

            string text = message.Text.Trim();

            if (message.Text.StartsWith('/'))
                return ResponseOnCommand(message);

            if (message.Text.StartsWith('#'))
            {
                string code = message.Text.Length > 1 ? message.Text.Substring(1) : String.Empty;
                return ResponseOnTNVED(code);
            }
            else
                return ResponseOnOtherText();
        }

        private static string ResponseOnCommand(Message message)
        {
            switch (message.Text)
            {
                case "/help":
                    return ResponseOnCommand_Help();

                case "/start":
                    return ResponseOnCommand_Start(message);

                case "/tnved":
                    return ResponseOnCommand_TNVED();
                
                default:
                    return "Такой команды нет";
            }
        }

        private static string ResponseOnCommand_Start(Message message)
        {
            string userFirstName = message.From?.FirstName ?? "пользователь";
            return $"Приветствую, {userFirstName}!\nПомощь по командам - /help";
        }

        private static string ResponseOnCommand_Help()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Доступные команды:");
            sb.AppendLine("/help - помощь по командам");
            sb.AppendLine("/start - начать работу с ботом");
            sb.AppendLine("/tnved - справочник кодов ТНВЭД");

            return sb.ToString();
        }

        private static string ResponseOnCommand_TNVED()
        {
            return "Запустил обработчик кодов ТНВЭД\nМожете вводить код ТН ВЭД (#xxxxxxxxxx)";
        }

        private static string ResponseOnOtherText()
        {
            return "Пока еще не знаю, что ответить";
        }

        private static string ResponseOnTNVED(string code)
        {
            if (String.IsNullOrWhiteSpace(code) || code.Length != 10)
                return "Нужно указать полный код ТН ВЭД (10 цифр)";

            var tnved = dbContext.TNVEDCodes.FirstOrDefault(c => c.Code == code);

            return tnved != null ? tnved.ToString() : "Такой код ТН ВЭД не найден";
        }
    }
}
