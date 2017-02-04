using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using ArangoDB.Client.Data;
using Castle.DynamicProxy.Generators.Emitters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Test;
using SchedulerDatabase;
using Shouldly;
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
                var mockDb = new Mock<IArangoDatabase>();

                var newId = Guid.NewGuid().ToString().Substring(8);
                var newKey = Guid.NewGuid().ToString().Substring(8);
                var newRev = Guid.NewGuid().ToString().Substring(8);

                mockDb.Setup(x => x.Insert<Event>(
                    It.IsAny<Event>(),
                    It.IsAny<bool?>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((object vertex, bool? b, Action<BaseResult> a) =>
                    {
                        ((Event)vertex).Id = Guid.NewGuid().ToString().Substring(8);
                        ((Event)vertex).Key = Guid.NewGuid().ToString().Substring(8);
                        ((Event)vertex).Rev = Guid.NewGuid().ToString().Substring(8);
                    });

                mockDb.Setup(x => x.Remove<Event>(
                    It.IsAny<Event>(),
                    It.IsAny<bool?>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((object vertex, bool? b, string s, Action<BaseResult> a) =>
                    {
                    });

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
                var mockDb = new Mock<IArangoDatabase>();

                var newId = Guid.NewGuid().ToString().Substring(8);
                var newKey = Guid.NewGuid().ToString().Substring(8);
                var newRev = Guid.NewGuid().ToString().Substring(8);

                mockDb.Setup(x => x.Insert<Event>(
                    It.IsAny<Event>(),
                    It.IsAny<bool?>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((object vertex, bool? b, Action<BaseResult> a) =>
                    {
                        ((Event)vertex).Id = Guid.NewGuid().ToString().Substring(8);
                        ((Event)vertex).Key = Guid.NewGuid().ToString().Substring(8);
                        ((Event)vertex).Rev = Guid.NewGuid().ToString().Substring(8);
                    });

                mockDb.Setup(x => x.Remove<Event>(
                    It.IsAny<Event>(),
                    It.IsAny<bool?>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((object vertex, bool? b, string s, Action<BaseResult> a) =>
                    {
                    });

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
                var mockDb = new Mock<IArangoDatabase>();

                var newId = Guid.NewGuid().ToString().Substring(8);
                var newKey = Guid.NewGuid().ToString().Substring(8);
                var newRev = Guid.NewGuid().ToString().Substring(8);

                mockDb.Setup(x => x.Insert<Event>(
                    It.IsAny<Event>(),
                    It.IsAny<bool?>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((object vertex, bool? b, Action<BaseResult> a) =>
                    {
                        ((Event)vertex).Id = Guid.NewGuid().ToString().Substring(8);
                        ((Event)vertex).Key = Guid.NewGuid().ToString().Substring(8);
                        ((Event)vertex).Rev = Guid.NewGuid().ToString().Substring(8);
                    });

                mockDb.Setup(x => x.Remove<Event>(
                    It.IsAny<Event>(),
                    It.IsAny<bool?>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((object vertex, bool? b, string s, Action<BaseResult> a) =>
                    {
                    });

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

            public void AndWhenEventIsUpdates()
            {
                _event.Title = "other";
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
            private static readonly string Id = Guid.NewGuid().ToString().Replace("-", "").Substring(8);
            private static readonly string RevInitial = Guid.NewGuid().ToString().Replace("-", "").Substring(8);
            private static readonly string RevUpdated = Guid.NewGuid().ToString().Replace("-", "").Substring(8);
            private static readonly string Key = nameof(_event) + "Type\\" + Id;

            [Fact]
            public void Execute()
            {
                var mockDb = new Mock<IArangoDatabase>();

                mockDb.Setup(x => x.Insert<Event>(
                    It.IsAny<Event>(),
                    It.IsAny<bool?>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((
                        object document, 
                        bool? waitForSync, 
                        Action<BaseResult> baseResult) =>
                    {
                        ((Event)document).Id = Id;
                        ((Event)document).Key = Key;
                        ((Event)document).Rev = RevInitial;
                    })
                    .Returns(new DocumentIdentifierBaseResult
                    {
                        Id = Id,
                        Key = Key,
                        Rev = RevInitial,
                        ErrorMessage = null,

                    });

                mockDb.Setup(x => x.Remove<Event>(
                    It.IsAny<Event>(),
                    It.IsAny<bool?>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((
                        object vertex, 
                        bool? waitForSync,
                        string ifMatchRev,
                        Action<BaseResult> baseResult) => { });

                mockDb.Setup(x => x.Update<Event>(
                        It.IsAny<Event>(),
                        It.IsAny<bool?>(),
                        It.IsAny<bool?>(),
                        It.IsAny<bool?>(),
                        It.IsAny<bool?>(),
                        It.IsAny<string>(),
                        It.IsAny<Action<BaseResult>>()))
                    .Callback((
                        object document,
                        bool? keepNull,
                        bool? mergeObjects,
                        bool? waitForSync,
                        bool? ignoreRevs,
                        string ifMatchRev,
                        Action<BaseResult> baseResult) =>
                    {
                        ((Event)document).Id = Id;
                        ((Event)document).Key = Key;
                        ((Event)document).Rev = RevUpdated;
                    });

                var mockRetrieved = new Mock<Event>();

                mockRetrieved.SetupGet(x => x.Id).Returns(Id);
                mockRetrieved.SetupGet(x => x.Key).Returns(Key);
                mockRetrieved.SetupGet(x => x.Rev).Returns(RevInitial);

                mockDb.Setup(x => x.Document<Event>(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Action<BaseResult>>()))
                    .Callback((
                        string id,
                        string ifMatchRev,
                        string ifNoneMatchRev,
                        Action<BaseResult> baseResult) => { })
                    .Returns(() => mockRetrieved.Object);

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
    }
}
