using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Globalization;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.HtmlParsers
{
    public class BrokedLessonLetterParser : IParser<LessonLetter>
    {
        public LessonLetter Parse(IHtmlDocument htmlDocument)
        {
            IElement? letterBody = htmlDocument.QuerySelector("div.bdy");

            var letterBuilder = LessonLetter.CreateBuilder();
            if (letterBody is null)
            {
                return letterBuilder.Build();
            }

            Task.WaitAll(SetInfoFromBody(letterBuilder, letterBody));

            return letterBuilder.Build();
        }
        private async Task SetInfoFromBody(LessonLetter.LessonLetterBuilder letterBuilder, IElement letterBody)
        {
            IElement? replyMessage = letterBody.QuerySelector("div#x_divRplyFwdMsg");

            Task? replyMessageParse = null;
            if (replyMessage is not null)
            {
                replyMessageParse = SetInfoFromRplyMsg(letterBuilder, replyMessage);
            }

            string text = letterBody.QuerySelectorAll("div").Last().Text();

            string date = String.Empty, time = String.Empty;
            Parallel.ForEach(text.Split('\n'), (str) =>
            {
                if (str.Contains("Организатор"))
                {
                    if (replyMessageParse is null)
                    {
                        letterBuilder.Teacher(
                            str.Split(':').Last()
                                .Trim(' '));
                    }
                }
                if (str.Contains("Дата"))
                {
                    date = str.Split(':').Last()
                                .Trim(' ');
                }
                if (str.Contains("https") && str.Contains(' '))
                {
                    letterBuilder.SessionLink(str);
                }
                if (str.Contains("Время"))
                {
                    time = str.Split(' ', ',')
                                .Skip(1).First()
                                .Trim(' ');
                }
                if (str.Contains("Номер сеанса"))
                {
                    letterBuilder.SessionNumber(str.Split(':').Last()
                                                    .Trim(' '));
                }
                if (str.Contains("Пароль сеанса"))
                {
                    letterBuilder.SessionPassword(str.Split(':').Last()
                                                        .Trim(' '));
                }
            });

            DateTime lessonDate;
            DateTime.TryParseExact(String.Concat(date, " ", time), "d MMMM yyyy г. H:mm", CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out lessonDate);

            letterBuilder.LessonStartDate(lessonDate);

            if (replyMessageParse is not null)
                await replyMessageParse;
        }
        private Task SetInfoFromRplyMsg(LessonLetter.LessonLetterBuilder letterBuilder, IElement replyMessage)
        {
            string text = replyMessage.Text();

            Parallel.ForEach(text.Split('\n'), (str, state) =>
            {
                if (str.Contains("Кому"))
                {
                    letterBuilder.Teacher(
                        str.Split(':').Last()
                            .Trim(' '));
                    state.Break();
                }
            });

            return Task.CompletedTask;
        }
    }
}
