using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Services
{
    public interface ISessionService
    {
        public Session StartSession(long userId);
        public Session GetSession(long userId);
    }
}
