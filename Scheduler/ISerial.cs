using Scheduler.Persistance;

namespace Scheduler
{
    public interface ISerial : IVertex
    {
        IEdgeVertexs<IEpisode> Episodes { get; }

        IEdgeVertexs<ITag> Tags { get; set; }
    }
}
