using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Generation
{
    public interface IInstance : IVertex
    {
        IEdgeVertexs<IEpisode> Episodes { get; set; }
        Instant Time { get; set; }
    }
}