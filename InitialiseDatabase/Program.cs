using System.Linq;
using ConsoleOutput;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Calendars;
using Scheduler.Persistance;
using Scheduler.Ranges;
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
            var timeZoneProvider = "Europe/London";

            var db = SchedulerDatabase.Database.Retrieve();

            var generate = new Generate(db, fakeClock);

            generate.Go();

            var serials = generate
                .Vertexs
                .OfType<ISerial>()
                .Select(serial => new Event.Builder
                {
                    Instance = null,
                    Location = new Location {Address = "the school"},
                    Serial = serial,
                }.Build());

            var calendar = new Calendar.Builder
            {

            }.Build();

            //foreach (var calendar in generate.Calendars)
            //{
            //    foreach (var @event in calendar.Events.Select(e => e.ToVertex))
            //    {
            //        foreach (var serial in @event.Serials.Select(s => s.ToVertex))
            //        {
            //            var episodes = serial.GenerateEpisodes(fakeClock).Select(e => e.ToVertex);

            //            ConsoleOutput.Output.DisplayList(episodes);

            //            ConsoleOutput.Output.Wait();
            //        }
            //    }
            //}

            //Output.DisplayGrid(compositeSchedule.Generate(fakeClock));

        }
    }
}
