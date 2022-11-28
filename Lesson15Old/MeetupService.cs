using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;

namespace Lesson15Old
{
    public class MeetupService
    {
        private MeetupSettings _settings;

        private List<string> _idList = new List<string>();

        public MeetupService(IOptions<MeetupSettings> options)
        {
            _settings = options.Value;
        }

        public bool Add(string id)
        {
            if (_idList.Count < _settings.MaxPeople)
            {
                _idList.Add(id);
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append(_settings.IsPresentation ? "Presentation" : "Meeting")
                .Append(" with maximum of ")
                .Append(_settings.MaxPeople)
                .Append(" people and time limit - ")
                .Append(_settings.MaxMeetupLength)
                .AppendLine();
            if (_settings.IsPresentation && !_settings.IsQuestionsAllowed) b.AppendLine("The presentation is silent");
            if (_idList.Count == 0) b.AppendLine("No people present");
            else
            {
                b.AppendLine("Current people:");
                _idList.ForEach(s => b.AppendLine(s));
            }
            return b.ToString();
        }
    }
}
