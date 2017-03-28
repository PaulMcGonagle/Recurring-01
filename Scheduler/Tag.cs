using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ArangoDB.Client;
using Newtonsoft.Json;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler
{
    public class Tag : Vertex, ITag
    {
        public Tag(string ident, string value)
        {
            Ident = ident;
            Value = value;
        }

        public Tag(string ident, string value, string payload)
            : this(ident, value)
        {
            Payload = payload;
        }

        public Tag()
        {
            
        }

        public string Ident { get; set; }
        public string Value { get; set; }
        public string Payload { get; set; }

        protected string CombinedKey(Tag tag)
        {
            return $"{tag.Ident}.{tag.Value}";
        }

        public ITag Connect(ITag connectTag)
        {
            if (connectTag == null)
                return null;

            RelatedTags.Add(new EdgeTag(connectTag));

            return connectTag;
        }

        public ITag Connect(string ident, string value)
        {
            return Connect(new Tag(ident: ident, value: value));
        }

        public void Connect(IEnumerable<ITag> connectTags)
        {
            RelatedTags.AddRange(connectTags.Select(ct => new EdgeTag(ct)));
        }

        [IgnoreDataMember]
        public EdgeVertexs<ITag> RelatedTags { get; set; } = new EdgeVertexs<ITag>();

        int IComparable.CompareTo(object obj)
        {
            var compareTag = (Tag) obj;

            var compare = String.Compare(CombinedKey(this), CombinedKey((Tag)obj), StringComparison.Ordinal);

            if (compare != 0)
                return compare;

            compare = Compare(RelatedTags.Select(r => r.ToVertex), RelatedTags.Select(r => r.ToVertex));

            if (compare != 0)
                return compare;

            //todo Introduce Payload Json comparison
            compare = string.Compare(Payload, compareTag.Payload, StringComparison.Ordinal);

            return compare;
        }

        protected override IEnumerable<IVertex> Links
        {
            get
            {
                var list = new List<IVertex>();

                list.AddRange(RelatedTags.Select(rt => rt.ToVertex));

                return list;
            }
        }

        public static int Compare(IEnumerable<ITag> compareFrom, IEnumerable<ITag> compareTo)
        {
            var compareFromList = compareFrom
                .ToList();
            var compareToList = compareTo
                .ToList();

            if (!compareFromList.Any() && !compareToList.Any())
                return 0;

            var compareResult = 0;

            compareFromList.Sort();
            compareToList.Sort();

            for (var i = 0; i < compareFromList.Count(); i++)
            {
                compareResult = compareFromList[i].CompareTo(compareToList[i]);

                if (compareResult != 0)
                    return compareResult;
            }

            return compareResult;
        }

        public override string ToString()
        {
            var s = new StringBuilder($"Tag ident: {Ident}, value: \"{Value}\"");

            if (RelatedTags.Count > 0)
            {
                s.Append(" { ");
                s.Append(string.Join(", ", RelatedTags.Select(rt => " { " + rt.ToVertex.ToString() + " } ")));
                s.Append(" } ");
            }
            return s.ToString();
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Tag>(db);
            RelatedTags?.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
