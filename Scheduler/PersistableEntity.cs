using System;
using System.Runtime.Serialization;
using ArangoDB.Client;

namespace Scheduler
{
    public abstract class PersistableEntity
    {
        [IgnoreDataMember]
        public string Key { get; protected set; }

        [IgnoreDataMember]
        public string Id { get; protected set; }

        [IgnoreDataMember]
        public string Rev { get; protected set; }

        [IgnoreDataMember]
        public virtual bool IsDirty { get; private set; } = true;

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
        
        protected void Save<T>(IArangoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var result = Key == null ? db.Insert<T>(this) : ToDelete ? db.Remove<T>(this) : db.Update<T>(this);

            Key = result.Key;
            Id = result.Id;
            Rev = result.Rev;

            IsDirty = false;
        }
    }
}
