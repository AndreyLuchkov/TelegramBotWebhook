using TelegramBotWebhook.Services;

namespace TelegramBotWebhook.Command.BotCommand
{
    public abstract class SessionedBotCommand : BotCommand, IServiceRequired
    {
        private readonly List<Type> _requiredServiceTypes; 
        
        private ISessionService<MPEISession>? _sessionService;
        private IEmailAutentificationService? _autentificationService;

        protected MPEISession Session { get; private set; } = null!;
        public IEnumerable<Type> RequiredServicesTypes { get => _requiredServiceTypes; }

        public SessionedBotCommand(string prefix, string? text) : base(prefix, text) 
        {
            _requiredServiceTypes = new List<Type> { typeof(ISessionService<MPEISession>), typeof(IEmailAutentificationService) };
        }

        public void AddService(object service)
        {
            if (service is ISessionService<MPEISession> sessionService)
            {
                _sessionService = sessionService;
            } 
            else if (service is IEmailAutentificationService autentificationService) 
            {
                _autentificationService = autentificationService;
            }

            AddNewService(service);
        }
        public override async Task<ExecuteResult> Execute(string option)
        {
            if (Session is null)
            {
                Session = await _sessionService!.GetSession(GetUserId(option));
            }
            if (Text != "login" && Text != "settings" && Prefix == "/")
            {
                if (Session.UserKey is null)
                {
                    if (Session.SaveCredentials && Session.Login is not null)
                    {
                        if (Session.Password == null)
                        {
                            return new ExecuteResult(ResultType.Text, "Введите пароль.");
                        }
                        else
                        {
                            await _autentificationService!.TryAutentificate(Session);
                        }
                    }
                    else
                    {
                        return new ExecuteResult(ResultType.Text, "Для использования данной команды необходимо выполнить вход на почту МЭИ.\nВоспользуйтесь командой /login, чтобы войти на почту.");
                    }
                }
            }

            return await ConcreteExecute(option);
        }
        private long GetUserId(string option)
        {
            long userId;
            if (long.TryParse(option, out userId))
            {
                return userId;
            }
            else
            {
                throw new ArgumentException("The incorrect user ID in the received option.");
            }
        }
        protected void AddRequiredServiceType(Type serviceType)
        {
            _requiredServiceTypes.Add(serviceType);
        }
        protected void AddRequiredServiceTypes(IEnumerable<Type> serviceTypes)
        {
            _requiredServiceTypes.AddRange(serviceTypes);
        }
        protected abstract Task<ExecuteResult> ConcreteExecute(string option);
        protected abstract void AddNewService(object service);
        
    }
}
