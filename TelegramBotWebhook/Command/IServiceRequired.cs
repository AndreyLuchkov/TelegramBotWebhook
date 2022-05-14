namespace TelegramBotWebhook.Command
{
    public interface IServiceRequired : ICommand
    {
        IEnumerable<Type> RequiredServicesTypes { get; }
        public void AddService(object service);
    }
}
