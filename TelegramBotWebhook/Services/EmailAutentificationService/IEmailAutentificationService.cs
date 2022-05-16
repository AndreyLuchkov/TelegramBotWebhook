using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Services
{
    public interface IEmailAutentificationService
    {
        public Task<bool> TryAutentificate(Session session);
        public Task<bool> TryUnlogin(Session session);
    }
}
