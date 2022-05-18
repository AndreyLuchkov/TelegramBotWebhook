using TelegramBotWebhook.Services;
using TelegramBotWebhook.Web.MPEIEmail;

namespace TelegramBotWebhook.Command.BotCommand
{
    public abstract class SessionedBotCommand : BotCommand, IServiceRequired
    {
        private readonly List<Type> _requiredServiceTypes; 
        
        private MPEIEmailSessionService? _sessionService;

        protected Session? Session { get; private set; }
        public IEnumerable<Type> RequiredServicesTypes { get => _requiredServiceTypes; }

        public SessionedBotCommand(string? text) : base(text) 
        {
            _requiredServiceTypes = new List<Type> { typeof(MPEIEmailSessionService) };
        }

        public void AddService(object service)
        {
            if (service is MPEIEmailSessionService sessionService)
            {
                _sessionService = sessionService;
            }

            AddNewService(service);
        }
        public override async Task<ExecuteResult> Execute(string option)
        {
            if (Session is null)
            {
                Session = _sessionService!.GetSession(GetUserId(option));
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
