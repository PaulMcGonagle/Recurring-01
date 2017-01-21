using System;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;
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

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<Event>(db),
                () => Serials.Save(db, clock, this),
                () => Location?.Save(db, clock, this) ?? SaveDummy(),
                () => base.Save(db, clock),
            });
        }

        public static Event Create(Schedule schedule, TimeRange timerange, string timeZoneProvider, Location location = null)
        {
            var serial = new Serial(
                schedule: new CompositeSchedule()
                {
                    InclusionsEdges = new EdgeVertexs<ISchedule>
                    {
                        new EdgeVertex<ISchedule>(schedule),
                    },
                },
                timeRange: timerange,
                timeZoneProvider: timeZoneProvider);

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
                        timeRange: timerange,
                        timeZoneProvider: timeZoneProvider)),
                Location = location != null ? new EdgeVertex<Location>(location) : null,
            };
        }
    }
}
