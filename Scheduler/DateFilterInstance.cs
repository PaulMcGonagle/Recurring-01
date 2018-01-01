using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public abstract class DateFilterInstance : Vertex
    {
        public abstract bool Filter(LocalDate localDate);
    }
}
