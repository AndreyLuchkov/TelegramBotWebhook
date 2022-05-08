namespace TelegramBotWebhook.Command
{
    public interface IServiceRequired
    {
        IEnumerable<Type> RequiredServicesTypes { get; }
        List<object> Services { get; }
    }
}
