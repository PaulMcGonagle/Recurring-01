using System;
using NodaTime;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.ScheduleAbstracts;

namespace Scheduler.ScheduleInstances
{
    public class ByWeekday : ScheduleAbstracts.Repeating
    {
        public IsoDayOfWeek Weekday;
        private IClock _clock;

        public ByWeekday()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        [IgnoreDataMember]
        public IClock Clock
        {
            // ToDo instantiate _clock using IOC
            get { return _clock ?? (_clock = SystemClock.Instance); }

            set
            {
                _clock = value;
            }
        }

        [IgnoreDataMember]
        public override IEnumerable<Scheduler.Date> Dates
        {
            get
            {
                if (Clock == null)
                    throw new ArgumentNullException($"Clock");

                var localDateTime = Clock.GetLocalDateTime();
                var localDate = localDateTime.Date;
                var localDay = localDate.IsoDayOfWeek;

                var offset = localDay < Weekday ? localDay - Weekday + 7 : localDay - Weekday;

                var startDay = localDate.PlusDays(-offset);

                var results = new List<Scheduler.Date>
                {
                    new Scheduler.Date(startDay)
                };

                var r = Enumerable.Range(1, CountFromDefault);
                results.AddRange(r.Select(o => new Scheduler.Date(startDay.PlusWeeks(-o))));
                var s = Enumerable.Range(1, CountToDefault);
                results.AddRange(s.Select(o => new Scheduler.Date(startDay.PlusWeeks(o))));

                results.Sort();

                return results;
            }
        }

        public override SaveResult Save(IArangoDatabase db)
        {
            return Save<ByWeekday>(db);
        }
    }
}
