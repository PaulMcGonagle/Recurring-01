using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Persistance;
using Scheduler.Users;

namespace Scheduler
{
    public class Event : Vertex
    {
        private EdgeVertex<Location> _location;

        [IgnoreDataMember]
        public EdgeVertex<Location> Location
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
        public Scheduler.Serials Serials { get; set; }

        public override SaveResult Save(IArangoDatabase db)
        {
            SaveResult result = Save<Event>(db);

            if (result != SaveResult.Success)
                return result;


            foreach (var serial in Serials)
            {
                result = serial.Save(db);

                if (result != SaveResult.Success)
                    return result;
            }

            if (Location != null)
                result = Location.Save(db, this);

            if (result != SaveResult.Success)
                return result;

            return SaveResult.Success;
        }
    }
}
