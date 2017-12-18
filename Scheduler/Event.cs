using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Calendars;
using Scheduler.Persistance;
using Scheduler.Users;

namespace Scheduler
{
    public class Event : Vertex, IEvent
    {
        private IEdgeVertex<ILocation> _location;

        [IgnoreDataMember]
        public IEdgeVertex<ILocation> Location
        {
            get => _location;
            set
            {
                _location = value;
                SetDirty();
            }
        }

        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                SetDirty();
            }
        }

        [IgnoreDataMember]
        public IEdgeVertexs<ISerial> Serials { get; set; }

        [IgnoreDataMember]
        public IEdgeVertex<IInstance> Instance { get; set; }

        protected override IEnumerable<IVertex> Links
        {
            get
            {
                var list = new List<IVertex>();

                if (Serials != null)
                    list.AddRange(Serials.Select(s => s.ToVertex));
                if (Tags != null)
                    list.AddRange(Tags.Select(t => t.ToVertex));

                if (Instance != null)
                {
                    list.Add(Instance.ToVertex);
                    list.Add(Instance.Edge);
                }
                if (Location != null)
                {
                    list.Add(Location.ToVertex);
                    list.Add(Location.Edge);
                }

                return list;
            }
        }

        public override void Validate()
        {
            Guard.AgainstNull(Location, nameof(Location));
            Guard.AgainstNull(Serials, nameof(Serials));
            //Guard.AgainstNull(Instance, nameof(Instance));

            Guard.AgainstNullOrWhiteSpace(Title, nameof(Title));
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Event>(db);
            Serials?.Save(db, clock, this);
            Tags?.Save(db, clock, this);
            Instance?.Save(db, clock, this);
            Location?.Save(db, clock, this);
            base.Save(db, clock);
        }

        public override void Rehydrate(IArangoDatabase db)
        {
            Serials = new EdgeVertexs<ISerial>(Utilities.GetEdges<Serial>(db, Id));
            Tags = new EdgeVertexs<ITag>(Utilities.GetEdges<Tag>(db, Id));
            Instance = new EdgeVertex<IInstance>(Utilities.GetEdges<Instance>(db, Id).SingleOrDefault());
            Location = new EdgeVertex<ILocation>(Utilities.GetEdges<Location>(db, Id).SingleOrDefault());

            base.Rehydrate(db);
        }

        public class Builder : Builder<Event>
        {
            public string Title
            {
                set => _target.Title = value;
            }

            public IInstance Instance
            {
                set => _target.Instance = new EdgeVertex<IInstance>(value);
            }

            public IEnumerable<ISerial> Serials
            {
                set => _target.Serials = new EdgeVertexs<ISerial>(value);
            }

            public ISerial Serial {  set => Serials = new [] { value, };}

            public ILocation Location
            {
                set => _target.Location = new EdgeVertex<ILocation>(value);
            }
        }
    }
}
