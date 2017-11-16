using System.Linq;
using ArangoDB.Client;
using Generators;
using NodaTime;
using Scheduler;

namespace School
{
    public class Generate
    {
        private IArangoDatabase _db;
        private IClock _clock;

        private ICompositeSchedule _years;
        private ICompositeSchedule _terms;
        private ICompositeSchedule _holidays;

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
        }

        public void GenerateYears()
        {
            var generator = GeneratorFactory.Get("calendar");

            var generated = generator.Generate(
                "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Years.xml",
                _clock)
                .ToArray();

            foreach (var g in generated)
            {
                g.Save(_db, _clock);
            }

            _years = generated
                .OfType<ICompositeSchedule>()
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
                .OfType<ICompositeSchedule>()
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
                .OfType<ICompositeSchedule>()
                .SingleOrDefault();
        }
    }
}
