using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.ScheduleEdges;

namespace Scheduler.DateFilterInstances
{
    public class DateFilterByRange : DateFilterInstance
    {
        public EdgeRangeDate EdgeRangeDate { get; set; }

        public override void Validate()
        {
            Guard.AgainstNull(EdgeRangeDate, nameof(Date));
        }

        public override bool Filter(LocalDate localDate)
        {
            return EdgeRangeDate
                .ToVertex
                .Contains(localDate);
        }

        #region Save

        public void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            EdgeRangeDate?.Save(db, clock, schedule);
        }



        #endregion
    }
}
