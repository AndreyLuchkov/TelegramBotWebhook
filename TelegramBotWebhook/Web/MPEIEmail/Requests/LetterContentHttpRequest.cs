using System.Net.Http.Headers;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Web.MPEIEmail.Requests
{
    public class LetterContentHttpRequest : MPEIEmailHttpRequest
    {
        private MPEISession? Session { get; set; }
        private LetterRecord? Letter { get; set; }

        public LetterContentHttpRequest(IPollingClient pollingClient) : base(pollingClient) 
        {
        }

        protected override void GetOptions(HttpRequestOptions options)
        {
            LetterRecord? letterRecord;
            MPEISession? session;
            options.TryGetValue(new HttpRequestOptionsKey<MPEISession>("session"), out session);
            options.TryGetValue(new HttpRequestOptionsKey<LetterRecord>("letterRecord"), out letterRecord);

            if (letterRecord is null || session is null)
                throw new Exception("Request options do not contain a letterRecord or userId key.");

            Session = session;
            Letter = letterRecord;
        }
        protected override HttpRequestMessage CreateRequestMessage() => new HttpRequestMessage(HttpMethod.Get, $"/owa/?ae=Item&t={Letter!.Type}&id={Letter.LetterKey}");
        protected override Task SetHeaders(HttpRequestHeaders headers)
        {
            headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            headers.AcceptEncoding.ParseAdd("gzip, deflate, br");
            headers.AcceptLanguage.ParseAdd("ru,en;q=0.9,en-GB;q=0.8,en-US;q=0.7");
            headers.Connection.ParseAdd("keep-alive");
            headers.Host = "legacy.mpei.ru";
            headers.Referrer = new Uri("https://legacy.mpei.ru/owa/");
            headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 Edg/100.0.1185.50");
            headers.Add("Cookie", $"_ym_uid=1650755988490906062; _ym_d=1650755988; _ym_isad=2; _ym_visorc=w; logondata=acc=0&lgn={Session!.Login}; {Session.UserKey};");

            return Task.CompletedTask;
        }
        protected override Task SetContent(HttpRequestMessage request) => Task.CompletedTask;
        protected override void AddHeadersToResponse(HttpResponseHeaders headers) { }
    }
}
