namespace Scheduler.Generation
{
    public class Generator
    {
        public void Snapshot(Event source)
        {
            foreach (var serial in source.Serials)
            {
                if (serial.EdgeSchedule != null)
                {
                    Snapshot(serial.EdgeSchedule.Schedule);
                }
            }
        }

        public void Snapshot(Schedule schedule)
        {
            
        }
    }
}
