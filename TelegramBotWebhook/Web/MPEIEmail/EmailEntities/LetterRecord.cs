namespace TelegramBotWebhook.MPEIEmail.EmailEntities
{
    public class LetterRecord
    {
        private int _emailNumber;
        private bool _isRead;
        private bool _hasFiles;
        private string _from = String.Empty;
        private string _theme = String.Empty;
        private DateTime _receivingTime;
        private string _letterKey = String.Empty;
        private string _type = String.Empty;

        public int EmailNumber { get => _emailNumber; }
        public bool IsRead { get => _isRead; }
        public bool HasFiles { get => _hasFiles; }
        public string From { get => _from; }
        public string Theme { get => _theme; }
        public DateTime ReceivingTime { get => _receivingTime; }
        public string LetterKey { get => _letterKey; }
        public string Type { get => _type; }
        
        private LetterRecord() { }

        public static LetterRecordBuilder CreateBuilder() => new LetterRecordBuilder(new LetterRecord());
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

        public class LetterRecordBuilder
        {
            private readonly LetterRecord _letterRecord;

            internal LetterRecordBuilder(LetterRecord letterRecord)
            {
                _letterRecord = letterRecord;
            } 

            public LetterRecordBuilder EmailNumber(int emailNumber)
            {
                _letterRecord._emailNumber = emailNumber;
                return this;
            }
            public LetterRecordBuilder IsRead(bool isRead)
            {
                _letterRecord._isRead = isRead;
                return this;
            }
            public LetterRecordBuilder HasFiles(bool hasFiles)
            {
                _letterRecord._hasFiles = hasFiles;
                return this;
            }
            public LetterRecordBuilder From(string from)
            {
                _letterRecord._from = from;
                return this;
            }
            public LetterRecordBuilder Theme(string theme)
            {
                _letterRecord._theme = theme;
                return this;
            }
            public LetterRecordBuilder ReceivingTime(DateTime receivingTime)
            {
                _letterRecord._receivingTime = receivingTime;
                return this;
            }
            public LetterRecordBuilder LetterKey(string letterKey)
            {
                _letterRecord._letterKey = letterKey;
                return this;
            }
            public LetterRecordBuilder Type(string type)
            {
                _letterRecord._type = type;
                return this;
            }
            public LetterRecord Build() => _letterRecord;
        }
    }
}
