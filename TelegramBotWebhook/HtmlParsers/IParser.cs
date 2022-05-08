using AngleSharp.Html.Dom;

namespace TelegramBotWebhook.HtmlParser
{
    public interface IParser<T> where T: class
    {
        T Parse(IHtmlDocument htmlDocument);
    }
}
