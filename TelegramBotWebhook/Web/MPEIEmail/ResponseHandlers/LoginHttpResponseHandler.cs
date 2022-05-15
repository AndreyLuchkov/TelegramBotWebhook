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
            if (!(response.StatusCode == HttpStatusCode.OK))
                throw new Exception($"The unexpected response status code: {response.StatusCode}");

            long userId = GetUserIdFromHeaders(response.Headers);
            
            string setCookieHeaderValue = GetSetCookieHeaderValue(response.Headers);

            Session.GetInstance(userId).UserKey = setCookieHeaderValue;

            var decompressor = new GzipToHtmlDecompressor();
            return await decompressor.DecompressToHtmlDoc(await response.Content.ReadAsStreamAsync());
        }
        private string GetSetCookieHeaderValue(HttpResponseHeaders headers)
        {
            IEnumerable<string>? setCookieValues;
            if (headers.TryGetValues("Set-Cookie", out setCookieValues))
                return setCookieValues.First().Split(';').First();
            else
                throw new NullReferenceException("The response headers do not have a Set-Cookie header.");
        }
        private long GetUserIdFromHeaders(HttpResponseHeaders headers)
        {
            IEnumerable<string>? userId;
            if (headers.TryGetValues("userId", out userId))
                return long.Parse(userId.First());
            else
                throw new NullReferenceException("The response headers do not have a userId header.");
        }
    }
}
