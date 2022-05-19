using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Globalization;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.HtmlParsers
{
    public class LessonLetterParser : IParser<LessonLetter>
    {
        public LessonLetter Parse(IHtmlDocument htmlDocument)
        {
            IElement? letterHead = htmlDocument.QuerySelector("table.msgHd");

            var letterBuilder = LessonLetter.CreateBuilder();
            if (letterHead is null)
            {
                return letterBuilder.Build();
            }

            Task.WaitAll(SetInfoFromHead(letterBuilder, letterHead));

            return letterBuilder.Build();
        }
        private async Task SetInfoFromHead(LessonLetter.LessonLetterBuilder letterBuilder, IElement letterHead)
        {
            var teacher = Task.Run(() => letterHead.QuerySelectorAll("span").AsParallel()
                .Where((span) => span.ClassName == "rwRRO" && span.HasChildNodes)
                .Select((span) => span.Text()).First());

            string strLessonDate = string.Empty, sessionLink = string.Empty;
            Parallel.ForEach(letterHead.QuerySelectorAll("td.hdmtxt"), (elem) =>
            {
                string text = elem.Text();

                if (text.Contains("https"))
                {
                    sessionLink = text;
                }
                else if (text.Contains("г."))
                {
                    strLessonDate = text.Split('-').First();
                }
            });

            DateTime lessonDate;
            DateTime.TryParseExact(strLessonDate, "d MMMM yyyy г. H:mm", CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out lessonDate);

            letterBuilder.Teacher(await teacher)
                .LessonStartDate(lessonDate)
                .SessionLink(sessionLink);
        }
    }
}
