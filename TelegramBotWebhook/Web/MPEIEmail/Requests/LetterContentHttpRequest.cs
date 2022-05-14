using System.Net.Http.Headers;
using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Web.MPEIEmail.Requests
{
    internal class LetterContentHttpRequest : IHttpRequest
    {
        private readonly IPollingClient _pollingClient;
        private Session? Session { get; set; }
        private LetterRecord? Letter { get; set; }
        public LetterContentHttpRequest(IPollingClient pollingClient)
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
            long userId;
            LetterRecord? letterRecord;
            options.TryGetValue(new HttpRequestOptionsKey<LetterRecord>("letterRecord"), out letterRecord);
            options.TryGetValue(new HttpRequestOptionsKey<long>("userId"), out userId);

            if (letterRecord is null || userId == 0)
                throw new Exception("Request options do not contain a letterRecord or userId key.");

            Session = Session.GetInstance(userId);
            Letter = letterRecord;
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
                new KeyValuePair<string, string>("ae", "Item"),
                new KeyValuePair<string, string>("t", $"{Letter!.Type}"),
                new KeyValuePair<string, string>("id", $"{Letter.LetterKey}"),
            });
        }
    }
}
