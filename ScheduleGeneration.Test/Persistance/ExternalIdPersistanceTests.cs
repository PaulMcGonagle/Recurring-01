using ArangoDB.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Testing;
using Scheduler.Persistance;
using TestStack.BDDfy;
using Xunit;
using Shouldly;

namespace ScheduleGeneration.Test.Persistance
{
    [TestClass]
    public class ExternalIdPersistanceTests
    {
        public class CreateAndPersistExternalId
        {
            private IArangoDatabase _db;
            private IClock _clock;
            private ExternalId _externalId;

            [Fact]
            public void Execute()
            {
                var mockDb = MockVertexFactory<ExternalId>.GetArangoDatabase();

                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                this.WithExamples(new ExampleTable(
                    "db",
                    "Clock"
                )
                {
                    {
                        mockDb.Object,
                        fakeClock
                    }
                }).BDDfy();
            }

            public void GivenADatabase(IArangoDatabase db)
            {
                _db = db;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenAnExternalIdIsGenerated()
            {
                _externalId = new ExternalId();
            }

            public void ThenExternalId()
            {
                _externalId.ShouldNotBeNull();
            }

            public void AndThenExternalIdHasUid()
            {
                _externalId.Uid.ShouldNotBeNullOrWhiteSpace();
            }
        }


        public class CreatePersistAndLinkExternalId
        {
            private IArangoDatabase _db;
            private IClock _clock;
            private IVertex _toVertex;
            private IEdge _edge;

            [Fact]
            public void Execute()
            {
                var mockDb = MockVertexFactory<ExternalId>.GetArangoDatabase();

                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                this.WithExamples(new ExampleTable(
                        "db",
                        "Clock"
                    )
                    {
                        {
                            mockDb.Object,
                            fakeClock
                        }
                    })
                    .BDDfy();
            }
            
            public void GivenADatabase(IArangoDatabase db)
            {
                _db = db;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void AndGivenAToVertex(IVertex toVertex)
            {
                _toVertex = toVertex;
            }

            public void WhenRelationIsCreated()
            {
                _edge = ExternalId.Link(_toVertex);
            }

            public void ThenRelationIsCreated()
            {
                _edge.ShouldNotBeNull();
            }

            public void AndThenRelationToVertexIsCorrect()
            {
                _edge.ToVertex.ShouldBe(_toVertex);
            }

            public void AndThenRelationHasFromVertex()
            {
                _edge.FromVertex.ShouldNotBeNull();
            }

            public void AndThenFromVertexIsExternalId()
            {
                _edge.FromVertex.ShouldBeOfType(typeof(ExternalId));
            }

            public void AndThenExternalIdIsValid()
            {
                ((ExternalId)_edge.FromVertex)
                    .Uid.ShouldNotBeNullOrEmpty();
            }
        }
    }
}
