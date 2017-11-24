using System.Linq;
using ArangoDB.Client;
using Generators;
using NodaTime;
using Scheduler;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.Users;

namespace School
{
    public class Generate
    {
        private IArangoDatabase _db;
        private IClock _clock;

        private IGeneratorSource _generatorSource;
        private ISchedule _years;
        private ISchedule _terms;
        private ISchedule _holidays;

        public Generate(IArangoDatabase db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public void Go()
        {
            GenerateYears();
            GenerateTerms();
            GenerateHolidays();
            GeneratePersons();
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

            var generatedInstant = new GeneratedInstantBuilder()
                .Create(_clock, _generatorSource, _years)
                .WithLabel("Generated")
                .Build();

            generatedInstant.Save(_db, _clock);
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
                .OfType<ISchedule>()
                .SingleOrDefault();
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
            var user = new UserBuilder
            {
                Forename = "Bob",
                Surname = "Smith",
            }.Build();

            user.Save(_db, _clock);

            var calendar = new CalendarBuilder
            {
                Dates = new EdgeVertexs<IDate>(),
                Description = "Personal calendar from Google",
            }.Build();

            calendar.Save(_db, _clock);
        }
    }
}
