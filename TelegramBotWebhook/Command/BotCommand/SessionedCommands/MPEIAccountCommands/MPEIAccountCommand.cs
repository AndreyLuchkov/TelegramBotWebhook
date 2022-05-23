using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotWebhook.Services;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class MPEIAccountCommand : SessionedBotCommand
    {
        private IEmailAutentificationService? _autentificationService;

        public MPEIAccountCommand() : base(";", "mpeiaccount")
        {
            AddRequiredServiceType(typeof(IEmailAutentificationService));
        }

        protected override void AddNewService(object service)
        {
            if (service is IEmailAutentificationService autentificationService)
            {
                _autentificationService = autentificationService;
            }
        }
        protected override Task<ExecuteResult> ConcreteExecute(string option)
        {
            string text, buttonText;
            if (!Session.SaveCredentials)
            {
                text = "По истечении сессии данные аккаунта не сохраняются.";
                buttonText = "Сохранять данные";
            }
            else
            {
                text = $"По истечении сессии данные аккаунта сохранятся.\n<b>Сохраненные данные</b>\nЛогин: {Session.Login}";
                buttonText = "Не сохранять данные";
            }

            var buttonRows = new List<IEnumerable<InlineKeyboardButton>>
            {
                new[] { InlineKeyboardButton.WithCallbackData(buttonText, ";mpeiaccountsave") },
                new[] { InlineKeyboardButton.WithCallbackData("<< Назад", "/settings") },
            };
            if (UsedLoginCommand())
            {
                var buttonRow = new[] { InlineKeyboardButton.WithCallbackData("Выйти из текущего аккаунта", ";mpeiaccountunlog") };

                buttonRows.Insert(1, buttonRow);
            }

            return Task.FromResult(new ExecuteResult(ResultType.EditMessageWithInlineKeyboard, text)
            {
                InlineKeyboardMarkup = new InlineKeyboardMarkup(buttonRows)
            }); 
        }
        private bool UsedLoginCommand() => Session.UserKey is not null && Session.UnlogKey is not null;
    }
}
