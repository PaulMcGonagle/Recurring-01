using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Generators;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.Users;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class TagGenerationTests
    {
        public class VerifyTags
        {
            private string _sourceFile;
            private IOrganisation _organisation;
            private IEnumerable<IEvent> _events;
            private IEvent _event;
            private ISerial _serial;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "organisation",
                    "expectedEventTags",
                    "expectedEventTags",
                    "expectedEventTags")
                    {
                        {
                            "C:\\Users\\Paul\\Documents\\Sandbox\\Recurring\\Recurring 01\\ScheduleGeneration.Test\\TestData\\BasicSchoolSchedule.xml",
                            new Organisation
                            {
                                Title = "a test organisation",
                            },
                            new List<ITag>
                            {
                                new Tag
                                {
                                    Ident = "event_id_A",
                                    Value = "event_value_A",
                                    RelatedTags = new EdgeVertexs<ITag>
                                        {
                                            new EdgeTag(new Tag
                                            {
                                                Ident = "event_id_B",
                                                Value = "event_value_B",
                                            }),
                                        },
                                },
                            },
                            new List<ITag>
                            {
                                new Tag
                                {
                                    Ident = "event_id_A",
                                    Value = "event_value_A",
                                    RelatedTags = new EdgeVertexs<ITag>
                                        {
                                            new EdgeTag(new Tag
                                            {
                                                Ident = "event_id_B",
                                                Value = "event_value_B",
                                            }),
                                        },
                                },
                            },
                            new List<ITag>
                            {
                                new Tag
                                {
                                    Ident = "event_id_A",
                                    Value = "event_value_A",
                                    RelatedTags = new EdgeVertexs<ITag>
                                        {
                                            new EdgeTag(new Tag
                                            {
                                                Ident = "event_id_B",
                                                Value = "event_value_B",
                                            }),
                                        },
                                },
                            }
                        },
                    })
                    .BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;
            }

            public void AndGivenAnOrganisation(IOrganisation organisation)
            {
                _organisation = organisation;
            }

            public void WhenEventsAreGenerated()
            {
                _events = Generator.GenerateEvents(_sourceFile, _organisation);
            }

            public void ThenOneEventIsGenerated()
            {
                _events.Count().ShouldBe(1);

                _event = _events.Single();
            }

            public void AndThenEventTagsAreAsExpected(IEnumerable<ITag> expectedEventTags)
            {
                _event.Tags.Select(t => t.ToVertex).ShouldBe(expectedEventTags);
            }

            public void AndThenEventHasOneSerial(IEnumerable<ITag> expectedEventTags)
            {
                _event.Serials.Count().ShouldBe(1);

                _serial = _event.Serials.Single().ToVertex;
            }

            public void AndThenSerialHasTagsAsExpected(IEnumerable<ITag> expectedEventTags)
            {
                _serial.Tags.Select(t => t.ToVertex).ShouldBe(expectedEventTags);
            }
        }
    }
}
