using System.Net.Http.Headers;
using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Web.MPEIEmail.Requests
{
    public class EmailPageHttpRequest : IHttpRequest
    {
        private readonly IPollingClient _pollingClient;
        private int PageNumber { get; set; }
        private Session? Session { get; set; }
        public EmailPageHttpRequest(IPollingClient pollingClient)
        {
            _pollingClient = pollingClient;
        }

        public async Task<HttpResponseMessage> Send(HttpRequestOptions options)
        {
            GetOptions(options);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/owa/");

            SetHeaders(request.Headers);
            SetContent(request);

            return await _pollingClient.Send(request);
        }
        private void GetOptions(HttpRequestOptions options)
        {
            int pageNumber;
            long userId;
            options.TryGetValue(new HttpRequestOptionsKey<long>("userId"), out userId);
            options.TryGetValue(new HttpRequestOptionsKey<int>("pageNumber"), out pageNumber);

            if (pageNumber == 0 || userId == 0)
                throw new Exception("The request options do not contain a userId or pageNumber key.");

            PageNumber = pageNumber;
            Session = Session.GetInstance(userId);
        }
        private void SetHeaders(HttpRequestHeaders headers)
        {
            headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            headers.AcceptEncoding.ParseAdd("gzip, deflate, br");
            headers.AcceptLanguage.ParseAdd("ru,en;q=0.9,en-GB;q=0.8,en-US;q=0.7");
            headers.Connection.ParseAdd("keep-alive");
            headers.Host = "legacy.mpei.ru";
            headers.Referrer = new Uri("https://legacy.mpei.ru/owa/");
            headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 Edg/100.0.1185.50");
            headers.Add("Cookie", $"_ym_uid=1650755988490906062; _ym_d=1650755988; _ym_isad=2; _ym_visorc=w; logondata=acc=0&lgn={Session!.Login}; {Session.UserKey};");
        }
        private void SetContent(HttpRequestMessage request)
        {
            request.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ae", "Folder"),
                new KeyValuePair<string, string>("t", "IPF.Note"),
                new KeyValuePair<string, string>("id", "LgAAAAAwKdr6ofryRp/EvmXd/7SLAQCE/tsTrZ4BTKDCJsVfen5wAPerJuDbAAAB"),
                new KeyValuePair<string, string>("slUsng", "0"),
                new KeyValuePair<string, string>("pg", $"{PageNumber}"),
            });
        }
    }
}
