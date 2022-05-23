namespace TelegramBotWebhook.Services
{
    public interface ICommandExecuteService<TResult> where TResult: class
    {
        public Task<TResult> ExecuteCommand(string command, long userId);
        public Task<TResult> HandleResponse(string response, long userId);
        bool IsExecuteOver(long userId);
    }
}
