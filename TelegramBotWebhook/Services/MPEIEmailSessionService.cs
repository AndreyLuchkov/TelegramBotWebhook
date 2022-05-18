using TelegramBotWebhook.Web.MPEIEmail;

namespace TelegramBotWebhook.Services
{
    public class MPEIEmailSessionService
    {
        public Session StartSession(long userId) => Session.GetInstance(userId);
        public Session GetSession(long userId) => Session.GetInstance(userId);
    }
}
