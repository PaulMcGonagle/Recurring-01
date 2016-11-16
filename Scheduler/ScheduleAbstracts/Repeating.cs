using NodaTime;

namespace Scheduler.ScheduleAbstracts
{
    public abstract class Repeating : ScheduleBase
    {

        public LocalDate? DateFrom;
        public LocalDate? DateTo;
        public int? CountFrom;
        public int? CountTo;
        public int CountFromDefault { get; set; }
        public int CountToDefault { get; set; }
        protected int increment = 1;
    }
}
