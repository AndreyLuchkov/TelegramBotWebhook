using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.Services;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class LoginCommand : BotCommand, ILongRunning, ISessionDepended, IServiceRequired
    {
        private List<object> _services;

        public Session? Session { get; set; }
        public IEnumerable<Type> RequiredServicesTypes { get; }

        public event Action? ExecuteIsOver;

        internal LoginCommand() : base("/login")
        {
            RequiredServicesTypes = new Type[1] { typeof(IEmailAutentificationService) };
            _services = new List<object>(1);
        }
        
        public void AddService(object service)
        {
            if (_services.Count != RequiredServicesTypes.Count())
                _services.Add(service);
        }
        public override async Task<ExecuteResult> Execute(string option)
        {
            if (Session is null)
                throw new NullReferenceException("The session property is null.");

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

                var autentificationService = (IEmailAutentificationService)_services.First();

                if (await autentificationService.TryAutentificate(Session))
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

        
    }
}
