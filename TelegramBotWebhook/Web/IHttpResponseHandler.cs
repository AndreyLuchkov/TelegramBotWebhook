using AngleSharp.Html.Dom;
using System.Net.Http.Headers;

namespace TelegramBotWebhook.Web
{
    public interface IHttpResponseHandler
    {
        public Task<IHtmlDocument> HandleResponse(HttpResponseMessage response);

        public Task<IEnumerable<string>> GetSetCookieValues(HttpResponseHeaders headers);
    }
}
