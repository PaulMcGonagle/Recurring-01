using NodaTime;
using Scheduler.Persistance;
using System;
using System.Collections.Generic;

namespace Scheduler.Generation
{
    public class GeneratedEvent : Vertex
    {
        public Instant Time { get; set; }

        EdgeVertex<Event> Source { get; set; }

        public IList<Episode> Episodes { get; set; }

        public void Generate(IClock clock, Event source)
        {
            Time = clock.Now;

            if (source.IsDirty)
                throw new ArgumentException("Event has not yet been persisted");

            Source = new EdgeVertex<Event>(source);

            foreach (var serial in source.Serials)
            {
                if (serial.EdgeSchedule?.Schedule == null)
                    throw new ArgumentException("Schedule");

                if (!serial.From.HasValue)
                    throw new ArgumentException("From");

                if (serial.Period == null)
                    throw new ArgumentException("Period");

                if (serial.TimeZoneProvider == null)
                    throw new ArgumentException("TimeZoneProvider");

                Episodes = new List<Episode>();

                foreach (var date in serial.EdgeSchedule.Schedule.GenerateDates())
                {
                    Episode episode = new Episode
                    {
                        From = DateTimeHelper.GetZonedDateTime(date.Date.Value.At(serial.From ?? new LocalTime(0, 0)), serial.TimeZoneProvider),
                        Period = serial.Period,                                
                    };

                    Episodes.Add(episode);
                }
            }
        }
    }
}
