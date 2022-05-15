using System.Collections.Concurrent;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.HtmlParsers
{
    public class LetterRecordsPageParser : IParser<IEnumerable<LetterRecord>>
    {
        public IEnumerable<LetterRecord> Parse(IHtmlDocument htmlDocument)
        {
            IEnumerable<IElement> letterRecords = Array.Empty<IElement>();
            try
            {
                letterRecords = htmlDocument.QuerySelector("table.lvw")!
                                         .QuerySelectorAll("tr").Skip(3);
            }
            catch
            {
                return Array.Empty<LetterRecord>();
            }

            ConcurrentBag<LetterRecord> letters = new();
            Parallel.ForEach(letterRecords, (letterRecord) =>
            {
                var letterRecordBuilder = LetterRecord.CreateBuilder();

                var letterRecordValues = letterRecord.QuerySelectorAll("td").Skip(1).SkipLast(1).ToArray();
                Parallel.For(0, letterRecordValues.Length, (i) =>
                {
                    switch (i)
                    {
                        case 0:
                            string? isReadAttrValue = letterRecordValues[i]?.FirstElementChild?.Attributes.Where((attribute) => attribute.Name == "alt").First().Value;

                            letterRecordBuilder.IsRead(isReadAttrValue != "Сообщение: не прочитано");
                            break;
                        case 1:
                            var hasFilesFlagChild = letterRecordValues[i]?.Children;

                            letterRecordBuilder.HasFiles(hasFilesFlagChild is not null);
                            break;
                        case 2:
                            letterRecordBuilder.LetterKey(letterRecordValues[i]?.FirstElementChild?.Attributes.Where((attr) => attr.Name == "value").First().Value!);
                            break;
                        case 3:
                            letterRecordBuilder.From(letterRecordValues[i]?.Text() ?? "");
                            break;
                        case 4:
                            letterRecordBuilder.Theme(letterRecordValues[i]?.Text() ?? "");

                            string typeAndNumberInfoAttrValue = letterRecordValues[i].QuerySelector("a")!.Attributes.Where((attr) => attr.Name == "onclick").First().Value;

                            letterRecordBuilder.EmailNumber(int.Parse(typeAndNumberInfoAttrValue.Split(',').Skip(2).First()));
                            letterRecordBuilder.Type(typeAndNumberInfoAttrValue.Split('\'').ToArray()[1]);
                            break;
                        case 5:
                            letterRecordBuilder.ReceivingTime(DateTime.Parse(letterRecordValues[i].Text()));
                            break;
                    }
                });
                letters.Add(letterRecordBuilder.Build());
            });
            return letters;
        }
    }
}
