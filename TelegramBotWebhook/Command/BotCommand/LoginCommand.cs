using TelegramBotWebhook.Services;
using TelegramBotWebhook.Web.MPEIEmail;

namespace TelegramBotWebhook.Command.BotCommand
{
    public sealed class LoginCommand : SessionedBotCommand, ILongRunning
    {
        private IEmailAutentificationService? _autentificationService;

        public event Action? ExecuteIsOver;

        internal LoginCommand() : base("/login")
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
                if (await _autentificationService!.TryUnlogin(Session!))
                {
                    Session!.Login = null;
                    Session.Password = null;
                }
                else
                {
                    ExecuteIsOver?.Invoke();
                    return new ExecuteResult(ResultType.Text, "Ошибка окончания сессии.");
                }
            }

            if (Session!.Login is null && IsUserId(option))
            {
                return new ExecuteResult(ResultType.Text, "Введите логин для почты.");
            } 
            else if (Session.Login is null && option != String.Empty)
            {
                Session.Login = option;
                return new ExecuteResult(ResultType.Text, "Введите пароль.");
            }
            else
            {
                Session.Password = option;

                if (await _autentificationService!.TryAutentificate(Session))
                {
                    ExecuteIsOver?.Invoke();
                    return new ExecuteResult(ResultType.Text, "Вы успешно вошли на почту МЭИ.");
                }
                else
                {
                    ExecuteIsOver?.Invoke();
                    return new ExecuteResult(ResultType.Text, "Не удалось войти на почту МЭИ.\nВозможно вы ввели неверный логин или пароль.");
                }
            }
        }
        private bool IsUserId(string option) => long.TryParse(option, out long userId);
        private bool AlreadyLoggedIn() => Session!.Login is not null && Session.Password is not null;
    }
}
