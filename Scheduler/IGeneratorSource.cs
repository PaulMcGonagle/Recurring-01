using Scheduler.Persistance;

namespace Scheduler
{
    public interface IGeneratorSource : IVertex
    {
        string Xml { get; set; }

        string GeneratorType { get; set; }

        IEdgeVertexs<IVertex> Caches { get; set; }
        
        IEdgeVertexs<ISchedule> Schedules { get; set; }
    }
}
