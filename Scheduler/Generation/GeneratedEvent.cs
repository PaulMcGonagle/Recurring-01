using NodaTime;
using Scheduler.Persistance;
using System;
using System.Linq;

namespace Scheduler.Generation
{
    public class GeneratedEvent : Vertex, IGeneratedEvent
    {
        public Instant Time { get; set; }

        private EdgeVertex<IEvent> Source { get; set; }

        public IEpisodes Episodes { get; set; }

        public static GeneratedEvent Generate(IClock clock, IEvent source)
        {
            if (source.IsDirty)
                throw new ArgumentException("Event has not yet been persisted");

            return new GeneratedEvent
            {
                Time = clock.Now,
                Source = new EdgeVertex<IEvent>(source),
                Episodes = new Episodes(
                    episodes: source.Serials.SelectMany(s => s.ToVertex.Episodes)),
            };
        }
    }
}
