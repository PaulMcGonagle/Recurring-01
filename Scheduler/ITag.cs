using System.Collections.Generic;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface ITag : IVertex
    {
        string Ident { get; set; }
        string Value { get; set; }
        string Payload { get; set; }

        EdgeVertexs<ITag> RelatedTags { get; set; }

        ITag Connect(ITag connectTag);
        ITag Connect(string ident, string value);
        void Connect(IEnumerable<ITag> connectTags);
    }
}