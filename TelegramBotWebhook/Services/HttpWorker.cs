using AngleSharp.Html.Dom;
using TelegramBotWebhook.Extensions;
using TelegramBotWebhook.Web.HttpFactories;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Services
{
    public class HttpWorker
    {
        private readonly HttpFactories _httpFactories;

        public HttpWorker(HttpFactories httpFactories)
        {
            _httpFactories = httpFactories;
        }

        public IEnumerable<string> SetCookieValues { get; set; } = Array.Empty<string>();

        private async Task<IHtmlDocument> SendRequest(IHttpFactory httpFactory, HttpRequestOptions options)
        {
            var request = httpFactory.GetRequest();

            var response = request.Send(options);

            var responseHandler = httpFactory.GetResponseHandler();

            SetCookieValues = await responseHandler.GetSetCookieValues((await response).Headers);

            return await responseHandler.HandleResponse(await response);
        }
        public async Task<IHtmlDocument> SendLoginRequest(MPEISession session)
        {
            HttpRequestOptions options = new();
            options.GetLoginOptions(session);

            return await SendRequest(_httpFactories.GetHttpFactory("login"), options);
        }
        public async Task<IHtmlDocument> SendEmailPageRequest(MPEISession session, int pageNumber)
        {
            HttpRequestOptions options = new();
            options.GetEmailPageOptions(session, pageNumber);

            return await SendRequest(_httpFactories.GetHttpFactory("emailPage"), options);
        }
        public async Task<IHtmlDocument> SendLetterContentRequest(MPEISession session, LetterRecord letterRecord)
        {
            HttpRequestOptions options = new();
            options.GetReadLetterOptions(session, letterRecord);

            return await SendRequest(_httpFactories.GetHttpFactory("letterContent"), options);
        }
        public async Task<IHtmlDocument> SendUnloginRequest(MPEISession session)
        {
            HttpRequestOptions options = new();
            options.GetLoginOptions(session);

            return await SendRequest(_httpFactories.GetHttpFactory("unlogin"), options);
        }
    }
}
