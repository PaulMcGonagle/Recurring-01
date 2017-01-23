using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;

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

        [IgnoreDataMember]
        public string Key { get; protected set; }

        [IgnoreDataMember]
        public string Id { get; protected set; }

        [IgnoreDataMember]
        public string Rev { get; protected set; }

        [IgnoreDataMember]
        public virtual bool IsDirty { get; private set; } = true;

        [IgnoreDataMember]
        public virtual bool IsPersisted => Key != null;

        [IgnoreDataMember]
        public virtual bool IsNew => !IsPersisted;

        [IgnoreDataMember]
        public bool ToDelete { get; private set; } = false;

        [IgnoreDataMember]
        public bool IsDeleted => !IsDirty && Key == null;

        public void SetToDelete()
        {
            ToDelete = true;
        }

        protected void SetDirty()
        {
            if (!IsDirty)
                IsDirty = true;
        }

        public IEnumerable<IVertex> GetLinks(int depth)
        {
            var links = Links;

            if (depth > 0)
            {
                var depthLinks = links.SelectMany(l => l.GetLinks(depth - 1));

                links = links.Union(depthLinks);
            }

            return links;
        }

        protected virtual IEnumerable<IVertex> Links => new List<IVertex>();

        #region Save

        protected void Save(IArangoDatabase db, IClock clock, IEnumerable<Vertex> vertexs)
        {
            foreach (var vertex in vertexs)
            {
                vertex.Save(db, clock);
            }
        }

        internal void Save<T>(IArangoDatabase db) where T : Vertex
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            if (!IsDirty)
                return;

            if (IsPersisted
                && !ToDelete)
            {
                var existing = db.Document<T>(id: Id);

                if (existing.Rev != Rev)
                {
                    throw new SaveException(SaveResult.Conflict, this.GetType(), $"Rev differs: {existing.Rev} vs {Rev}");
                }
            }

            var result = IsNew ? db.Insert<T>(this) : ToDelete ? db.Remove<T>(this) : db.Update<T>(this);

            Key = result.Key;
            Id = result.Id;
            Rev = result.Rev;

            IsDirty = false;

            return;
        }

        public virtual void Save(IArangoDatabase db, IClock clock)
        {
            if (GetType().Name == "Backup")
            {
                return;
            }

            Backup backup;

            try
            {
                backup = Backup.Create(clock, this);

                backup.Save(db, clock);
            }
            catch (SaveException saveException)
            {
                throw new SaveException(saveException.SaveResult, typeof(Backup), $"Unable to save backup of {Id}");
            }
        }

        protected static Exception NewSaveException(SaveResult saveResult, System.Type sourceType, string message)
        {
            return new Exception($"Save Exception: SaveResult={saveResult} saving type={sourceType}, message={message}");
        }

        #endregion

        int IComparable.CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
