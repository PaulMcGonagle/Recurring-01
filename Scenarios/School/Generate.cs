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
        private IArangoDatabase _db;
        private IClock _clock;

        private IGeneratorSource _generatorSource;
        private ISchedule _years;
        private IEnumerable<ISchedule> _terms;
        private ISchedule _holidays;

        public Generate(IArangoDatabase db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public void Go()
        {
            //GenerateYears();
            //GenerateTerms();
            //GenerateHolidays();
            //GeneratePersons();
            GenerateEpisodes();

            var organisation = new Organisation.Builder
            {
                Title = "Westminster Council School Holidays",
            }.Build();

            organisation.Save(_db, _clock);

            var organisationTerms = new EdgeVertexs<ISchedule>(_terms);

            organisationTerms.Save(_db, _clock, organisation, "HasTerms");
        }

        public void GenerateYears()
        {
            var generator = GeneratorFactory.Get("calendar");

            var generated = generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Years.xml",
                    _clock)
                .ToArray();

            generated.Save(_db, _clock);

            foreach (var g in generated)
            {
                g.Save(_db, _clock);
            }

            _generatorSource = generated
                .OfType<IGeneratorSource>()
                .SingleOrDefault();

            _years = generated
                .OfType<ISchedule>()
                .SingleOrDefault();
        }

        public void GenerateTerms()
        {
            var generator = GeneratorFactory.Get("calendar");

            var generated = generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Terms.xml",
                    _clock)
                .ToArray();

            foreach (var g in generated)
            {
                g.Save(_db, _clock);
            }

            _terms = generated
                .OfType<ISchedule>();
        }

        public void GenerateHolidays()
        {
            var generator = GeneratorFactory.Get("calendar");

            var generated = generator.Generate(
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Holidays.xml",
                    _clock)
                .ToArray();

            foreach (var g in generated)
            {
                g.Save(_db, _clock);
            }

            _holidays = generated
                .OfType<ISchedule>()
                .SingleOrDefault();
        }

        public void GeneratePersons()
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
        }

        public void GenerateEpisodes()
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
                Location = new EdgeVertex<ILocation>(new Location()),
                Instance = new Instance(),
                Title = "an occassion"
            }.Build();

            var calendar = new Calendar.Builder
            {
                Description = "My personal calendar",
                Events = new EdgeVertexs<IEvent>(@event),
            }.Build();
        }
    }
}
