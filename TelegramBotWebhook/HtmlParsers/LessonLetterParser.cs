using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Globalization;
using TelegramBotWebhook.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.HtmlParsers
{
    public class LessonLetterParser : IParser<LessonLetter>
    {
        public LessonLetter Parse(IHtmlDocument htmlDocument)
        {
            IElement? letterHead = htmlDocument.QuerySelector("table.msgHd");

            IElement? letterBody = htmlDocument.QuerySelector("td.bdy");

            var letterBuilder = LessonLetter.CreateBuilder();
            if (letterBody is null || letterHead is null)
            {
                return letterBuilder.Build();
            }

            letterBuilder = SetInfoFromHead(letterHead, letterBuilder);

            return letterBuilder.Build();
        }
        private LessonLetter.LessonLetterBuilder SetInfoFromHead(IElement letterHead, LessonLetter.LessonLetterBuilder letterBuilder)
        {
            string teacher = letterHead.QuerySelectorAll("span")
                .Where((span) => span.ClassName == "rwRRO" && span.HasChildNodes)
                .Select((span) => span.Text()).First();

            var infoElements = letterHead.QuerySelectorAll("td.hdmtxt")
                .Skip(1).SkipLast(1);

            string strLessonDate = infoElements.First()
                .Text()
                .Split('-').First();
            DateTime lessonDate = DateTime.ParseExact(strLessonDate, "d MMMM yyyy г. H:mm", CultureInfo.GetCultureInfo("ru-RU"));

            string sessionLink = infoElements.Last().Text();

            return letterBuilder.Teacher(teacher)
                                .LessonStartDate(lessonDate)
                                .SessionLink(sessionLink);
        }
        //private LessonLetter.LessonLetterBuilder SetInfoFromBody(IElement letterBody, LessonLetter.LessonLetterBuilder letterBuilder)
        //{

        //}
    }
}
