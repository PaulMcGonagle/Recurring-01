using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ArangoDB.Client;
using Generators;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Shouldly;
using TestHelpers;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class LoadAndEnrich
    {
        public class EnrichEventNumbering
        {
            private string _sourceFile;
            private IClock _clock;
            private IGenerateFromFile _generator;
            private IEnumerable<IVertex> _vertexs;
            private ISchedule _schedule;
            private CompositeSchedule _compositeSchedule;
            private IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "db",
                    "expectedDates"
                )
                {
                    {
                        AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\BasicSchoolSchedule.xml",
                        fakeClock,
                        mockDb.Object,
                        new List<IDate>
                        {
                            new Date(2017, YearMonth.MonthValue.January, 02),
                            new Date(2017, YearMonth.MonthValue.January, 03),
                            new Date(2017, YearMonth.MonthValue.January, 04),
                            new Date(2017, YearMonth.MonthValue.January, 06),
                            new Date(2017, YearMonth.MonthValue.January, 09),
                            new Date(2017, YearMonth.MonthValue.January, 10)
                        }
                    },
                }).BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;

                XElement.Load(sourceFile);
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void AndGivenADatabase(IArangoDatabase db)
            {
            }

            public void WhenGeneratorIsRetrieved()
            {
                _generator = GeneratorFactory.Get("classes");
            }

            public void AndWhenVertexsAreGenerated()
            {
                _vertexs = _generator
                    .Generate(_sourceFile, _clock)
                    .ToList();
            }

            public void ThenAScheduleIsCreated()
            {
                _schedule = _vertexs
                    .OfType<ISchedule>()
                    .SingleOrDefault();

                _schedule.ShouldNotBeNull();
            }

            public void AndThenScheduleInstanceIsACompositeSchedule()
            {
                _schedule.ScheduleInstance.ShouldBeOfType<CompositeSchedule>();
                _compositeSchedule = (CompositeSchedule)_schedule.ScheduleInstance;
            }

            public void AndThenSchedulesAreGenerated()
            {
                _dates = _compositeSchedule
                    .Generate(_clock);
            }

            public void AndThenDatesAreExpected(IEnumerable<IDate> expectedDates)
            {
                _dates
                    .ShouldBe(expectedDates);
            }
        }
    }
}
