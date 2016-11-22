using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test
{
    public class CompositeScheduleTests
    {
        public class MyTestClass
        {
            CompositeSchedule _sut;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "expected")
                    {
                        {
                            new CompositeSchedule
                            {
                                Inclusions = new List<ISchedule>
                                {
                                    TestData.DataRetrieval.ScheduleArchive[""],
                                }
                            },
                            "text"
                        }
                    })
                    .BDDfy();
            }

            public void GivenSut(CompositeSchedule sut)
            {
                _sut = sut;
            }

            public void WhenSomethingIsDone()
            {

            }

            public void ThenSomethingHappens(string expected)
            {

            }
        }

    }
}
