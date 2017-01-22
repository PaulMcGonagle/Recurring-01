using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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
