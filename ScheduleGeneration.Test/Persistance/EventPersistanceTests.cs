using ArangoDB.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Shouldly;
using TestStack.BDDfy;
using TestHelpers;
using Xunit;

namespace ScheduleGeneration.Test.Persistance
{
    [TestClass]
    public class EventPersistanceTests
    {
        public class CreateThenSaveThenUpdateThenSave
        {
            private IArangoDatabase _db;
            private IEvent _event;
            private IClock _clock;

            [Fact]
            public void Execute()
            {
                var mockDb = MockVertexFactory<Event>.GetArangoDatabase();

                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                this.WithExamples(new ExampleTable(
                    "db",
                    "Clock",
                    "Event"
                )
                {
                    {
                        mockDb.Object,
                        fakeClock,
                        new Event()
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

            public void AndGivenAnEvent(IEvent @event)
            {
                _event = @event;
            }

            public void WhenEventIsSaved()
            {
                _event.Save(_db, _clock);
            }

            public void AndWhenEventIsUpdated()
            {
                _event.Title = "other";
            }

            public void AndWhenEventIsSavedAgain()
            {
                _event.Save(_db, _clock);
            }

            public void ThenIsNewShouldBeFalse()
            {
                _event.IsNew.ShouldBeFalse();
            }

            public void AndThenIsDeletedShouldBeFalse()
            {
                _event.IsDeleted.ShouldBeFalse();
            }

            public void AndThenIsPersistedShouldBeTrue()
            {
                _event.IsPersisted.ShouldBeTrue();
            }

            public void AndThenIsDirtyShouldBeFalse()
            {
                _event.IsDirty.ShouldBeFalse();
            }

        }
    }
}
