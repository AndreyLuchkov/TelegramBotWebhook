using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Command
{
    public interface ISessionDepended : ICommand
    {
        Session? Session { get; set; }
    }
}
