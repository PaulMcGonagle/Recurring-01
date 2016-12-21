using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Event : Vertex
    {
        private string _location;

        public string Location
        {
            get { return _location; }
            set
            {
                _location = value;
                SetDirty();
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                SetDirty();
            }
        }

        [IgnoreDataMember]
        public Scheduler.ISerial Serials { get; set; }

        public SaveResult Save(IArangoDatabase db)
        {
            return Save<Event>(db);
        }
    }
}
