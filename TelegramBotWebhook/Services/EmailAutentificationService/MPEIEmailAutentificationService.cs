using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Services
{
    public class MPEIEmailAutentificationService : IEmailAutentificationService
    {
        private readonly HttpWorker _httpWorker;

        public MPEIEmailAutentificationService(HttpWorker httpWorker)
        {
            _httpWorker = httpWorker;
        }

        public async Task<bool> TryAutentificate(Session session)
        {
            if (session.Login is null || session.Password is null)
            {
                throw new NullReferenceException("Session login or password property is null.");
            }

            await _httpWorker.SendLoginRequest(session);

            return session.UserKey is not null;
        }
    }
}
