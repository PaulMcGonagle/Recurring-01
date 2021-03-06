﻿using System;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;

namespace Scheduler.Persistance
{
    public class GeneratedInstant : Vertex
    {
        public Instant Instant { get; set; }

        public string Label { get; set; }

        [IgnoreDataMember]
        public IVertex FromVertex { get; set; }

        [IgnoreDataMember]
        public IVertex ToVertex { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeFrom)]
        public string FromId { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeTo)]
        public string ToId { get; set; }

        public override void Validate()
        {
            base.Validate();

            Guard.AgainstNull(FromVertex, nameof(FromVertex));
            Guard.AgainstNull(ToVertex, nameof(ToVertex));
        }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            base.Save(db, clock);

            FromId = FromVertex.Id;

            if (!FromVertex.IsPersisted)
                throw new SaveException(SaveResult.Incomplete, GetType(),
                    $"FromVertex has not been persisted ({FromVertex})");

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
                throw NewRehydrateException(RehydrateResult.MissingId, nameof(FromId));
            }

            var fromInfo = db.FindDocumentInfo(FromId);

            if (fromInfo == null)
            {
                throw NewRehydrateException(RehydrateResult.InvalidObject, nameof(FromId));
            }

            base.Rehydrate(db);
        }

        #endregion

        public override string ToString()
        {
            return $"Edge to: {ToVertex}";
        }

        public class Builder : Builder<GeneratedInstant>
        {
            public string Label
            {
                set =>_target.Label = value;
            }

            public IVertex FromVertex
            {
                set => _target.FromVertex = value;
            }

            public IVertex ToVertex
            {
                set => _target.ToVertex = value;
            }
        }
    }
}
