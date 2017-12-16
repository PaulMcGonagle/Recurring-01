using System.Linq;
using ConsoleOutput;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Calendars;
using Scheduler.Persistance;
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

            var location = new Location
            {
                Address = "the school",
            };

            var events = generate
                .Vertexs
                .OfType<ISerial>()
                .Select(serial => new Event.Builder
                {
                    Instance = null,
                    Location = location,
                    Serial = serial,
                    Title = serial.GetTagValue("name"),
                }.Build());

            var calendar = new Calendar.Builder
                {
                    Events = new EdgeVertexs<IEvent>(events),
                    Description = "school timetable"
                }.Build();



            var calendarEvents = calendar
                .Events
                .GetToVertexs();

            var calendarSerials = calendarEvents
                .SelectMany(c => c.Serials.GetToVertexs());

            var calendarEpisodes = calendarSerials
                .SelectMany(e => e.GenerateEpisodes(fakeClock));

            Output.DisplayList(calendarEpisodes, timeZoneProvider);
            Output.Wait();
        }
    }
}
