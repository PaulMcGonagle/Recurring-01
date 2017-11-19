using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ArangoDB.Client;
using ConsoleOutput;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Scheduler.Users;
using School;

namespace InitialiseDatabase
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            Output.WriteLine("test");
            var fakeClock = new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00));

            var db = SchedulerDatabase.Database.Retrieve();

            var generate = new Generate(db, fakeClock);

            generate.Go();
        }
    }
}
