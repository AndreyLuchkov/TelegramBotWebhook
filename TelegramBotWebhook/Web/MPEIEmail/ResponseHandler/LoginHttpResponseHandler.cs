using System.Net;
using System.Diagnostics;
using System.IO.Compression;
using TelegramBotWebhook.Web;
using System.Net.Http.Headers;

namespace TelegramBot.Web.MPEIEmail.ResponseHandlers
{
    public class LoginHttpResponseHandler : IHttpResponseHandler
    {
        public async Task<DirectoryInfo> HandleResponse(HttpResponseMessage response)
        {
            if (!(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Redirect))
                throw new Exception($"The unexpected response status code: {response.StatusCode}");

            string setCookieHeaderValue = GetSetCookieHeader(response.Headers);
            
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
            
            DirectoryInfo bodyFileDirectory = Directory.CreateDirectory(@"C:\Users\purpl\source\repos\TelegramBotWebhook\TelegramBotWebhook\bin\Debug\net6.0\MPEIEmail\MainPages");
            await SaveResponseBodyToFile(bodyFileDirectory, response.Content);

            return bodyFileDirectory;
        }
        private string GetSetCookieHeader(HttpResponseHeaders headers)
        {
            IEnumerable<string>? setCookieValues;
            if (headers.TryGetValues("Set-Cookie", out setCookieValues))
                return setCookieValues.First().Split(';').First();
            else
                throw new NullReferenceException("The response headers do not have a set-cookie header.");
        }
        private async Task SaveResponseBodyToFile(DirectoryInfo fileDirectory, HttpContent body)
        {
            using Stream bodyStream = await body.ReadAsStreamAsync();
            using var decompressedBody = new GZipStream(bodyStream, CompressionMode.Decompress);

            var options = new FileStreamOptions()
            {
                Access = FileAccess.Write,
                Mode = FileMode.OpenOrCreate,
            };
            using FileStream file = new FileStream(fileDirectory.FullName + @$"\Page 1.html", options);

            decompressedBody.CopyTo(file);
        }
    }
}
