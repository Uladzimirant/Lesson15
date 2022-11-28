using System;

namespace Lesson15Old
{
    public class MeetupSettings
    {
        public int MaxPeople { get; set; }
        public TimeSpan MaxMeetupLength { get; set; }
        public bool IsPresentation { get; set; }
        public bool IsQuestionsAllowed { get; set; }
    }
}
