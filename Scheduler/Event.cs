using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
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
            Serials = new EdgeVertexs<ISerial>(Utilities.GetByFromId<Serial>(db, this.Id));
            Tags = new EdgeVertexs<ITag>(Utilities.GetByFromId<Tag>(db, this.Id));
            Instance = new EdgeVertex<IInstance>(Utilities.GetByFromId<Instance>(db, this.Id).SingleOrDefault());
            Location = new EdgeVertex<ILocation>(Utilities.GetByFromId<Location>(db, this.Id).SingleOrDefault());

            base.Rehydrate(db);
        }

        public static Event Create(Schedule schedule, IRangeTime rangeTime, string timeZoneProvider, Location location = null)
        {
            return new Event
            {
                Serials = new EdgeVertexs<ISerial>(
                    toVertex: new Serial(
                        schedule: new CompositeSchedule()
                        {
                            Inclusions = new EdgeVertexs<ISchedule>
                            {
                                new EdgeVertex<ISchedule>(schedule),
                            },
                        },
                        rangeTime: new EdgeRangeTime(rangeTime),
                        timeZoneProvider: timeZoneProvider)),
                Location = location != null ? new EdgeVertex<ILocation>(location) : null,
            };
        }
    }
}
