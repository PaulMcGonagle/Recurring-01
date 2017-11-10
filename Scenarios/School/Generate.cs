using ArangoDB.Client;
using Generators;
using NodaTime;

namespace School
{
    public class Generate
    {
        public static void Go(IArangoDatabase db, IClock clock)
        {
            var generator = GeneratorFactory.Get("calendar");

            var generated = generator.Generate(
                "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Scenarios\\School\\Files\\Years.xml",
                clock);

            foreach (var g in generated)
            {
                g.Save(db, clock);
            }
        }
    }
}
