using TelegramBotWebhook.Web.MPEIEmail.Requests;
using TelegramBotWebhook.Web.MPEIEmail.ResponseHandlers;

namespace TelegramBotWebhook.Web.HttpFactories
{
    public class UnloginHttpFactory : IHttpFactory
    {
        private readonly IPollingClient _pollingClient;

        public UnloginHttpFactory(IPollingClient pollingClient)
        {
            _pollingClient = pollingClient;
        }

        public IHttpRequest GetRequest()
        {
            return new UnloginHttpRequest(_pollingClient);
        }
        public IHttpResponseHandler GetResponseHandler()
        {
            return new UnloginResponseHandler();
        }
    }
}
