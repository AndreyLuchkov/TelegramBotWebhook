namespace TelegramBotWebhook.Extensions
{
    static class HttpRequestOptionsExtensions
    {
        static public HttpRequestOptions GetLoginOptions(this HttpRequestOptions options, string login, string password)
        {
            options.TryAdd("login", login);
            options.TryAdd("password", password);

            return options;
        } 
    }
}
