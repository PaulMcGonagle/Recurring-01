using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public abstract class Vertex : IComparable
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

        #region Save

        protected static SaveResult SaveDummy()
        {
            return SaveResult.Success;
        }

        protected SaveResult Save(IEnumerable<Func<SaveResult>> saveFuncs)
        {
            foreach (var saveFunc in saveFuncs)
            {
                var result = saveFunc.Invoke();

                if (result != SaveResult.Success)
                    return result;
            }

            return SaveResult.Success;
        }

        protected SaveResult Save(IArangoDatabase db, IClock clock, IEnumerable<Vertex> vertexs)
        {
            foreach (var vertex in vertexs)
            {
                var result = vertex.Save(db, clock);

                if (result != SaveResult.Success)
                    return result;
            }

            return SaveResult.Success;
        }

        internal SaveResult Save<T>(IArangoDatabase db) where T : Vertex
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            if (!IsDirty)
                return SaveResult.Success;

            if (IsPersisted
                && !ToDelete)
            {
                var existing = db.Document<T>(id: Id);

                if (existing.Rev != Rev)
                {
                    return SaveResult.Conflict;
                }
            }

            var result = IsNew ? db.Insert<T>(this) : ToDelete ? db.Remove<T>(this) : db.Update<T>(this);

            Key = result.Key;
            Id = result.Id;
            Rev = result.Rev;

            IsDirty = false;

            return SaveResult.Success;
        }

        public virtual SaveResult Save(IArangoDatabase db, IClock clock)
        {
            if (GetType().Name == "Backup")
            {
                return SaveResult.Success;
            }

            var backup = Backup.Create(clock, this);

            var result = backup.Save(db, clock);

            return result;
        }

        #endregion

        int IComparable.CompareTo(object obj)
        {
            throw new NotImplementedException();
            //return String.Compare(Key, ((PersistableEntity)obj).Key, StringComparison.Ordinal);
        }
    }
}
