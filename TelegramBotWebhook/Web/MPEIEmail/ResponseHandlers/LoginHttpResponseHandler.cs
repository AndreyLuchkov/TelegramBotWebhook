using System.Net;
using System.Net.Http.Headers;
using AngleSharp.Html.Dom;
using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.Web.Decompressor;

namespace TelegramBotWebhook.Web.MPEIEmail.ResponseHandlers
{
    public class LoginHttpResponseHandler : IHttpResponseHandler
    {
        public async Task<IHtmlDocument> HandleResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"The unexpected response status code: {response.StatusCode}");

            long userId = GetUserIdFromHeaders(response.Headers);

            Session.GetInstance(userId).UserKey = GetSetCookieHeaderValue(response.Headers);

            var decompressor = new GzipToHtmlDecompressor();
            return await decompressor.DecompressToHtmlDoc(await response.Content.ReadAsStreamAsync());
        }
        private string? GetSetCookieHeaderValue(HttpResponseHeaders headers)
        {
            IEnumerable<string>? setCookieValues;
            if (headers.TryGetValues("Set-Cookie", out setCookieValues))
                return setCookieValues.First().Split(';').First();
            else
                return null;
        }
        private long GetUserIdFromHeaders(HttpResponseHeaders headers)
        {
            IEnumerable<string>? userId;
            if (headers.TryGetValues("User-Id", out userId))
                return long.Parse(userId.First());
            else
                throw new NullReferenceException("The response headers do not have a User-Id header.");
        }
    }
}
