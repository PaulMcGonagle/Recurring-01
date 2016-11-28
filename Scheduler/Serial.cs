﻿using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace Scheduler
{
    public class Serial : ISerial
    {
        public ISchedule Schedule;

        public LocalTime? From;
        public Period Period;
        public string TimeZoneProvider;

        public IEnumerable<Episode> Episodes
        {
            get
            {
                if (Schedule == null)
                    throw new System.ArgumentException("Schedule");

                if (!From.HasValue)
                    throw new System.ArgumentException("From");

                if (Period == null)
                    throw new System.ArgumentException("Period");

                if (TimeZoneProvider == null)
                    throw new System.ArgumentException("TimeZoneProvider");

                return Schedule.Dates.Select(o => new Episode
                {
                    From = DateTimeHelper.GetZonedDateTime(o, From.Value, TimeZoneProvider),
                    Period = Period,
                });
            }
        }
    }
}
