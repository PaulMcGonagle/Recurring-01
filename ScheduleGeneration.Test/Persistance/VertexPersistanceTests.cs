using ArangoDB.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Shouldly;
using TestHelpers;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test.Persistance
{
    [TestClass]
    public class VertexPersistanceTests
    {
        public class CreateThenSave
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

            public void ThenToDeleteShouldBeFalse()
            {
                _event.ToDelete.ShouldBeFalse();
            }

            public void AndThenToPersistShouldBeFalse()
            {
                _event.IsPersisted.ShouldBeTrue();
            }

            public void AndThenIsNewShouldBeFalse()
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

        public class CreateThenSaveThenDelete
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

            public void AndWhenEventIsSetToDelete()
            {
                _event.SetToDelete();
            }

            public void AndWhenEventIsSavedAgain()
            {
                _event.Save(_db, _clock);
            }

            public void ThenToDeleteShouldBeFalse()
            {
                _event.ToDelete.ShouldBeFalse();
            }

            public void AndThenToPersistShouldBeTrue()
            {
                _event.IsPersisted.ShouldBeTrue();
            }

            public void AndThenIsNewShouldBeFalse()
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

        public class CreateThenSaveThenUpdate
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

            public void ThenToPersistShouldBeTrue()
            {
                _event.IsPersisted.ShouldBeTrue();
            }

            public void AndThenIsNewShouldBeFalse()
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

            public void AndThenIsDirtyShouldBeTrue()
            {
                _event.IsDirty.ShouldBeTrue();
            }
        }

        public class CreateThenSaveThenUpdateThenSave
        {
            private IArangoDatabase _db;
            private IEvent _event;
            private IClock _clock;

            [Fact]
            public void Execute()
            {
                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

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

        public class CreateThenSaveThenUpdateThenSaveThenUpdate
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

            public void AndWhenEventIsUpdatedAgain()
            {
                _event.Title = "yet other";
            }

            public void AndThenIsNewShouldBeFalse()
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

            public void AndThenIsDirtyShouldBeTrue()
            {
                _event.IsDirty.ShouldBeTrue();
            }
        }
    }
}
