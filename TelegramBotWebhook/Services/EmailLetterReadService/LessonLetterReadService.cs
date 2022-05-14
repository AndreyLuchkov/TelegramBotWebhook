using AngleSharp.Html.Dom;
using TelegramBotWebhook.MPEIEmail.EmailEntities;
using TelegramBotWebhook.Web.HttpFactories;
using TelegramBotWebhook.Extensions;
using TelegramBotWebhook.HtmlParsers;
using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Services
{
    public class LessonLetterReadService : IEmailLetterReadService<LessonLetter>
    {
        private readonly IHttpFactory _httpFactory;

        public LessonLetterReadService(IEnumerable<IHttpFactory> httpFactories)
        {
            _httpFactory = httpFactories.Where((factory) => factory is LetterContentHttpFactory).First();
        }

        public async Task<LessonLetter> ReadLetter(Session session, LetterRecord letterRecord)
        {
            if (letterRecord.Type != "IPM.Schedule.Meeting.Request")
            {
                throw new ArgumentException("The letter record with a wrong type property.");
            }
            
            var response = GetResponseFromEmail(session, letterRecord);

            LessonLetterParser letterContentParser = new();

            LessonLetter letter = letterContentParser.Parse(await response);

            return letter;
        }
        public async Task<IEnumerable<LessonLetter>> ReadLetters(Session session, IEnumerable<LetterRecord> letterRecords)
        {
            var lessonLetters = letterRecords.Select((letterRecord) => ReadLetter(session, letterRecord)).ToArray();
            
            return await Task.WhenAll(lessonLetters);
        }
        private async Task<IHtmlDocument> GetResponseFromEmail(Session session, LetterRecord letterRecord)
        {
            var request = _httpFactory.GetRequest();

            HttpRequestOptions options = new();
            options.GetReadLetterOptions(session.UserId, letterRecord);

            var response = request.Send(options);

            var responseHandler = _httpFactory.GetResponseHandler();

            return await responseHandler.HandleResponse(await response);
        }
    }
}
