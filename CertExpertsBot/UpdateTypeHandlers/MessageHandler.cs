using CertExpertsBot.Data;
using CertExpertsBot.UpdateTypeHandlers.Managers;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CertExpertsBot.UpdateTypeHandlers
{
    public static class MessageHandler
    {
        private static readonly AppDbContext dbContext = new AppDbContext();

        public static async Task HandleMessageAsync(ITelegramBotClient botClient, Message? message,
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
                var replyMarkupDefault = ReplyMarkupManager.ReplyMarkupKB_AllNumbers();
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

        private static string Response(Message message)
        {
            if (message == null || String.IsNullOrWhiteSpace(message.Text))
                return "Странно, но пришло пустое сообщение";

            string text = message.Text.Trim();

            if (message.Text.StartsWith('/'))
                return ResponseOnCommand(message);

            if (message.Text.StartsWith('.'))
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
                case "/codes_quantity":
                    return ResponseOnCommand_CodesQuantity();
                case "/techdocs_list_p1":
                    return ResponseOnCommand_TechDocsList(1);
                case "/techdocs_list_p2":
                    return ResponseOnCommand_TechDocsList(2);
                case "/techdocs_list_p3":
                    return ResponseOnCommand_TechDocsList(3);
                case "/techdocs_list_p4":
                    return ResponseOnCommand_TechDocsList(4);
                case "/techdocs_list_p5":
                    return ResponseOnCommand_TechDocsList(5);
                case "/techdocs_list_p6":
                    return ResponseOnCommand_TechDocsList(6);
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
            sb.AppendLine("/codes_quantity - количество кодов ТН ВЭД");
            sb.AppendLine("/techdocs_list_p1 - список тех.док-ов (стр.1)");
            sb.AppendLine("/techdocs_list_p2 - список тех.док-ов (стр.2)");
            sb.AppendLine("/techdocs_list_p3 - список тех.док-ов (стр.3)");
            sb.AppendLine("/techdocs_list_p4 - список тех.док-ов (стр.4)");
            sb.AppendLine("/techdocs_list_p5 - список тех.док-ов (стр.5)");
            sb.AppendLine("/techdocs_list_p6 - список тех.док-ов (стр.6)");

            return sb.ToString();
        }

        private static string ResponseOnCommand_TNVED()
        {
            return "Обработчик кодов ТНВЭД запущен.\n" + 
                "Можете вводить 10-ти значный код ТН ВЭД через точку (например, .0123456789)\n" + 
                "либо использовать кнопки ниже";
        }

        private static string ResponseOnCommand_CodesQuantity()
        {
            var quantity = dbContext.TNVEDCodes.Count();
            return $"Всего кодов ТН ВЭД в базе: {quantity}";
        }

        private static string ResponseOnCommand_TechDocsList(int page)
        {
            int skipResultsQuantity = (page - 1) * 10;
            var techRegs = dbContext.TechRegs
                .OrderBy(tr => tr.Name)
                .Select(tr => tr.Name)
                .Skip(skipResultsQuantity)
                .Take(10);
            return $"Список тех. документов (стр. {page}):\n{String.Join("\n\n", techRegs)}";
        }

        private static string ResponseOnOtherText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Пока еще не знаю, что ответить.\n");
            sb.AppendLine("Вы можете связаться с компанией \"Консалтинг+\"");
            sb.AppendLine("Email: deman.russs@mail.ru");
            sb.AppendLine("WA: +7-708-434-30-60");

            return sb.ToString();
        }

        private static string ResponseOnTNVED(string code)
        {
            if (code == null || code.Length != 10)
                return "Нужно указать полный код ТН ВЭД (10 цифр)";

            var tnved = dbContext.TNVEDCodes
                .Include(c => c.TechRegs)
                .FirstOrDefault(c => c.Code == code);

            return tnved != null ? tnved.ToString() : "Такой код ТН ВЭД не найден";
        }
    }
}
