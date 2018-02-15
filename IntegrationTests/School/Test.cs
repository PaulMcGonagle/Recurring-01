using System.Linq;
using ArangoDB.Client;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;
using Scheduler.Users;
using School;

namespace IntegrationTests.School
{
    public class Test
    {
        public void Go(IArangoDatabase db, IClock clock)
        {
            var schoolGenerator = new Generator();

            var organisation = schoolGenerator.Organisation;

            var location = schoolGenerator.Location;

            organisation.Location = new EdgeVertex<ILocation>(location);

            organisation.Save(db, clock);

            var terms = schoolGenerator
                .Terms
                .ToList();

            var organisationTerms = new EdgeVertexs<ISchedule>(terms);

            organisationTerms.Save(db, clock, organisation, "HasTerm");

            var holidayGenerator = new SourceScenarios
                .Holidays
                .Generator(clock);

            var years = holidayGenerator
                .Years;

            years.Save(db, clock);

            var holidayDates = holidayGenerator
                .HolidayDates
                .ToList();

            var holidayDatesGrouped = holidayDates
                .GroupBy(holidayDate => holidayDate
                    .Tags
                    .ToVertexs
                    .Where(t => t.Ident == "year")
                    .Select(t => t.Value)
                    .FirstOrDefault())
                .ToList();

            var holidaysUK = holidayDatesGrouped
                .Select(hg =>
                {
                    var year = hg.Key;

                    var schedule = new Schedule(new ByDateList.Builder
                    {
                        Items = new EdgeVertexs<IDate>(holidayDates),
                    }.Build());

                    schedule.Connect("type", "Public Holidays");
                    schedule.Connect("locale", "UK");
                    schedule.Connect("year", year);

                    return schedule;
                });

            holidaysUK.Save(db, clock);
        }
    }
}
