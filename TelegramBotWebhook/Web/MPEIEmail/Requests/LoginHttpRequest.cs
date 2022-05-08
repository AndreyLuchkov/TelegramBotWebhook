using System.Net.Http.Headers;
using TelegramBotWebhook.Web;

namespace TelegramBot.Web.MPEIEmail.Requests
{
    public class LoginHttpRequest : IHttpRequest
    {
        private readonly IPollingClient pollingClient;

        public LoginHttpRequest(IPollingClient pollingClient)
        {
            this.pollingClient = pollingClient;
        }

        public async Task<HttpResponseMessage> Send(HttpRequestOptions options)
        {
            Session session = Session.Instance();

            HttpRequestMessage request;
            if (session.CookieKey1 is null)
            {
                request = new HttpRequestMessage(HttpMethod.Post, "/CookieAuth.dll?Logon");

                GetLoginOptions(options);
            }
            else if (session.CookieKey1 is not null && session.CookieKey2 is null)
            {
                request = new HttpRequestMessage(HttpMethod.Get, "/owa");
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Get, "/owa/");
            }

            SetHeaders(pollingClient.GetHeaders());
            SetContent(request);

            return await pollingClient.Send(request);
        }
        private void GetLoginOptions(HttpRequestOptions options)
        {
            Session session = Session.Instance();

            string? login, password;
            options.TryGetValue(new HttpRequestOptionsKey<string>("login"), out login);
            options.TryGetValue(new HttpRequestOptionsKey<string>("password"), out password);

            if (login is null || password is null)
                throw new Exception("The request options do not contain objects with a login and password key.");

            session.Login = login;
            session.Password = password;
        }
        private void SetHeaders(HttpRequestHeaders headers)
        {
            Session session = Session.Instance();
            
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
            
            if (session.CookieKey1 is not null && session.CookieKey2 is null)
                headers.Add("Cookie", $"_ym_uid=1650755988490906062; _ym_d=1650755988; _ym_isad=2; _ym_visorc=w; logondata=acc=0&lgn={session.Login}; {session.CookieKey1}");
            else if (session.CookieKey2 is not null)
            {
                headers.Add("Cookie", $"_ym_uid=1650755988490906062; _ym_d=1650755988; _ym_isad=2; _ym_visorc=w; logondata=acc=0&lgn={session.Login}; {session.CookieKey1}; {session.CookieKey2}");
            }
            else
            {
                headers.Add("Cookie", $"_ym_uid=1650755988490906062; _ym_d=1650755988; _ym_isad=2; _ym_visorc=w; logondata=acc=0&lgn={session.Login}");
            }
        }
        private void SetContent(HttpRequestMessage request)
        {
            Session session = Session.Instance();

            request.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("curl", "Z2Fowa"),
                new KeyValuePair<string, string>("flags", "0"),
                new KeyValuePair<string, string>("forcedownlevel", "0"),
                new KeyValuePair<string, string>("formdir", "2"),
                new KeyValuePair<string, string>("username", $"{session.Login}"),
                new KeyValuePair<string, string>("password", $"{session.Password}"),
                new KeyValuePair<string, string>("isUtf8", "1"),
                new KeyValuePair<string, string>("trusted", "4"),
            });
        }
    }
}
