using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Generation
{
    public class GeneratedDate : Vertex
    {
        public Instant Instant { get; set; }

        public EdgeVertexs<Date> Source { get; set; }

        public GeneratedDate(IClock clock, Date source, LocalDate time, Period period)
        {
        }
    }
}
