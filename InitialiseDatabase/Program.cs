using System.Linq;
using ConsoleOutput;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Calendars;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleInstances;
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

            var i = generate.GeneratorSource
                .Caches
                .GetToVertexs()
                .GetByTag(ident: "name", value: "Year.2018");

            var summerTerm = generate
                .Terms
                .GetByTag(ident: "name", value: "2017/18.Summer")
                .Single();

            var compositeSchedule = new CompositeSchedule.Builder
            {
                Inclusion = summerTerm,
                Exclusion = generate.Holidays.Single(),
            }.Build();

            var newSerial = new Serial.Builder
            {
                Schedule = new Schedule(compositeSchedule),
                RangeTime = new RangeTime.Builder
                    {
                        Start = new LocalTime(09, 00),
                        End = new LocalTime(09, 50),
                    }.Build(),
                TimeZoneProvider = timeZoneProvider,
            }.Build();

            var episodes = newSerial
                .GenerateEpisodes(fakeClock)
                .GetToVertexs();

            Output.DisplayList(episodes);


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

            Output.Wait();
        }
    }
}
