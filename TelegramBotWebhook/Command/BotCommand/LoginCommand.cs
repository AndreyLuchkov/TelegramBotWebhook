using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.Services;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class LoginCommand : BotCommand, ILongRunning, ISessionDepended, IServiceRequired
    {
        IEmailAutentificationService? _autentificationService;

        public Session? Session { get; set; }
        public IEnumerable<Type> RequiredServicesTypes { get; }

        public event Action? ExecuteIsOver;

        internal LoginCommand() : base("/login")
        {
            RequiredServicesTypes = new Type[1] { typeof(IEmailAutentificationService) };
        }
        
        public void AddService(object service)
        {
            if (service is IEmailAutentificationService autentificationService)
            {
                _autentificationService = autentificationService;
            }
        }
        public override async Task<ExecuteResult> Execute(string option)
        {
            if (Session is null)
                throw new InvalidOperationException("The session property is null.");
            if (_autentificationService is null)
                throw new InvalidOperationException("The required services has not added.");

            if (AlreadyLoggedIn())
            {
                if (await _autentificationService!.TryUnlogin(Session))
                {
                    Session.Login = null;
                    Session.Password = null;
                }
                else
                {
                    ExecuteIsOver?.Invoke();
                    return new ExecuteResult(ResultType.Text, "Ошибка окончания сессии.");
                }
            }

            if (Session.Login is null && option == String.Empty)
            {
                return new ExecuteResult(ResultType.Text, "Введите логин для почты:");
            } 
            else if (Session.Login is null && option != String.Empty)
            {
                Session.Login = option;
                return new ExecuteResult(ResultType.Text, "Введите пароль:");
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
        private bool AlreadyLoggedIn() => Session!.Login is not null && Session.Password is not null;
    }
}
