using TelegramBotWebhook.Web.MPEIEmail.Requests;
using TelegramBotWebhook.Web.MPEIEmail.ResponseHandlers;

namespace TelegramBotWebhook.Web.HttpFactories
{
    public class LoginHttpFactory : IHttpFactory
    {
        private readonly IPollingClient _pollingClient;

        public LoginHttpFactory(IPollingClient pollingClient)
        {
            _pollingClient = pollingClient;
        }

        public IHttpRequest GetRequest()
        {
            return new LoginHttpRequest(_pollingClient);
        }
        public IHttpResponseHandler GetResponseHandler()
        {
            return new LoginHttpResponseHandler();
        }
    }
}
