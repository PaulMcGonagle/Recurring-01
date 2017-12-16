using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.ScheduleEdges;

namespace Scheduler.Persistance
{
    public abstract class Vertex : IVertex
    {
        public enum SaveResult
        {
            Success,
            Conflict,
            Incomplete,
        }

        public enum RehydrateResult
        {
            MissingId,
            InvalidObject,
            VersionClash
        }

        [DocumentProperty(Identifier = IdentifierType.Key)]
        public virtual string Key { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Handle)]
        public virtual string Id { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Revision)]
        public virtual string Rev { get; set; }

        [IgnoreDataMember]
        public virtual bool IsDirty { get; private set; } = true;

        [IgnoreDataMember]
        public virtual bool IsLinksDirty
        {
            get
            {
                return (
                    IsDirty
                    || Links.Any(t => t.IsDirty)
                );
            }
        }

        [IgnoreDataMember]
        public virtual bool IsPersisted => !IsNew;

        [IgnoreDataMember]
        public virtual bool IsNew => Key == null;

        [IgnoreDataMember]
        public virtual bool ToDelete { get; private set; }

        [IgnoreDataMember]
        public virtual bool IsDeleted => !IsDirty && Key == null;

        public void SetToDelete()
        {
            ToDelete = true;

            SetDirty();
        }

        protected void SetDirty()
        {
            if (!IsDirty)
                IsDirty = true;
        }

        #region Tags

        public ITag Connect(ITag connectTag)
        {
            if (connectTag == null)
                return null;

            Tags.Add(new EdgeTag(connectTag));

            return connectTag;
        }

        public ITag Connect(string ident, string value)
        {
            return Connect(new Tag(ident: ident, value: value));
        }

        public void Connect(IEnumerable<ITag> connectTags)
        {
            Tags.AddRange(connectTags.Select(ct => new EdgeTag(ct)));
        }

        public IEdge ConnectAsEdge(IVertex connectVertex, string label)
        {
            return new Edge.Builder
            {
                FromVertex = this,
                ToVertex = connectVertex,
                Label = label,
            }.Build();
        }

        [IgnoreDataMember]
        public EdgeVertexs<ITag> Tags { get; set; } = new EdgeVertexs<ITag>();

        #endregion

        protected virtual IEnumerable<IVertex> Links => new List<IVertex>();

        public virtual void Validate()
        {
            
        }

        #region Save

        internal virtual void Save<T>(IArangoDatabase db) where T : IVertex
        {
            Guard.AgainstNull(db, nameof(db));

            if (!IsDirty)
                return;

            if (IsPersisted
                && !ToDelete)
            {
                var existing = db.Document<T>(id: Id);

                if (existing.Rev != Rev)
                {
                    throw new SaveException(SaveResult.Conflict, GetType(), $"Rev differs: {existing.Rev} vs {Rev}");
                }
            }

            if (IsNew)
            {
                db.Insert<T>(this, null, null);
            }
            else if (ToDelete)
            {
                db.Remove<T>(this, null, null, null);

                ToDelete = false;
            }
            else
            {
                db.Update<T>(this, null, null, null, null, null, null);
            }

            IsDirty = false;
        }

        public virtual void Save(IArangoDatabase db, IClock clock)
        {
            if (GetType().Name == "Backup")
            {
                return;
            }

            try
            {
                var backup = Backup.Create(clock, this);

                backup.Save(db, clock);
            }
            catch (SaveException saveException)
            {
                throw new SaveException(saveException.SaveResult, typeof(Backup), $"Unable to save backup of {Id}");
            }

            Tags.Save(db, clock, this);
        }

        protected static Exception NewSaveException(SaveResult saveResult, Type sourceType, string message)
        {
            return new Exception($"Save Exception: SaveResult={saveResult} saving type={sourceType}, message={message}");
        }

        protected static Exception NewRehydrateException(RehydrateResult rehydrateResult, string message)
        {
            return new Exception($"Rehydrate Exception: RehydrateResult={rehydrateResult} message={message}");
        }

        public virtual void Rehydrate(IArangoDatabase db)
        {
            Tags = new EdgeVertexs<ITag>(Utilities.GetEdges<Tag>(db, this.Id));

            foreach (var tag in Tags)
            {
                tag.ToVertex.Rehydrate(db);
            }
        }

        #endregion

        int IComparable.CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
        
        public abstract class Builder<T> where T : Vertex, new()
        {
            protected readonly T _target;

            protected Builder()
            {
                _target = new T();
            }

            public IEnumerable<ITag> Tags
            {
                set => _target.Tags = new EdgeVertexs<ITag>(value);
            }

            public virtual T Build()
            {
                _target.Validate();

                return _target;
            }
        }
    }
}
