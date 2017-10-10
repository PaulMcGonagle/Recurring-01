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
            private IRelation _relation;

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
                _relation = ExternalId.Link(_toVertex);
            }

            public void ThenRelationIsCreated()
            {
                _relation.ShouldNotBeNull();
            }

            public void AndThenRelationToVertexIsCorrect()
            {
                _relation.ToVertex.ShouldBe(_toVertex);
            }

            public void AndThenRelationHasFromVertex()
            {
                _relation.FromVertex.ShouldNotBeNull();
            }

            public void AndThenFromVertexIsExternalId()
            {
                _relation.FromVertex.ShouldBeOfType(typeof(ExternalId));
            }

            public void AndThenExternalIdIsValid()
            {
                ((ExternalId)_relation.FromVertex)
                    .Uid.ShouldNotBeNullOrEmpty();
            }
        }
    }
}
