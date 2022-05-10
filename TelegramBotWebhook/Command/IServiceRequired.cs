namespace TelegramBotWebhook.Command
{
    public interface IServiceRequired
    {
        IEnumerable<Type> RequiredServicesTypes { get; }
        public void AddService(object service);
    }
}
