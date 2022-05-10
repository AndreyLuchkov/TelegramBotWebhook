using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using TelegramBotWebhook.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.HtmlParser
{
    public class EmailLettersParser : IParser<List<Letter>>
    {
        public List<Letter> Parse(IHtmlDocument htmlDocument)
        {
            var letterRows = htmlDocument.QuerySelectorAll("table")
                                         .Where((table) => table.ClassName == "lvw").First()
                                         .QuerySelectorAll("tr").Skip(3);

            List<Letter> letters = new List<Letter>(20);
            int letterNumber = 0;
            foreach (var htmlLetter in letterRows)
            {
                bool isRead = false, hasFiles = false;
                DateTime receivingTime = DateTime.MinValue;
                string from = String.Empty, theme = String.Empty, letterKey = String.Empty, type = String.Empty;

                var letterHeader = htmlLetter.QuerySelectorAll("td").Skip(1).SkipLast(1).ToArray();
                for (int i = 0; i < letterHeader.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            string? isReadAttrValue = letterHeader[i]?.FirstElementChild?.Attributes.Where((attribute) => attribute.Name == "alt").First().Value;

                            isRead = !(isReadAttrValue == "Сообщение: не прочитано");
                            break;
                        case 1:
                            var hasFilesFlagChild = letterHeader[i]?.Children;

                            hasFiles = hasFilesFlagChild is not null;
                            break;
                        case 2:
                            letterKey = letterHeader[i]?.FirstElementChild?.Attributes.Where((attr) => attr.Name == "value").First().Value!;
                            break;
                        case 3:
                            from = letterHeader[i]?.Text() ?? "";
                            break;
                        case 4:
                            theme = letterHeader[i]?.Text() ?? "";

                            type = letterHeader[i].QuerySelector("a")!.Attributes.Where((attr) => attr.Name == "onclick").First().Value.Split('\'').ToArray()[1];
                            break;
                        case 5:
                            receivingTime = DateTime.Parse(letterHeader[i].Text());
                            break;
                    }
                }
                Letter letter = new Letter()
                {
                    EmailNumber = letterNumber++,
                    IsRead = isRead,
                    HasFiles = hasFiles,
                    From = from,
                    Theme = theme,
                    ReceivingTime = receivingTime,
                    LetterKey = letterKey,
                    Type = type
                };
                letters.Add(letter);
            }
            return letters;
        }
    }
}
