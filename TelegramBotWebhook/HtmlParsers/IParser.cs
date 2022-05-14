using AngleSharp.Html.Dom;

namespace TelegramBotWebhook.HtmlParsers
{
    public interface IParser<T> where T: class
    {
        T Parse(IHtmlDocument htmlDocument);
    }
}
