using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static string CombinedKey(Tag tag)
        {
            return $"{tag.Ident}.{tag.Value}";
        }

        int IComparable.CompareTo(object obj)
        {
            var compareTag = (Tag)obj;

            var compare = String.Compare(CombinedKey(this), CombinedKey((Tag)obj), StringComparison.Ordinal);

            if (compare != 0)
                return compare;

            compare = Compare(Tags.Select(r => r.ToVertex), Tags.Select(r => r.ToVertex));

            if (compare != 0)
                return compare;

            //todo Introduce Payload Json comparison
            compare = string.Compare(Payload, compareTag.Payload, StringComparison.Ordinal);

            return compare;
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
            var s = new StringBuilder($"Tag ident: \"{Ident}\", value: \"{Value}\"");

            if (Payload != null)
                s.Append($", payload: \"{Payload}");

            if (Tags.Count <= 0)
                return s.ToString();

            s.Append(" { ");
            s.Append(string.Join(", ", Tags.Select(rt => " { " + rt.ToVertex.ToString() + " } ")));
            s.Append(" } ");

            return s.ToString();
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Tag>(db);
            Tags?.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
