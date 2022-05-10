using System.Net;
using System.IO.Compression;
using System.Net.Http.Headers;
using TelegramBotWebhook.Web;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace TelegramBot.Web.MPEIEmail.ResponseHandlers
{
    public class LoginHttpResponseHandler : IHttpResponseHandler
    {
        public async Task<IHtmlDocument> HandleResponse(HttpResponseMessage response)
        {
            if (!(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Redirect))
                throw new Exception($"The unexpected response status code: {response.StatusCode}");

            string setCookieHeaderValue = GetSetCookieHeaderValue(response.Headers);
            
            var session = Session.Instance();
            if (setCookieHeaderValue.Contains("cadata"))
            {
                session.CookieKey1 = setCookieHeaderValue;
            }
            else if (setCookieHeaderValue.Contains("ISAWPLB"))
            {
                session.CookieKey2 = setCookieHeaderValue;
            }
            else
            {
                session.UserKey = setCookieHeaderValue;
            }

            var htmlDoc = await DecompressResponseBodyToHtmlDoc(response.Content);
            response.Dispose();

            return htmlDoc;
        }
        private string GetSetCookieHeaderValue(HttpResponseHeaders headers)
        {
            IEnumerable<string>? setCookieValues;
            if (headers.TryGetValues("Set-Cookie", out setCookieValues))
                return setCookieValues.First().Split(';').First();
            else
                throw new NullReferenceException("The response headers do not have a set-cookie header.");
        }
        private async Task<IHtmlDocument> DecompressResponseBodyToHtmlDoc(HttpContent body)
        {
            using Stream bodyStream = await body.ReadAsStreamAsync();
            using var decompressedBody = new GZipStream(bodyStream, CompressionMode.Decompress);

            var htmlParser = new HtmlParser();
            var htmlDoc = htmlParser.ParseDocument(decompressedBody);

            return htmlDoc;
        }
    }
}
