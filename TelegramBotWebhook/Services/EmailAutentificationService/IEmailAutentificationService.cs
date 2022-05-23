namespace TelegramBotWebhook.Services
{
    public interface IEmailAutentificationService
    {
        public Task<bool> TryAutentificate(MPEISession session);
        public Task<bool> TryUnlogin(MPEISession session);
    }
}
