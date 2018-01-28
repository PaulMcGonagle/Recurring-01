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

namespace School
{
    public class Generate
    {
        private readonly IArangoDatabase _db;
        private readonly IClock _clock;

        public IGeneratorSource GeneratorSource { get; private set; }
        public IList<ISchedule> Years { get; } = new List<ISchedule>();
        public IList<ISchedule> Terms { get; } = new List<ISchedule>();
        public IList<ISchedule> Holidays { get; } = new List<ISchedule>();
        public IList<ICalendar> Calendars { get; } = new List<ICalendar>();

        public List<IVertex> Vertexs { get; } = new List<IVertex>();

        public Generate(IArangoDatabase db, IClock clock)
        {
            _db = db;
            _clock = clock;

            var organisation = new Organisation.Builder
            {
                Title = "Westminster Council Schools",
            }.Build();

            organisation.Save(_db, _clock);

            var organisationTerms = new EdgeVertexs<ISchedule>(Terms);

            organisationTerms.Save(_db, _clock, organisation, "HasTerms");
        }

        public Generate WithYears()
        {
            var generator = GenerateFromFileFactory.Get("calendar");

            var generated = generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Years.xml",
                    _clock)
                .ToArray();

            generated.Save(_db, _clock);

            foreach (var g in generated)
            {
                g.Save(_db, _clock);
            }

            GeneratorSource = generated
                .OfType<IGeneratorSource>()
                .SingleOrDefault();

            Years.Add(generated
                .OfType<ISchedule>()
                .SingleOrDefault());

            return this;
        }

        public Generate WithTerms()
        {
            var generator = GenerateFromFileFactory.Get("calendar");

            var generated = generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Terms.xml",
                    _clock)
                .ToArray();

            foreach (var g in generated)
            {
                g.Save(_db, _clock);
            }

            foreach (var schedule in generated
                .OfType<ISchedule>())
            {
                Terms.Add(schedule);
            }

            return this;
        }

        public Generate WithHolidays()
        {
            var generator = GenerateFromFileFactory.Get("calendar");

            var generated = generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Holidays.xml",
                    _clock)
                .ToArray();

            foreach (var g in generated)
            {
                g.Save(_db, _clock);
            }

            Holidays.Add(generated
                .OfType<ISchedule>()
                .SingleOrDefault());

            return this;
        }

        public Generate WithPersons()
        {
            var user = new User.Builder
            {
                Forename = "Bob",
                Surname = "Smith",
            }.Build();

            user.Save(_db, _clock);

            var calendar = new Calendar.Builder
            {
                Events = new EdgeVertexs<IEvent>(),
                Description = "Personal calendar from Google",
            }.Build();

            calendar.Save(_db, _clock);

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

            Vertexs.AddRange(generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\HG.xml",
                    _clock)
                .ToArray());

            return this;
        }

        public Generate WithTimetable()
        {
            var generator = GenerateFromFileFactory.Get("timetables");

            Vertexs.AddRange(generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\TimetableShape.xml",
                    _clock)
                .ToArray());

            return this;
        }
    }
}
