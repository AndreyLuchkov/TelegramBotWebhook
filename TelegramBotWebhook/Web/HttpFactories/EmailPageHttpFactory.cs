using TelegramBotWebhook.Web.MPEIEmail.Requests;
using TelegramBotWebhook.Web.MPEIEmail.ResponseHandlers;

namespace TelegramBotWebhook.Web.HttpFactories
{
    public class EmailPageHttpFactory : IHttpFactory
    {
        private readonly IPollingClient _pollingClient;

        public EmailPageHttpFactory(IPollingClient pollingClient)
        {
            _pollingClient = pollingClient;
        }

        public IHttpRequest GetRequest()
        {
            return new EmailPageHttpRequest(_pollingClient);
        }
        public IHttpResponseHandler GetResponseHandler()
        {
            return new EmailPageHttpResponseHandler();
        }
    }
}
