using TelegramBotWebhook.Web;
using TelegramBotWebhook.Web.HttpFactories;

namespace TelegramBotWebhook.Services
{
    public class HttpFactories
    {
        IPollingClient _pollingClient;

        public HttpFactories(IPollingClient pollingClient)
        {
            _pollingClient = pollingClient;
        }

        public IHttpFactory GetHttpFactory(string factoryName)
        {
            return factoryName.ToLower() switch
            {
                "login" => new LoginHttpFactory(_pollingClient),
                "emailpage" => new EmailPageHttpFactory(_pollingClient),
                "lettercontent" => new LetterContentHttpFactory(_pollingClient),
                "unlogin" => new UnloginHttpFactory(_pollingClient),
                _ => throw new ArgumentException("Unable to find the requested HTTP factory."),
            };
        }
    }
}
