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

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "organisation",
                    "expectedTags")
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
                                    Ident = "id_A",
                                    Value = "value_A",
                                    RelatedTags = new EdgeVertexs<ITag>
                                        {
                                            new EdgeTag(new Tag
                                            {
                                                Ident = "id_B",
                                                Value = "value_B",
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
            }

            public void AndThenTagsAreAsExpected(IEnumerable<ITag> expectedTags)
            {
                var @event = _events.Single();

                @event.Tags.Select(t => t.ToVertex).ShouldBe(expectedTags);
            }
        }
    }
}
