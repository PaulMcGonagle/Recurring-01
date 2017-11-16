using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public class GeneratedInstant : Vertex
    {
        public Instant Instant { get; private set; }

        public string Label { get; set; }

        [IgnoreDataMember]
        public IVertex FromVertex { get; set; }

        [IgnoreDataMember]
        public IVertex ToVertex { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeFrom)]
        public string FromId { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeTo)]
        public string ToId { get; set; }

        internal GeneratedInstant(IClock clock, IVertex fromVertex, IVertex toVertex)
        {
            Instant = clock.Now;
            FromVertex = fromVertex;
            ToVertex = toVertex;
        }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            base.Save(db, clock);

            FromId = FromVertex.Id;

            if (!FromVertex.IsPersisted)
                throw new SaveException(SaveResult.Incomplete, GetType(), $"FromVertex has not been persisted ({FromVertex})");

            ToVertex.Save(db, clock);

            ToId = ToVertex.Id;

            Save<GeneratedInstant>(db);

        }

        public virtual void Save(IArangoDatabase db, IClock clock, IVertex fromVertex)
        {
            FromVertex = fromVertex;

            Save(db, clock);
        }

        public override void Rehydrate(IArangoDatabase db)
        {
            if (string.IsNullOrWhiteSpace(FromId))
            {
                throw Vertex.NewRehydrateException(RehydrateResult.MissingId, nameof(FromId));
            }

            var fromInfo = db.FindDocumentInfo(FromId);

            if (fromInfo == null)
            {
                throw Vertex.NewRehydrateException(RehydrateResult.InvalidObject, nameof(FromId));
            }

            var type = fromInfo.Document.Type;

            base.Rehydrate(db);
        }

        #endregion

        public override string ToString()
        {
            return $"Edge to: {ToVertex}";
        }
    }

    public class GeneratedInstantBuilder
    {
        private GeneratedInstant _generatedInstant;

        public GeneratedInstantBuilder Create(IClock clock, IVertex fromVertex, IVertex toVertex)
        {
            _generatedInstant = new GeneratedInstant(clock, fromVertex, toVertex);

            return this;
        }

        public GeneratedInstantBuilder WithLabel(string label)
        {
            _generatedInstant.Label = label;

            return this;
        }

        public GeneratedInstantBuilder WithTags(string label)
        {
            _generatedInstant.Label = label;

            return this;
        }

        public GeneratedInstant Build()
        {
            if (_generatedInstant.FromVertex == null)
                throw new Exception("Missing FromVertex");


            if (_generatedInstant.ToVertex == null)
                throw new Exception("Missing ToVertex");

            return _generatedInstant;
        }
    }
}
