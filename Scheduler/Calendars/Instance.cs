using System;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Calendars
{
    public class Instance : Vertex, IInstance
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

            source.Instance = new EdgeVertex<IInstance>(new Instance
            {
                Time = clock.Now,
                Source = new Link<IEvent>(source),
                Episodes = new EdgeVertexs<IEpisode>(source
                    .Serials
                    .SelectMany(s => s
                        .ToVertex
                        .GenerateEpisodes(clock)))
            });
        }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Instance>(db);
            // ToDo what about Source.Save?
            //() => Source.Save(db, clock, this);
            base.Save(db, clock);
        }

        #endregion
    }
}
