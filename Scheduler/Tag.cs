using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Tag : Vertex, ITag
    {
        public Tag(string ident, string value)
        {
            Ident = ident;
            Value = value;
        }

        public Tag()
        {
            
        }

        public string Ident { get; set; }
        public string Value { get; set; }

        protected string CombinedKey(Tag tag)
        {
            return $"{tag.Ident}.{tag.Value}";
        }

        [IgnoreDataMember]
        public EdgeVertexs<ITag> RelatedTags { get; set; } = new EdgeVertexs<ITag>();

        int IComparable.CompareTo(object obj)
        {
            var compareTag = (Tag) obj;

            var compare = String.Compare(CombinedKey(this), CombinedKey((Tag)obj), StringComparison.Ordinal);

            if (compare != 0)
                return compare;

            return Compare(this.RelatedTags.Select(r => r.ToVertex), RelatedTags.Select(r => r.ToVertex));
        }

        public static int Compare(IEnumerable<ITag> compareFrom, IEnumerable<ITag> compareTo)
        {
            if (!compareFrom.Any() && !compareTo.Any())
                return 0;

            int compareResult = 0;

            var listFrom = compareFrom
                .ToList();

            listFrom.Sort();

            var listTo = compareTo
                .ToList();

            listTo.Sort();

            for (var i = 0; i < listFrom.Count(); i++)
            {
                compareResult = listFrom[i].CompareTo(listFrom[i]);

                if (compareResult != 0)
                    return compareResult;
            }

            return compareResult;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Tag>(db);
            RelatedTags?.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
