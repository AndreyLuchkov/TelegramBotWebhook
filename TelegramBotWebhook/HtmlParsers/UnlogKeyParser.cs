using AngleSharp.Html.Dom;

namespace TelegramBotWebhook.HtmlParsers
{
    public class UnlogKeyParser : IParser<string>
    {
        public string Parse(IHtmlDocument htmlDocument)
        {
            var unlogButton = htmlDocument.QuerySelector("a#lo");

            if (unlogButton is null)
            {
                return String.Empty;
            }

            return unlogButton.Attributes
                .Where((attr) => attr.Name == "onclick").First().Value
                    .Split('\'').Skip(1).First();
        }
    }
}
