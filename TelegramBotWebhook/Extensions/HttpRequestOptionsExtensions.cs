using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Extensions
{
    static class HttpRequestOptionsExtensions
    {
        static public HttpRequestOptions GetLoginOptions(this HttpRequestOptions options, long userId)
        {
            options.TryAdd("userId", userId);

            return options;
        } 
        static public HttpRequestOptions GetEmailPageOptions(this HttpRequestOptions options, long userId, int pageNumber)
        {
            options.TryAdd("userId", userId);
            options.TryAdd("pageNumber", pageNumber);

            return options;
        }
        static public HttpRequestOptions GetReadLetterOptions(this HttpRequestOptions options, long userId, LetterRecord letterRecord)
        {
            options.TryAdd("userId", userId);
            options.TryAdd("letterRecord", letterRecord);

            return options;
        }

    }
}
