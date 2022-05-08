using System.Net.Http.Headers;

namespace TelegramBotWebhook.Web
{
    public class MPEIEmailPollingClient : IPollingClient
    {
        private readonly HttpClient client;
        public Uri? BaseAddress => client.BaseAddress;

        public MPEIEmailPollingClient(HttpClient client)
        {
            this.client = client;
            client.BaseAddress = new Uri("https://legacy.mpei.ru");
        }

        public HttpRequestHeaders GetHeaders() => client.DefaultRequestHeaders;
        public async Task<HttpResponseMessage> Send(HttpRequestMessage request) => await client.SendAsync(request);
        
    }
}
