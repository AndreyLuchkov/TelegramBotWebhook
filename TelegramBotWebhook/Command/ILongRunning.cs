namespace TelegramBotWebhook.Command
{
    public interface ILongRunning : ICommand
    {
        public event Action? ExecuteIsOver; 
    }
}
