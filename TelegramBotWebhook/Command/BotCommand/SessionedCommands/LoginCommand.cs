using TelegramBotWebhook.Services;
using TelegramBotWebhook.Web.MPEIEmail;

namespace TelegramBotWebhook.Command.BotCommand
{
    public sealed class LoginCommand : SessionedBotCommand, ILongRunning
    {
        private IEmailAutentificationService? _autentificationService;

        public event Action? ExecuteIsOver;

        internal LoginCommand() : base("/", "login")
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
            if (AlreadyLoggedIn())
            {
                if (Session.UserKey is null)
                {
                    return await TryAutentificate();
                }
                else
                {
                    return new ExecuteResult(ResultType.Text, "Вход на почту МЭИ уже выполнен.");
                }
            }

            if (Session.Login is null && IsUserId(option))
            {
                return new ExecuteResult(ResultType.Text, "Введите логин для почты.");
            } 
            else if (Session.Login is null && option != String.Empty)
            {
                Session.Login = option;
                return new ExecuteResult(ResultType.Text, "Введите пароль.");
            }
            else if (Session.Login is not null && Session.Password is null && IsUserId(option))
            {
                return new ExecuteResult(ResultType.Text, "Введите пароль.");
            }
            else
            {
                Session.Password = option;
                return await TryAutentificate();
            }
        }
        private async Task<ExecuteResult> TryAutentificate()
        {
            if (await _autentificationService!.TryAutentificate(Session))
            {
                ExecuteIsOver?.Invoke();
                return new ExecuteResult(ResultType.Text, "<b>Вы успешно вошли на почту МЭИ.</b>\nСессия завершается автоматически через 15 мин. в случае неактивности пользователя.\nЧтобы настроить сохранение данных при входе воспользуйтесь командой /settings.");
            }
            else
            {
                ExecuteIsOver?.Invoke();
                return new ExecuteResult(ResultType.Text, "<b>Не удалось войти на почту МЭИ.</b>\nВозможно вы ввели неверный логин или пароль.");
            }
        }
        private bool IsUserId(string option) => long.TryParse(option, out long userId);
        private bool AlreadyLoggedIn() => Session.Login is not null && Session.Password is not null;
    }
}
