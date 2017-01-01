using System;
using System.Runtime.Serialization;
using ArangoDB.Client;

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

        internal SaveResult Save<T>(IArangoDatabase db) where T : Vertex
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            if (!IsDirty)
                return SaveResult.Success;

            if (IsPersisted
                && !ToDelete)
            {
                var existing = db.Document<T>(id: this.Id);

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

        public virtual SaveResult Save(IArangoDatabase db)
        {
            return SaveResult.Success;
        }

        int IComparable.CompareTo(object obj)
        {
            throw new NotImplementedException();
            //return String.Compare(Key, ((PersistableEntity)obj).Key, StringComparison.Ordinal);
        }
    }
}
