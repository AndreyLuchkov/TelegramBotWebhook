using AngleSharp.Html.Dom;
using TelegramBotWebhook.Extensions;
using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.Web.HttpFactories;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Services
{
    public class HttpWorker
    {
        HttpFactories _httpFactories;

        public HttpWorker(HttpFactories httpFactories)
        {
            _httpFactories = httpFactories;
        }

        private async Task<IHtmlDocument> SendRequest(IHttpFactory httpFactory, HttpRequestOptions options)
        {
            var request = httpFactory.GetRequest();

            var response = request.Send(options);

            var responseHandler = httpFactory.GetResponseHandler();

            return await responseHandler.HandleResponse(await response);
        }
        public async Task<IHtmlDocument> SendLoginRequest(Session session)
        {
            HttpRequestOptions options = new();
            options.GetLoginOptions(session);

            return await SendRequest(_httpFactories.GetHttpFactory("login"), options);
        }
        public async Task<IHtmlDocument> SendEmailPageRequest(Session session, int pageNumber)
        {
            HttpRequestOptions options = new();
            options.GetEmailPageOptions(session, pageNumber);

            return await SendRequest(_httpFactories.GetHttpFactory("emailPage"), options);
        }
        public async Task<IHtmlDocument> SendLetterContentRequest(Session session, LetterRecord letterRecord)
        {
            HttpRequestOptions options = new();
            options.GetReadLetterOptions(session, letterRecord);

            return await SendRequest(_httpFactories.GetHttpFactory("letterContent"), options);
        }
        public async Task<IHtmlDocument> SendUnloginRequest(Session session)
        {
            HttpRequestOptions options = new();
            options.GetLoginOptions(session);

            return await SendRequest(_httpFactories.GetHttpFactory("unlogin"), options);
        }
    }
}
