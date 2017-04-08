using System.Collections.Generic;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;

namespace Generators
{
    public static class GenerateFromCalendar
    {
        public static IEnumerable<IVertex> Go(
            List<LocalDateTime> instances,
            string timeZoneProvider)
        {
            var xGenerators = new XElement("generators");
            var xGenerator = new XElement("generator");
            xGenerators.Add(xGenerator);
            var xGeneratorTags = new XElement("tags");
            xGenerator.Add(xGeneratorTags);
            xGeneratorTags.Add(new XElement("tag", new XAttribute("id", "timeZoneProvider"), new XAttribute("value", timeZoneProvider)));
            var xInstances = new XElement("instances");
            xGenerator.Add(xInstances);

            foreach (var instance in instances)
            {
                xInstances.Add(new XElement("instance", new XAttribute("when", instance.ToString())));   
            }

            yield return new GeneratorSource
            {
                GeneratorType = "google.calendar",
                Xml = xGenerators.ToString(),
            };
        }
    }
}
