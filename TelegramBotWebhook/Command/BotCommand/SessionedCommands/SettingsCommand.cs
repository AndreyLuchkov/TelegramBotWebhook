using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class SettingsCommand : SessionedBotCommand
    {
        public SettingsCommand() : base("/", "settings")
        {
        }

        protected override void AddNewService(object service)
        {
        }
        protected override Task<ExecuteResult> ConcreteExecute(string option)
        {
            return Task.FromResult(new ExecuteResult(ResultType.InlineKeyboardWithCallback, "Выберите, что хотите настроить.")
            {
                InlineKeyboardMarkup = (new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("Аккаунт почты МЭИ", ";mpeiaccount") },
                })
            });
        }
    }
}
