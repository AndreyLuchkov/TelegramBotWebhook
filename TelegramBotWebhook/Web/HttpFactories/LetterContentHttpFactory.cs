using TelegramBotWebhook.Web.MPEIEmail.Requests;
using TelegramBotWebhook.Web.MPEIEmail.ResponseHandlers;

namespace TelegramBotWebhook.Web.HttpFactories
{
    public class LetterContentHttpFactory : IHttpFactory
    {
        private readonly IPollingClient _pollingClient;

        public LetterContentHttpFactory(IPollingClient pollingClient)
        {
            _pollingClient = pollingClient;
        }

        public IHttpRequest GetRequest()
        {
            return new LetterContentHttpRequest(_pollingClient);
        }
        public IHttpResponseHandler GetResponseHandler()
        {
            return new LetterContentHttpResponseHandler();
        }
    }
}
