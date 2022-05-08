namespace TelegramBot.Web.MPEIEmail
{
    public class Session
    {
        static Session? session;
        public string? CookieKey1 { get; set; }
        public string? CookieKey2 { get; set; }
        public string? UserKey { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        
        protected Session() { }
        public static Session Instance()
        {
            if (session is null)
                session = new Session();
            return session;
        }
    }
}
