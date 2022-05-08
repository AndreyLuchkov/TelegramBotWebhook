namespace TelegramBotWebhook.Web.HttpFactories
{
    public interface IHttpFactory
    {
        IHttpRequest GetRequest();
        IHttpResponseHandler GetResponseHandler();
    }
}
