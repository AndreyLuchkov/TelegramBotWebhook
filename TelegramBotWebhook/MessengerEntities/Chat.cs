namespace TelegramBotWebhook
{
    public class Chat : IChat
    {
        public long Id { get; set; }

        public Chat(long id)
        {
            Id = id;
        }
    }
}
