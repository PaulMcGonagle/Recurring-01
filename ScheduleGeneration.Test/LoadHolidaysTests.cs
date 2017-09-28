using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using Generators;
using Generators.Instances;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class LoadHolidaysTests
    {
        public class LoadHolidays
        {
            private string _sourceFile;
            private IArangoDatabase _db;
            private IClock _clock;
            private IGenerator _generator;
            private GeneratorCalendars _generatorCalendars;
            private IEnumerable<IVertex> _vertexs;
            private IEnumerable<ISchedule> _schedules;
            private IEnumerable<LocalDate> _dates;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "db",
                    "clock",
                    "Expected Dates"
                )
                {
                    {
                        "..\\..\\TestData\\Holidays.xml",
                        mockDb.Object,
                        fakeClock,
                        new List<LocalDate>
                        {
                            new LocalDate(2017, 12, 25),
                            new LocalDate(2017, 01, 02),
                            new LocalDate(2018, 01, 01),
                        }
                    },
                }).BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void AndGivenADatabase(IArangoDatabase db)
            {
                _db = db;
            }
            
            public void WhenGeneratorIsRetrieved()
            {
                _generator = GeneratorFactory.Get("holidays");
            }

            public void AndWhenGeneratorIsHoliday()
            {
                _generator.ShouldBeOfType<GeneratorCalendars>();

                _generatorCalendars = (GeneratorCalendars) _generator;
            }

            public void AndWhenGenerated()
            {
                _vertexs = _generatorCalendars.Generate(_sourceFile, _clock);
            }

            public void AndWhenSchedulesAreRetrived()
            {
                _schedules = _vertexs
                    .OfType<Schedule>();
            }

            public void AndWhenDatesAreRetrived()
            {
                _dates = _schedules
                    .SelectMany(schedule => schedule.Generate(_clock))
                    .Select(date => date.Value)
                    .ToList();
            }

            public void ThenDatesAreAsExpected(IEnumerable<LocalDate> expectedDates)
            {
                _dates
                    .OrderBy(d => d)
                    .ShouldBe(expectedDates.OrderBy(ed => ed));
            }
        }
    }
}