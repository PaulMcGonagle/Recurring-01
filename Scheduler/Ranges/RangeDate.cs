using System;
using System.Globalization;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler.Ranges
{
    public class RangeDate : Vertex, IRangeDate
    {
        [IgnoreDataMember]
        public EdgeDate Start { get; }
        [IgnoreDataMember]
        public EdgeDate End { get; }

        public RangeDate(int startYear, YearMonth.MonthValue startMonth, int startDay, int endYear, YearMonth.MonthValue endMonth, int endDay)
        {
            Start = new EdgeDate(startYear, startMonth, startDay);
            End = new EdgeDate(endYear, endMonth, endDay);
        }

        public RangeDate(EdgeDate start, EdgeDate end)
        {
            if (start.Date.Value > end.Date.Value)
                throw new ArgumentOutOfRangeException(nameof(start), $"Start date [{end.Date.Value.ToString("D", CultureInfo.CurrentCulture)}] cannot be greater than To date [{start.Date.Value.ToString("D", CultureInfo.CurrentCulture)}]");

            Start = start;
            End = end;
        }

        public RangeDate(LocalDate start, LocalDate end)
            : this(
                start: new EdgeDate(start),
                end: new EdgeDate(end))
        {
        }

        public void Validate()
        {
            if (Start?.Date?.Value != null && End?.Date?.Value == null && Start.Date?.Value <= End?.Date?.Value)
            {
                throw new ArgumentOutOfRangeException($"Range is invalid: Start={Start?.Date?.Value}, End={End?.Date?.Value}");
            }
        }

        public override string ToString()
        {
            return $"{Start.Date}->{End.Date}";
        }

        public bool Contains(LocalDate localDate)
        {
            return Start.Date?.Value <= localDate && localDate <= End.Date?.Value;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<RangeDate>(db);
            Start?.Save(db, clock, this, "HasStartDate");
            End?.Save(db, clock, this, "HasEndDate");
            base.Save(db, clock);
        }
    }
}
