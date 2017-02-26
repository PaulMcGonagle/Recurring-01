using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
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
            private XElement _source;

            private IOrganisation _organisation;
            private IEnumerable<IEvent> _events;
            private IEvent _event;
            private ISerial _serial;
            private IEnumerable<ITag> _providedGeneratorTags;
            private IEnumerable<ITag> _providedEventTags;
            private IEnumerable<ITag> _providedScheduleTags;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "organisation",
                    "expectedGeneratorTags",
                    "expectedEventTags",
                    "expectedScheduleTags",
                    "eventPath",
                    "serialPath")
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
                                    Ident = "generator_id_A",
                                    Value = "generator_value_A",
                                    RelatedTags = new EdgeVertexs<ITag>
                                        {
                                            new EdgeTag(new Tag
                                            {
                                                Ident = "generator_id_B",
                                                Value = "generator_value_B",
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
                                    Payload = @"{ ""detail"" : { ""url"": ""http://www.hampdengurneyschool.org.uk"" } }",
                                    RelatedTags = new EdgeVertexs<ITag>
                                        {
                                            new EdgeTag(new Tag
                                            {
                                                Ident = "event_id_B",
                                                Value = "event_value_B",
                                            }),
                                        },
                                }
                            },
                            new List<ITag>
                            {
                                new Tag
                                {
                                    Ident = "_schedule_id_A",
                                    Value = "_schedule_value_A",
                                    RelatedTags = new EdgeVertexs<ITag>
                                        {
                                            new EdgeTag(new Tag
                                            {
                                                Ident = "_schedule_id_B",
                                                Value = "_schedule_value_B",
                                            }),
                                        },
                                }
                            },
                            "./generator/events/event",
                            "./generator/classes/class/schedules/schedule | ./generator/events/event"
                        },
                    })
                    .BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;

                _source = XElement.Load(sourceFile);
            }

            public void AndGivenAnOrganisation(IOrganisation organisation)
            {
                _organisation = organisation;
            }

            public void AndGivenGeneratorTags(IEnumerable<ITag> expectedGeneratorTags)
            {
                _providedGeneratorTags = expectedGeneratorTags;
            }

            public void AndGivenEventTags(IEnumerable<ITag> expectedEventTags)
            {
                _providedEventTags = expectedEventTags;
            }

            public void AndGivenScheduleTags(IEnumerable<ITag> expectedScheduleTags)
            {
                _providedScheduleTags = expectedScheduleTags;
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

            public void AndThenGenerationTagsAreValid(string eventPath)
            {
                var source = _source.XPathSelectElement(eventPath);

                source.ShouldNotBeNull();

                CompareTagsToSource(_event.Tags.Select(e => e.ToVertex), source);
            }

            public void AndThenEventHasOneSerial()
            {
                _event.Serials.Count().ShouldBe(1);

                _serial = _event.Serials.Single().ToVertex;
            }

            public void AndThenSerialTagsAreValid(string serialPath)
            {
                var source = _source.XPathSelectElement(serialPath);

                source.ShouldNotBeNull();

                CompareTagsToSource(_event.Tags.Select(e => e.ToVertex), source);
            }
        }

        private static void CompareTagsToSource(IEnumerable<ITag> tags, XElement source)
        {
            var sourceTags = source.XPathSelectElements("./tags/tag");

            var enumeratedTags = tags as ITag[] ?? tags.ToArray();

            enumeratedTags.Count().ShouldBe(sourceTags.Count());

            foreach (var tag in enumeratedTags)
            {
                var sourceTag = sourceTags
                    .Where(st => st.Attribute("id").Value == tag.Ident && st.Attribute("value").Value == tag.Value);

                sourceTag.ShouldHaveSingleItem();

                CompareTagsToSource(tag.RelatedTags.Select(rl => rl.ToVertex), sourceTag.Single());
            }
        }
    }
}
