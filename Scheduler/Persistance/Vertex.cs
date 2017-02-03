﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using ArangoDB.Client.Data;
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

        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Key { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Handle)]
        public string Id { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Revision)]
        public string Rev { get; set; }

        [IgnoreDataMember]
        public virtual bool IsDirty { get; private set; } = true;

        [IgnoreDataMember]
        public virtual bool IsPersisted => !IsNew;

        [IgnoreDataMember]
        public virtual bool IsNew => Key == null;

        [IgnoreDataMember]
        public bool ToDelete { get; private set; } = false;

        [IgnoreDataMember]
        public bool IsDeleted => !IsDirty && Key == null;

        public void SetToDelete()
        {
            ToDelete = true;
            IsDirty = true;
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

        protected void Save(IArangoDatabase db, IClock clock, IEnumerable<IVertex> vertexs)
        {
            foreach (var vertex in vertexs)
            {
                vertex.Save(db, clock);
            }
        }

        internal void Save<T>(IArangoDatabase db) where T : IVertex
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
