namespace TelegramBotWebhook.Web.MPEIEmail.EmailEntities
{
    public class LessonLetter 
    {
        private string _teacher = String.Empty;
        private DateTime _lessonStartDate;
        private string _sessionLink = String.Empty;
        private int _sessionNumber;
        private string _sessionPassword = String.Empty;

        public string Teacher { get => _teacher; }
        public DateTime LessonStartDate { get => _lessonStartDate; }
        public string SessionLink { get => _sessionLink; }
        public int SessionNumber { get => _sessionNumber; }
        public string SessionPassword { get => _sessionPassword; }

        private LessonLetter() { }

        static public LessonLetterBuilder CreateBuilder()
        {
            return new LessonLetterBuilder(new LessonLetter());
        }

        public class LessonLetterBuilder
        {
            private LessonLetter _lessonLetter;

            internal LessonLetterBuilder(LessonLetter letterRecord)
            {
                _lessonLetter = letterRecord;
            }

            public LessonLetterBuilder Teacher(string teacher)
            {
                _lessonLetter._teacher = teacher;
                return this;
            }
            public LessonLetterBuilder LessonStartDate(DateTime date) 
            {
                _lessonLetter._lessonStartDate = date;
                return this;
            }
            public LessonLetterBuilder SessionLink(string link)
            {
                _lessonLetter._sessionLink = link;
                return this;
            }
            public LessonLetterBuilder SessionNumber(int sessionNumber)
            {
                _lessonLetter._sessionNumber = sessionNumber;
                return this;
            }
            public LessonLetterBuilder SessionPassword(string sessionPassword) 
            {
                _lessonLetter._sessionPassword = sessionPassword;
                return this;
            }
            public LessonLetter Build()
            {
                return _lessonLetter;
            }
        }
    }
}
