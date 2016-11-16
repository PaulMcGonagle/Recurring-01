using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler
{
    public class CalendarEvent : ICalendarEvent
    {
        public ScheduleBase Schedule;

        public LocalTime? TimeStart;
        public Period Period;
        public string TimeZoneProvider;

        public IEnumerable<Appointment> Occurrences()
        {
            if (Schedule == null)
                throw new System.ArgumentException("Schedule");

            if (!TimeStart.HasValue)
                throw new System.ArgumentException("TimeStart");

            if (Period == null)
                throw new System.ArgumentException("Period");

            if (TimeZoneProvider == null)
                throw new System.ArgumentException("TimeZoneProvider");

            return Schedule.Occurrences().Select(o => new Appointment
            {
                From = DateTimeHelper.GetZonedDateTime(o, this.TimeStart.Value, TimeZoneProvider),
                Period = this.Period,
            });
        }
    }
}
