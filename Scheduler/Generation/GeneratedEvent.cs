using NodaTime;
using Scheduler.Persistance;
using System;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;

namespace Scheduler.Generation
{
    public class GeneratedEvent : Vertex, IGeneratedEvent
    {
        public Instant Time { get; set; }

        [IgnoreDataMember]
        private ILink<IEvent> Source { get; set; }

        [IgnoreDataMember]
        public IEdgeVertexs<IEpisode> Episodes { get; set; }

        public static void Generate(IClock clock, IEvent source)
        {
            if (source.IsDirty)
                throw new ArgumentException("Event has not yet been persisted");

            source.GeneratedEvent = new EdgeVertex<IGeneratedEvent>(new GeneratedEvent
            {
                Time = clock.Now,
                Source = new Link<IEvent>(source),
                Episodes = new EdgeVertexs<IEpisode>(
                    source.Serials.SelectMany(s => s.ToVertex.Episodes)
                    ),
            });
        }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<GeneratedEvent>(db);
            // ToDo what about Source.Save?
            //() => Source.Save(db, clock, this);
            base.Save(db, clock);
        }

        #endregion
    }
}
