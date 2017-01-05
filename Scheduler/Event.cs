using System;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.Users;

namespace Scheduler
{
    public class Event : Vertex
    {
        public Event(ISerials serials)
        {
            Serials = serials;
        }
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
        public ISerials Serials { get; set; }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<Event>(db),
                () => Save(db, clock, Serials.Select(serial => (Vertex)serial)),
                () => Location?.Save(db, clock, this) ?? SaveDummy(),
                () => base.Save(db, clock),
            });
        }
    }
}
