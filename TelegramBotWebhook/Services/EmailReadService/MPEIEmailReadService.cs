using TelegramBotWebhook.Web.HttpFactories;
using TelegramBotWebhook.Extensions;
using TelegramBotWebhook.MPEIEmail.EmailEntities;
using TelegramBotWebhook.HtmlParsers;
using AngleSharp.Html.Dom;
using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Services
{
    public class MPEIEmailReadService : IEmailReadService
    {
        private readonly IHttpFactory _loginHttpFactory;
        private readonly IHttpFactory _emailPageHttpFactory;

        public MPEIEmailReadService(IEnumerable<IHttpFactory> httpFactories)
        {
            _loginHttpFactory = httpFactories.Where((factory) => factory is LoginHttpFactory).First();
            _emailPageHttpFactory = httpFactories.Where((factory) => factory is EmailPageHttpFactory).First();
        }

        public async Task<IEnumerable<LetterRecord>> GetLetters(Session session)
        {
            Task<IHtmlDocument> response;
            if (session.UserKey is null)
            {
                response = GetLoginResponseFromEmail(session.UserId);
            }
            else
            {
                response = GetEmailPageResponse(session.UserId);
            }
           
            var letterParser = new LetterRecordsPageParser();

            IEnumerable<LetterRecord> letters = letterParser.Parse(await response);

            return letters;
        }
        private async Task<IHtmlDocument> GetLoginResponseFromEmail(long userId)
        {
            var request = _loginHttpFactory.GetRequest();

            var options = new HttpRequestOptions();
            options.GetLoginOptions(userId);

            var response = request.Send(options);

            var responseHandler = _loginHttpFactory.GetResponseHandler();

            return await responseHandler.HandleResponse(await response);
        }
        private async Task<IHtmlDocument> GetEmailPageResponse(long userId)
        {
            var request = _emailPageHttpFactory.GetRequest();

            var options = new HttpRequestOptions();
            options.GetEmailPageOptions(
                userId,
                pageNumber: 1);

            var response = request.Send(options);

            var responseHandler = _emailPageHttpFactory.GetResponseHandler();

            return await responseHandler.HandleResponse(await response);
        }
    }
}
