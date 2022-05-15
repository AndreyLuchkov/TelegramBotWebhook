using TelegramBotWebhook.Web.HttpFactories;
using TelegramBotWebhook.Extensions;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;
using TelegramBotWebhook.HtmlParsers;
using AngleSharp.Html.Dom;
using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Services
{
    public class MPEIEmailReadService : IEmailReadService
    {
        private readonly HttpWorker _httpWorker;

        public MPEIEmailReadService(HttpWorker httpWorker)
        {
            _httpWorker = httpWorker;
        }

        public async Task<IEnumerable<LetterRecord>> GetLetters(Session session)
        {
            var response = _httpWorker.SendEmailPageRequest(session, 1);
           
            var letterParser = new LetterRecordsPageParser();

            IEnumerable<LetterRecord> letters = letterParser.Parse(await response);

            return letters;
        }
    }
}
