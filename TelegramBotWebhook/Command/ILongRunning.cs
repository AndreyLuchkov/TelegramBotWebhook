namespace TelegramBotWebhook.Command
{
    public interface ILongRunning
    {
        public event Action? ExecuteIsOver; 
    }
}
