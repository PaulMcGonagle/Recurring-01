using NodaTime;
using Scheduler.Persistance;
using System;
using System.Collections.Generic;

namespace Scheduler.Generation
{
    public class GeneratedEvent : Vertex, IGeneratedEvent
    {
        public Instant Time { get; set; }

        private EdgeVertex<Event> Source { get; set; }

        public IEpisodes Episodes { get; set; }

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

                Episodes = new Episodes();

                var generatedDates = serial.EdgeSchedule.Schedule.Generate();

                foreach (var generatedDate in generatedDates)
                {
                    var episode = new Episode
                    {
                        SourceGeneratedDate = new EdgeVertex<IGeneratedDate>(generatedDate),
                        SourceSerial = new EdgeVertex<ISerial>(serial),
                        From = DateTimeHelper.GetZonedDateTime(generatedDate.Date.Value.At(serial.From ?? new LocalTime(0, 0)), serial.TimeZoneProvider),
                        Period = serial.Period,                                
                    };

                    Episodes.Add(episode);
                }
            }
        }
    }
}
