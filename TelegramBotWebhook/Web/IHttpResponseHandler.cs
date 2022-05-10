using AngleSharp.Html.Dom;

namespace TelegramBotWebhook.Web
{
    public interface IHttpResponseHandler
    {
        public Task<IHtmlDocument> HandleResponse(HttpResponseMessage response);
    }
}
