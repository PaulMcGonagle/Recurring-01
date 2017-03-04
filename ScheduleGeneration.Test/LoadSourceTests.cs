using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Generators;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Generation;
using Scheduler.Persistance;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class LoadSourceTests
    {
        public class LoadHg
        {
            private string _sourceFile;
            private XElement _source;
            private IClock _clock;
            private IEnumerable<IEvent> _events;
            private IEvent _event;
            private IGeneratedEvent _generatedEvent;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "expectedTitle",
                    "tagPath"
                )
                {
                    {
                        "C:\\Users\\Paul\\Documents\\Sandbox\\Recurring\\Recurring 01\\ScheduleGeneration.Test\\TestData\\BasicSchoolSchedule.xml",
                        fakeClock,
                        "Hampden Gurney Primary School.Autumn 2016/17.Literacy",
                        "./generator/events/event"
                    },
                }).BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;

                _source = XElement.Load(sourceFile);
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenEventsAreGenerated()
            {
                _events = Generator.GenerateEvents(_sourceFile);
            }

            private void ThenOnlyOneEventIsCreated()
            {
                _events.ShouldHaveSingleItem();

                _event = _events.Single();
            }

            private void AndTitleIsExpected(string expectedTitle)
            {
                _event.Title.ShouldBe(expectedTitle);
            }

            private void AndTagsAreExpected(string tagPath)
            {
                var tags = _event.Tags.Select(t => t.ToVertex);

                var expectedTags = _source.XPathSelectElement(tagPath);

                TagGenerationTests.CompareTagsToSource(tags, expectedTags);
            }
        }
    }
}
