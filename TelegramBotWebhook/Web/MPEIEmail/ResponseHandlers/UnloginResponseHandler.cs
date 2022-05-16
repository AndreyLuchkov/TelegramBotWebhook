using System.Net;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace TelegramBotWebhook.Web.MPEIEmail.ResponseHandlers
{
    public class UnloginResponseHandler : IHttpResponseHandler
    {
        public Task<IHtmlDocument> HandleResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"The unexpected response status code: {response.StatusCode}");

            HtmlParser parser = new();
            return Task.FromResult(parser.ParseDocument(""));
        }
    }
}
