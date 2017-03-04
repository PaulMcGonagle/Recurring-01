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
        public IEdgeVertexs<ISerial> Serials { get; set; }

        [IgnoreDataMember]
        public IEdgeVertexs<ITag> Tags { get; set; }

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

        public static Event Create(Schedule schedule, ITimeRange rangeTime, string timeZoneProvider, Location location = null)
        {
            return new Event
            {
                Serials = new EdgeVertexs<ISerial>(
                    toVertex: new Serial(
                        schedule: new CompositeSchedule()
                        {
                            InclusionsEdges = new EdgeVertexs<ISchedule>
                            {
                                new EdgeVertex<ISchedule>(schedule),
                            },
                        },
                        timeRange: new EdgeRangeTime(rangeTime),
                        timeZoneProvider: timeZoneProvider)),
                Location = location != null ? new EdgeVertex<Location>(location) : null,
            };
        }
    }
}
