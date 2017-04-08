using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler.Ranges
{
    public class DateRange : Vertex, IDateRange
    {
        [IgnoreDataMember]
        public EdgeDate From { get; }
        [IgnoreDataMember]
        public EdgeDate To { get; }

        public DateRange(int fromYear, YearMonth.MonthValue fromMonth, int fromDay, int toYear, YearMonth.MonthValue toMonth,
            int toDay)
        {
            From = new EdgeDate(fromYear, fromMonth, fromDay);
            To = new EdgeDate(toYear, toMonth, toDay);
        }

        public DateRange(EdgeDate from, EdgeDate to)
        {
            if (from.Date.Value > to.Date.Value)
                throw new ArgumentOutOfRangeException(nameof(from), $"From date [{to.Date.Value.ToString("D", CultureInfo.CurrentCulture)}] cannot be greater than To date [{from.Date.Value.ToString("D", CultureInfo.CurrentCulture)}]");

            From = from;
            To = to;
        }

        public DateRange(LocalDate from, LocalDate to)
            :this(
                from: new EdgeDate(from),
                to: new EdgeDate(to))
        {
        }

        public void Validate()
        {
            if (From?.Date?.Value != null && To?.Date?.Value == null && From.Date?.Value <= To.Date?.Value)
            {
                throw new ArgumentOutOfRangeException($"Range is invalid: From={From?.Date?.Value}, To={To?.Date?.Value}");
            }
        }

        public override string ToString()
        {
            return $"{From.Date}->{To.Date}";
        }

        public bool Contains(LocalDate localDate)
        {
            return From.Date?.Value <= localDate && localDate <= To.Date?.Value;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<DateRange>(db);
            From?.Save(db, clock, this);
            To?.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
