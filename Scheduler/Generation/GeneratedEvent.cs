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

        public void Generate(IClock clock, IEvent source)
        {
            Time = clock.Now;

            if (source.IsDirty)
                throw new ArgumentException("Event has not yet been persisted");

            Source = new EdgeVertex<IEvent>(source);

            Episodes = new Episodes();

            Episodes.AddRange(source.Serials.SelectMany(s => s.Episodes));
        }
    }
}
