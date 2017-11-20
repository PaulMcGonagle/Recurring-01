using System;
using System.Collections.Generic;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Schedule : Vertex, ISchedule
    {
        public virtual IEnumerable<IDate> Generate(IClock clock)
        {
            return ScheduleInstance.Generate(clock);
        }

        public string TypeName => GetType().FullName;

        public ScheduleInstance ScheduleInstance { get; set; }

        public void Validate()
        {
            Guard.AgainstNull(ScheduleInstance, nameof(ScheduleInstance));
        }

        internal override void Save<T> (
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

    public class ScheduleBuilder
    {
        private Schedule _schedule;

        public ScheduleBuilder()
        {
            _schedule = new Schedule();
        }

        public ScheduleInstance ScheduleInstance
        {
            set => _schedule.ScheduleInstance = value;
        }

        public ISchedule Build()
        {
            _schedule.Validate();

            return _schedule;
        }
    }
}
