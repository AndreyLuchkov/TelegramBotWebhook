using System.Net;
using System.Net.Http.Headers;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace TelegramBotWebhook.Web.MPEIEmail.ResponseHandlers
{
    public class UnloginResponseHandler : IHttpResponseHandler
    {
        public Task<IEnumerable<string>> GetSetCookieValues(HttpResponseHeaders headers) => Task.FromResult((IEnumerable<string>)Array.Empty<string>());
        public Task<IHtmlDocument> HandleResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"The unexpected response status code: {response.StatusCode}");

            HtmlParser parser = new();
            return Task.FromResult(parser.ParseDocument(""));
        }
    }
}
