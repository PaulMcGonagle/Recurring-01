using System;
using ArangoDB.Client;

namespace Scheduler
{
    public abstract class PersitableEntity
    {
        public string Key { get; protected set; }
        public string Id { get; protected set; }
        public string Rev { get; protected set; }

        protected void Save<T>(IArangoDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var result = Key == null ? db.Insert<T>(this) : db.Update<T>(this);

            Key = result.Key;
            Id = result.Id;
            Rev = result.Rev;
        }
    }
}
