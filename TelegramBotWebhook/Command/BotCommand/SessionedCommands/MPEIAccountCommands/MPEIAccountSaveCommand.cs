using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class MPEIAccountSaveCommand : SessionedBotCommand
    {
        public MPEIAccountSaveCommand() : base(";", "mpeiaccountsave")
        {
        }

        protected override void AddNewService(object service)
        {
        }

        protected override Task<ExecuteResult> ConcreteExecute(string option)
        {
            string text, buttonText;
            if (Session.SaveCredentials)
            {
                Session.SaveCredentials = false;
                text = "По истечении сессии данные аккаунта не сохраняются.";
                buttonText = "Сохранять данные";
            } 
            else
            {
                Session.SaveCredentials = true;
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
