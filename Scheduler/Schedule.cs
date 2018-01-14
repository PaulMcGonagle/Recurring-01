using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Schedule : Vertex, ISchedule
    {
        public Schedule(IScheduleInstance scheduleInstance)
        {
            ScheduleInstance = scheduleInstance;
        }

        public virtual IEnumerable<IDate> Generate(IClock clock)
        {
            return ScheduleInstance.Generate(clock);
        }

        public string TypeName => GetType().FullName;

        public string InstanceSerialized { get; set; }

        [IgnoreDataMember]
        public IScheduleInstance ScheduleInstance { get; set; }

        public void SerializeInstance()
        {
            InstanceSerialized = Transfer.Serialize(ScheduleInstance);
        }

        public void DeserializeInstance()
        {

            switch (TypeName)
            {
                case "Scheduler.CompositeSchedule":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.CompositeSchedule>(InstanceSerialized);
                    break;
                case "ScheduleInstances.ByDateList":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByDateList>(InstanceSerialized);
                    break;
                case "Scheduler.ByDayOfMonth":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByDayOfMonth>(InstanceSerialized);
                    break;
                case "Scheduler.ByDayOfYear":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByDayOfYear>(InstanceSerialized);
                    break;
                case "Scheduler.ByOffset":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByOffset>(InstanceSerialized);
                    break;
                case "Scheduler.ByRangeDate":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByRangeDate>(InstanceSerialized);
                    break;
                case "Scheduler.ByWeekdays":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByWeekdays>(InstanceSerialized);
                    break;
                case "Scheduler.SingleDay":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.SingleDay>(InstanceSerialized);
                    break;
                default:
                    throw new KeyNotFoundException(TypeName);
            }
        }

        public override void Validate()
        {
            Guard.AgainstNull(ScheduleInstance, nameof(ScheduleInstance));
        }

        internal override void Save<T>(
            IArangoDatabase db)
        {
            base.Save<Schedule>(db);
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Schedule>(db);
            base.Save(db, clock);
        }
    }
}