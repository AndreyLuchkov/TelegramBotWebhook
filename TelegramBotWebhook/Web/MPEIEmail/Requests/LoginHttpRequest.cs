using System.Net.Http.Headers;

namespace TelegramBotWebhook.Web.MPEIEmail.Requests
{
    public class LoginHttpRequest : MPEIEmailHttpRequest
    {
        private Session? Session { get; set; }

        public LoginHttpRequest(IPollingClient pollingClient) : base(pollingClient) { }

        protected override void GetOptions(HttpRequestOptions options)
        {
            Session? session;
            options.TryGetValue(new HttpRequestOptionsKey<Session>("session"), out session);

            if (session is null)
                throw new Exception("The request options do not contain a session key.");

            Session = session;
        }
        protected override HttpRequestMessage CreateRequestMessage() => new HttpRequestMessage(HttpMethod.Post, "/CookieAuth.dll?Logon");
        protected override Task SetHeaders(HttpRequestHeaders headers)
        {
            headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            headers.AcceptEncoding.ParseAdd("gzip, deflate, br");
            headers.AcceptLanguage.ParseAdd("ru,en;q=0.9,en-GB;q=0.8,en-US;q=0.7");
            headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.Zero
            };
            headers.Connection.ParseAdd("keep-alive");
            headers.Host = "legacy.mpei.ru";
            headers.Referrer = new Uri("https://legacy.mpei.ru/CookieAuth.dll?GetLogon?curl=Z2Fowa&reason=0&formdir=2");
            headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 Edg/100.0.1185.50");
            headers.Add("Cookie", $"_ym_uid=1650755988490906062; _ym_d=1650755988; _ym_isad=2; _ym_visorc=w; logondata=acc=0&lgn={Session!.Login}");

            return Task.CompletedTask;
        }
        protected override Task SetContent(HttpRequestMessage request)
        {
            request.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("curl", "Z2Fowa"),
                new KeyValuePair<string, string>("flags", "0"),
                new KeyValuePair<string, string>("forcedownlevel", "0"),
                new KeyValuePair<string, string>("formdir", "2"),
                new KeyValuePair<string, string>("username", $"{Session!.Login}"),
                new KeyValuePair<string, string>("password", $"{Session.Password}"),
                new KeyValuePair<string, string>("isUtf8", "1"),
                new KeyValuePair<string, string>("trusted", "4"),
            });

            return Task.CompletedTask;
        }
        protected override void AddHeadersToResponse(HttpResponseHeaders headers) 
        {
            headers.Add("User-Id", $"{Session!.UserId}");
        }
    }
}
