using AngleSharp.Html.Dom;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;
using TelegramBotWebhook.Web.HttpFactories;
using TelegramBotWebhook.HtmlParsers;
using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Services
{
    public class LessonLetterReadService : IEmailLetterReadService<LessonLetter>
    {
        private readonly HttpWorker _httpWorker;

        public LessonLetterReadService(HttpWorker httpWorker)
        {
            _httpWorker = httpWorker;
        }

        public async Task<LessonLetter> ReadLetter(Session session, LetterRecord letterRecord)
        {
            if (letterRecord.Type != "IPM.Schedule.Meeting.Request")
            {
                throw new ArgumentException("The letter record with a wrong type property.");
            }
            
            var response = _httpWorker.SendLetterContentRequest(session, letterRecord);

            LessonLetterParser letterContentParser = new();

            LessonLetter letter = letterContentParser.Parse(await response);

            return letter;
        }
        public async Task<IEnumerable<LessonLetter>> ReadLetters(Session session, IEnumerable<LetterRecord> letterRecords)
        {
            Task<LessonLetter>[] lessonLetters = letterRecords.Select((letterRecord) => ReadLetter(session, letterRecord)).ToArray();
            
            return await Task.WhenAll(lessonLetters);
        }
    }
}
