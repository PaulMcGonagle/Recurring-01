using System;
using NodaTime;
using NodaTime.Testing;

namespace InitialiseDatabase
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var db = SchedulerDatabase.Database.Retrieve();
            var clock = new FakeClock(new Instant(DateTime.Now.Ticks));
            
            var integrationTest =  new IntegrationTests.School.Test();

            integrationTest.Go(db, clock);
        }
    }
}