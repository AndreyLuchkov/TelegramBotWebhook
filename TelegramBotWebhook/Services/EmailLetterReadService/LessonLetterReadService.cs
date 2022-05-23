using TelegramBotWebhook.HtmlParsers;
using TelegramBotWebhook.Web.MPEIEmail;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;
namespace TelegramBotWebhook.Services
{
    public class LessonLetterReadService : IEmailLetterReadService<LessonLetter>
    {
        private readonly HttpWorker _httpWorker;

        public LessonLetterReadService(HttpWorker httpWorker)
        {
            _httpWorker = httpWorker;
        }

        public async Task<LessonLetter> ReadLetter(MPEISession session, LetterRecord letterRecord)
        {
            if (letterRecord.Type != "IPM.Schedule.Meeting.Request" && !letterRecord.From.Contains("messenger@webex"))
            {
                return LessonLetter.CreateBuilder().Build();
            } 
            else
            {
                var response = _httpWorker.SendLetterContentRequest(session, letterRecord);

                IParser<LessonLetter> letterParser;
                if (letterRecord.Type != "IPM.Schedule.Meeting.Request" && letterRecord.From.Contains("messenger@webex"))
                {
                    letterParser = new BrokedLessonLetterParser();
                }
                else
                {
                    letterParser = new LessonLetterParser();
                }

                LessonLetter letter = letterParser.Parse(await response);

                return letter;
            }
        }
        public async Task<IEnumerable<LessonLetter>> ReadLetters(MPEISession session, IEnumerable<LetterRecord> letterRecords)
        {
            Task<LessonLetter>[] lessonLetters = letterRecords.Select((letterRecord) => ReadLetter(session, letterRecord)).ToArray();
            
            return await Task.WhenAll(lessonLetters);
        }
    }
}
