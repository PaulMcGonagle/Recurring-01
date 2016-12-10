using System.Runtime.Serialization;

namespace Calendar
{
    public class Event
    {
        public string Key { get; set; }

        public string Title { get; set; }
        public string Location { get; set; }

        [IgnoreDataMember]
        public Scheduler.ISerial Serials { get; set; }
    }
}
