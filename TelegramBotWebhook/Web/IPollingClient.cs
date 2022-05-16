using System.Net.Http.Headers;

namespace TelegramBotWebhook.Web
{
    public interface IPollingClient
    {
        public Uri? BaseAddress { get; }

        public Task<HttpResponseMessage> Send(HttpRequestMessage request);
    }
}
