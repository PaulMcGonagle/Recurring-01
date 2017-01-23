using System.Runtime.Serialization;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Tag : Vertex, ITag
    {
        public Tag(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        [IgnoreDataMember]
        public EdgeVertexs<ITag> RelatedTags { get; set; } = new EdgeVertexs<ITag>();
    }
}
