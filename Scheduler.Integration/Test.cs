using System.Linq;
using ArangoDB.Client;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Users;

namespace IntegrationTests.School
{
    public class Test
    {
        public void Go(IArangoDatabase db, IClock clock)
        {
            var generator = new SourceScenarios.School.Generate();

            var organisation = generator.Organisation;

            var location = generator.Location;

            organisation.Location = new EdgeVertex<ILocation>(location);

            organisation.Save(db, clock);

            var terms = generator
                .Terms
                .ToList();

            var organisationTerms = new EdgeVertexs<ISchedule>(terms);

            organisationTerms.Save(db, clock, organisation, "HasTerm");
        }
    }
}
