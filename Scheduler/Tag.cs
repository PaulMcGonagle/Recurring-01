using System;
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

            var compareTagSubs = compareTag
                .RelatedTags
                .ToList();

            var tagSubs = RelatedTags
                .ToList();

            compare = tagSubs.Count.CompareTo(compareTagSubs.Count);

            if (compare != 0)
                return compare;

            compareTagSubs.Sort();
            tagSubs.Sort();

            for (var i = 0; i < tagSubs.Count(); i++)
            {
                compare = tagSubs[i].ToVertex.CompareTo(compareTagSubs[i].ToVertex);

                if (compare != 0)
                    return compare;
            }

            return compare;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Tag>(db);
            RelatedTags?.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
