using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public interface IVertex : IComparable
    {
        [IgnoreDataMember]
        string Key { get; }

        [IgnoreDataMember]
        string Id { get; }

        [IgnoreDataMember]
        string Rev { get; }

        [IgnoreDataMember]
        bool IsDirty { get; }

        [IgnoreDataMember]
        bool IsPersisted { get; }

        [IgnoreDataMember]
        bool IsNew { get; }

        [IgnoreDataMember]
        bool ToDelete { get; }

        [IgnoreDataMember]
        bool IsDeleted { get; }

        void SetToDelete();

        IEnumerable<IVertex> GetLinks(int depth);

        #region Save

        void Save(IArangoDatabase db, IClock clock);

        #endregion
    }
}
