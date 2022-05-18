using TelegramBotWebhook.HtmlParsers;
using TelegramBotWebhook.Web.MPEIEmail;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

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
