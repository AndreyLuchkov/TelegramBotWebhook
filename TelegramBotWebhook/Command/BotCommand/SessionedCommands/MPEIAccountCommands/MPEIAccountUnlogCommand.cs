using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotWebhook.Services;

namespace TelegramBotWebhook.Command.BotCommand;

public class MPEIAccountUnlogCommand : SessionedBotCommand
{
    private IEmailAutentificationService? _autentificationService;

    public MPEIAccountUnlogCommand() : base(";", "mpeiaccountunlog")
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
    protected override async Task<ExecuteResult> ConcreteExecute(string option)
    {
        if (await _autentificationService!.TryUnlogin(Session))
        {
            Session.Login = null;
            Session.Password = null;

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

            return new ExecuteResult(ResultType.EditMessageWithInlineKeyboard, text)
            {
                InlineKeyboardMarkup = new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData(buttonText, ";mpeiaccountsave") },
                    new[] { InlineKeyboardButton.WithCallbackData("<< Назад", "/settings") },
                })
            };
        }
        else
        {
            return new ExecuteResult(ResultType.NoEdit);
        }
    }
}

