﻿using System.Net;
using System.Net.Http.Headers;
using AngleSharp.Html.Dom;
using TelegramBotWebhook.Web;
using TelegramBotWebhook.Web.Decompressor;

namespace TelegramBot.Web.MPEIEmail.ResponseHandlers
{
    internal class LoginHttpResponseHandler : IHttpResponseHandler
    {
        public async Task<IHtmlDocument> HandleResponse(HttpResponseMessage response)
        {
            if (!(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Redirect))
                throw new Exception($"The unexpected response status code: {response.StatusCode}");

            long userId = GetUserIdFromHeaders(response.Headers);
            if (Session.GetInstance(userId).UserKey is null)
            {
                string setCookieHeaderValue = GetSetCookieHeaderValue(response.Headers);

                Session.GetInstance(userId).UserKey = setCookieHeaderValue;
            }

            var decompressor = new GzipToHtmlDecompressor();
            return await decompressor.DecompressToHtmlDoc(await response.Content.ReadAsStreamAsync());
        }
        private string GetSetCookieHeaderValue(HttpResponseHeaders headers)
        {
            IEnumerable<string>? setCookieValues;
            if (headers.TryGetValues("Set-Cookie", out setCookieValues))
                return setCookieValues.First().Split(';').First();
            else
                throw new NullReferenceException("The response headers do not have a set-cookie header.");
        }
        private long GetUserIdFromHeaders(HttpResponseHeaders headers)
        {
            IEnumerable<string>? userId;
            if (headers.TryGetValues("userId", out userId))
                return long.Parse(userId.First());
            else
                throw new NullReferenceException("The response headers do not have a userId header.");
        }
    }
}