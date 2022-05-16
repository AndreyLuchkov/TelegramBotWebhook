using System.Net;
using AngleSharp.Html.Dom;
using TelegramBotWebhook.Web.Decompressor;

namespace TelegramBotWebhook.Web.MPEIEmail.ResponseHandlers
{
    public class LetterContentHttpResponseHandler : IHttpResponseHandler
    {
        public async Task<IHtmlDocument> HandleResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"The unexpected response status code: {response.StatusCode}");

            var decompressor = new GzipToHtmlDecompressor();
            return await decompressor.DecompressToHtmlDoc(await response.Content.ReadAsStreamAsync());
        }
    }
}
