using System.Net.Http.Headers;

namespace TelegramBotWebhook.Web
{
    public class MPEIEmailPollingClient : IPollingClient
    {
        private readonly HttpClient _client;
        public Uri? BaseAddress => _client.BaseAddress;

        public MPEIEmailPollingClient(HttpClient client)
        {
            _client = client;
            client.BaseAddress = new Uri("https://legacy.mpei.ru");
        }

        public HttpRequestHeaders GetHeaders() => _client.DefaultRequestHeaders;
        public async Task<HttpResponseMessage> Send(HttpRequestMessage request) => await _client.SendAsync(request);
        
    }
}
