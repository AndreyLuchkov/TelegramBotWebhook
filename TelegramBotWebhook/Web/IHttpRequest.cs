namespace TelegramBotWebhook.Web
{
    public interface IHttpRequest
    {
        public Task<HttpResponseMessage> Send(HttpRequestOptions options);
    }
}
