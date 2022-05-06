using TelegramBotWebhook.Command;

namespace TelegramBotWebhook.Services
{
    public interface ICommandExecuteService<TResult> where TResult: class
    {
        public Task<TResult> ExecuteCommand(string command);
        public Task<TResult> HandleResponse(string response);
        bool ExecuteIsOver();
    }
}
