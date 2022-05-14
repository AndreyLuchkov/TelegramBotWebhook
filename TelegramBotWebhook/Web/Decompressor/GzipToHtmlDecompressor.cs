using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.IO.Compression;

namespace TelegramBotWebhook.Web.Decompressor
{
    public class GzipToHtmlDecompressor
    {
        private readonly HtmlParser _htmlParser;

        public GzipToHtmlDecompressor()
        {
            _htmlParser = new HtmlParser();
        }

        public Task<IHtmlDocument> DecompressToHtmlDoc(Stream stream)
        {
            using var decompressedBody = new GZipStream(stream, CompressionMode.Decompress);

            return Task.FromResult(_htmlParser.ParseDocument(decompressedBody));
        }
    }
}
