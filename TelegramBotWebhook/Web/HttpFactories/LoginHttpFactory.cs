using TelegramBot.Web.MPEIEmail.Requests;
using TelegramBot.Web.MPEIEmail.ResponseHandlers;

namespace TelegramBotWebhook.Web.HttpFactories
{
    public class LoginHttpFactory : IHttpFactory
    {
        private readonly IPollingClient pollingClient;

        public LoginHttpFactory(IPollingClient pollingClient)
        {
            this.pollingClient = pollingClient;
        }

        public IHttpRequest GetRequest()
        {
            return new LoginHttpRequest(pollingClient);
        }
        public IHttpResponseHandler GetResponseHandler()
        {
            return new LoginHttpResponseHandler();
        }
    }
}
