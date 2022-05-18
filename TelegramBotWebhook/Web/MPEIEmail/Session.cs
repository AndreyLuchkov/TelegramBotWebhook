using System.Collections.Concurrent;

namespace TelegramBotWebhook.Web.MPEIEmail
{
    public sealed class Session
    {
        private static ConcurrentDictionary<long, Session> _sessions = new ConcurrentDictionary<long, Session>();
        public long UserId { get; }
        public string? UserKey { get; set; }
        public string? UnlogKey { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }

        private Session(long userId) 
        {
            UserId = userId;
        }

        public static Session GetInstance(long userId)
        {
            return _sessions.GetOrAdd(userId, (userId) => new Session(userId));
        }
    }
}
