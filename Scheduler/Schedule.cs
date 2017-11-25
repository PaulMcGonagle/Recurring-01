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
        private string _instanceSerialized;

        public virtual IEnumerable<IDate> Generate(IClock clock)
        {
            return ScheduleInstance.Generate(clock);
        }

        public string TypeName => GetType().FullName;

        public string InstanceSerialized
        {
            get { return _instanceSerialized; }
            set { _instanceSerialized = value; }
        }

        [IgnoreDataMember]
        public IScheduleInstance ScheduleInstance { get; set; }

        public void SerializeInstance()
        {
            _instanceSerialized = Transfer.Serialize(ScheduleInstance);
        }

        public void DeserializeInstance()
        {

            switch (TypeName)
            {
                case "Scheduler.CompositeSchedule":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.CompositeSchedule>(_instanceSerialized);
                    break;
                case "ScheduleInstances.ByDateList":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByDateList>(_instanceSerialized);
                    break;
                case "Scheduler.ByDayOfMonth":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByDayOfMonth>(_instanceSerialized);
                    break;
                case "Scheduler.ByDayOfYear":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByDayOfYear>(_instanceSerialized);
                    break;
                case "Scheduler.ByOffset":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByOffset>(_instanceSerialized);
                    break;
                case "Scheduler.ByRangeDate":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByRangeDate>(_instanceSerialized);
                    break;
                    break;
                case "Scheduler.ByWeekdays":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.ByWeekdays>(_instanceSerialized);
                    break;
                case "Scheduler.SingleDay":
                    ScheduleInstance = Transfer.Deserialize<ScheduleInstances.SingleDay>(_instanceSerialized);
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

        public override void Rehydrate(IArangoDatabase db)
        {


            base.Rehydrate(db);
        }

        public class Builder : Vertex.Builder<Schedule>
        {
            public IScheduleInstance ScheduleInstance
            {
                set => _target.ScheduleInstance = value;
            }
        }
    }
}