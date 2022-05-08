namespace TelegramBotWebhook.Web
{
    public interface IHttpResponseHandler
    {
        public Task<DirectoryInfo> HandleResponse(HttpResponseMessage response);
    }
}
