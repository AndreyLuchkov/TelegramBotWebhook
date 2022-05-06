namespace TelegramBotWebhook.Command
{
    public interface ILongRunningCommand
    {
        public event Action? ExecuteIsOver; 
    }
}
