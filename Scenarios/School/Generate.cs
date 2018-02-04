using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using Generators;
using NodaTime;
using Scheduler;
using Scheduler.Calendars;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Scheduler.Users;

namespace SourceScenarios.School
{
    public class Generate : SourceScenario
    {
        private readonly IArangoDatabase _db;
        private readonly IClock _clock;

        public IGeneratorSource GeneratorSource { get; private set; }
        public IList<ISchedule> Years { get; } = new List<ISchedule>();
        public IList<ISchedule> Holidays { get; } = new List<ISchedule>();
        public IList<ICalendar> Calendars { get; } = new List<ICalendar>();

        public Generate WithYears()
        {
            var generator = GenerateFromFileFactory.Get("calendar");

            var generated = generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Years.xml",
                    _clock)
                .ToList();

            Vertexs = Vertexs
                .Union(generated);

            return this;
        }

        public IEnumerable<ISchedule> Terms
        {
            get
            {
                var generator = GenerateFromFileFactory.Get("calendar");

                var generated = generator.Generate(
                        "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Terms.xml",
                        _clock)
                    .ToArray();

                return generated
                    .OfType<ISchedule>();
            }
        }

        public Generate WithPersons()
        {
            var user = new User.Builder
            {
                Forename = "Bob",
                Surname = "Smith",
            }.Build();

            Vertexs = Vertexs
                .Union( new []{
                    user}
            );

            var calendar = new Calendar.Builder
            {
                Events = new EdgeVertexs<IEvent>(),
                Description = "Personal calendar from Google",
            }.Build();

            Vertexs = Vertexs
                .Union(new[]{
                    calendar}
                );

            user
                .ConnectAsEdge(calendar, "Manages")
                .Save(_db, _clock);

            return this;
        }

        public Generate WithEpisodes()
        {
            var schedule = new Schedule(new ByWeekdays.Builder
            {
                Weekdays = new[] {IsoDayOfWeek.Tuesday,},
                RangeDate = new RangeDate.Builder
                {
                    Start = new Date(2017, YearMonth.MonthValue.November, 03),
                    End = new Date(2017, YearMonth.MonthValue.December, 18),
                }.Build(),
            }.Build());

            var serial = new Serial.Builder
            {
                EdgeSchedule = new EdgeSchedule(schedule),
                RangeTime = new RangeTime.Builder
                    {
                        Start = new LocalTime(18, 00),
                        Period = new PeriodBuilder
                        {
                            Hours = 1
                        }.Build()
                    }.Build(),
                TimeZoneProvider = "Europe/London",
            }.Build();

            var episode = serial
                .GenerateEpisodes(_clock)
                .ToArray();

            var @event = new Event.Builder
            {
                Serial =  serial,
                Location = new Location(),
                Instance = new Instance(),
                Title = "an occassion"
            }.Build();

            Calendars.Add(new Calendar.Builder
                {
                    Description = "My personal calendar",
                    Events = new EdgeVertexs<IEvent>(@event),
                }.Build());

            return this;
        }

        public Generate WithSchoolGroups()
        {
            if (Calendars.Any(calendar => calendar.Description == "Hampden Gurney Year 1"))
            {
                Calendars.Add(new Calendar.Builder
                {
                    Description = "Hampden Gurney Year 1",
                }.Build());
            }

            if (Calendars.Any(calendar => calendar.Description == "Hampden Gurney Year 2"))
            {
                Calendars.Add(new Calendar.Builder
                {
                    Description = "Hampden Gurney Year 2",
                }.Build());
            }

            if (Calendars.Any(calendar => calendar.Description == "Hampden Gurney Year 3"))
            {
                Calendars.Add(new Calendar.Builder
                {
                    Description = "Hampden Gurney Year 3",
                }.Build());
            }

            if (Calendars.Any(calendar => calendar.Description == "Hampden Gurney Year 4"))
            {
                Calendars.Add(new Calendar.Builder
                {
                    Description = "Hampden Gurney Year 4",
                }.Build());
            }

            if (Calendars.Any(calendar => calendar.Description == "Hampden Gurney Year 5"))
            {
                Calendars.Add(new Calendar.Builder
                {
                    Description = "Hampden Gurney Year 5",
                }.Build());
            }

            return this;
        }

        public Generate WithSchoolClass()
        {
            var generator = GenerateFromFileFactory.Get("classes");

            var generated = generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\HG.xml",
                    _clock)
                .ToList();

            Vertexs = Vertexs
                .Union(generated);

            return this;
        }

        public Generate WithTimetable()
        {
            var generator = GenerateFromFileFactory.Get("timetables");

            var generated = generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\TimetableShape.xml",
                    _clock)
                .ToList();

            Vertexs = Vertexs
                .Union(generated);

            return this;
        }

        public IOrganisation Organisation => new Organisation {Title = "Springfield Primary School"};
        public ILocation Location => new Location.Builder { Address = "Main Street" }.Build();
    }
}
