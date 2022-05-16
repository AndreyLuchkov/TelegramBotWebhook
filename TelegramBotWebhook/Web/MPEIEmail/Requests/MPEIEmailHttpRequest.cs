using System.Net.Http.Headers;

namespace TelegramBotWebhook.Web.MPEIEmail.Requests
{
    public abstract class MPEIEmailHttpRequest : IHttpRequest
    {
        private readonly IPollingClient _pollingClient;
        public MPEIEmailHttpRequest(IPollingClient pollingClient)
        {
            _pollingClient = pollingClient;
        }
        public async Task<HttpResponseMessage> Send(HttpRequestOptions options)
        {
            GetOptions(options);

            HttpRequestMessage request = CreateRequestMessage();

            Task.WaitAll(SetHeaders(request.Headers), SetContent(request));

            var response = await _pollingClient.Send(request);

            AddHeadersToResponse(response.Headers);

            return response;
        }
        protected abstract void GetOptions(HttpRequestOptions options);
        protected abstract HttpRequestMessage CreateRequestMessage();
        protected abstract Task SetHeaders(HttpRequestHeaders headers);
        protected abstract Task SetContent(HttpRequestMessage request);
        protected abstract void AddHeadersToResponse(HttpResponseHeaders headers);
    }
}
