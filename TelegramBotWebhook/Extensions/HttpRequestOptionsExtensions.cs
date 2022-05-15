﻿using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Extensions
{
    static class HttpRequestOptionsExtensions
    {
        static public HttpRequestOptions GetLoginOptions(this HttpRequestOptions options, Session session)
        {
            options.TryAdd("session", session);

            return options;
        } 
        static public HttpRequestOptions GetEmailPageOptions(this HttpRequestOptions options, Session session, int pageNumber)
        {
            options.TryAdd("session", session);
            options.TryAdd("pageNumber", pageNumber);

            return options;
        }
        static public HttpRequestOptions GetReadLetterOptions(this HttpRequestOptions options, Session session, LetterRecord letterRecord)
        {
            options.TryAdd("session", session);
            options.TryAdd("letterRecord", letterRecord);

            return options;
        }

    }
}
