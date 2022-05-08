namespace TelegramBotWebhook.MPEIEmail.EmailEntities
{
    public class Letter
    {
        public int? LocalNumber { get; set; }
        public int EmailNumber { get; init; }
        public bool IsRead { get; init; }
        public bool HasFiles { get; init; }
        public string From { get; init; } = String.Empty;
        public string Theme { get; init; } = String.Empty;
        public DateTime ReceivingTime { get; init; }
        public string LetterKey { get; init; } = String.Empty;
        public string Type { get; init; } = String.Empty;

        static public string FromTrimDots(string from)
        {
            if (!from.Contains("..."))
            {
                return from;
            }
            var splitedFrom = from.Split(' ');
            if (splitedFrom.Length > 1)
            {
                string newFrom = $"{splitedFrom.First()} ";
                foreach (var item in splitedFrom.Skip(1).Where((str) => !(str.Length == 4 && str.Contains("..."))))
                {
                    newFrom += item.First() + ".";
                }
                return newFrom;
            }
            else
            {
                return from;
            }
        }
    }
}
