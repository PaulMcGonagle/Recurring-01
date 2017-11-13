using System.Linq;
using ArangoDB.Client;
using Generators;
using NodaTime;

namespace School
{
    public class Generate
    {
        private IArangoDatabase _db;
        private IClock _clock;

        public Generate(IArangoDatabase db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public void Go()
        {
            //GenerateYears();
            GenerateTerms();
        }

        public void GenerateYears()
        {
            var generator = GeneratorFactory.Get("calendar");

            var generated = generator.Generate(
                "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Years.xml",
                _clock);

            foreach (var g in generated)
            {
                g.Save(_db, _clock);
            }
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
        }
    }
}
