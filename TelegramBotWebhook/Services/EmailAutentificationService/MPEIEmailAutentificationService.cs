using TelegramBotWebhook.HtmlParsers;

namespace TelegramBotWebhook.Services
{
    public class MPEIEmailAutentificationService : IEmailAutentificationService
    {
        private readonly HttpWorker _httpWorker;

        public MPEIEmailAutentificationService(HttpWorker httpWorker)
        {
            _httpWorker = httpWorker;
        }

        public async Task<bool> TryAutentificate(MPEISession session)
        {
            if (session.Login is null || session.Password is null)
            {
                return false;
            }
            
            var response = _httpWorker.SendLoginRequest(session);

            UnlogKeyParser parser = new();

            session.UnlogKey = parser.Parse(await response);

            session.UserKey = _httpWorker.SetCookieValues.FirstOrDefault();

            return session.UserKey is not null && session.UnlogKey != String.Empty;
        }
        public async Task<bool> TryUnlogin(MPEISession session)
        {
            var response = _httpWorker.SendUnloginRequest(session);

            session.UserKey = null;
            session.UnlogKey = null;
            
            try
            {
                await response;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
