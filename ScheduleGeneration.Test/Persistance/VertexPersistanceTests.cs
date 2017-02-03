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

                mockDb.Setup(x => x.Insert<Vertex>(
                    It.IsAny<Vertex>(),
                    It.IsAny<bool?>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((object vertex, bool? b, Action<BaseResult> a) =>
                    {
                        ((Vertex)vertex).Id = Guid.NewGuid().ToString().Substring(8);
                        ((Vertex)vertex).Key = Guid.NewGuid().ToString().Substring(8);
                        ((Vertex)vertex).Rev = Guid.NewGuid().ToString().Substring(8);
                    });

                mockDb.Setup(x => x.Remove<Vertex>(
                    It.IsAny<Vertex>(),
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

                mockDb.Setup(x => x.Insert<Vertex>(
                    It.IsAny<Vertex>(),
                    It.IsAny<bool?>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((object vertex, bool? b, Action<BaseResult> a) =>
                    {
                        ((Vertex)vertex).Id = Guid.NewGuid().ToString().Substring(8);
                        ((Vertex)vertex).Key = Guid.NewGuid().ToString().Substring(8);
                        ((Vertex)vertex).Rev = Guid.NewGuid().ToString().Substring(8);
                    });

                mockDb.Setup(x => x.Remove<Vertex>(
                    It.IsAny<Vertex>(),
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

                mockDb.Setup(x => x.Insert<Vertex>(
                    It.IsAny<Vertex>(),
                    It.IsAny<bool?>(),
                    It.IsAny<Action<BaseResult>>()))
                    .Callback((object vertex, bool? b, Action<BaseResult> a) =>
                    {
                        ((Vertex)vertex).Id = Guid.NewGuid().ToString().Substring(8);
                        ((Vertex)vertex).Key = Guid.NewGuid().ToString().Substring(8);
                        ((Vertex)vertex).Rev = Guid.NewGuid().ToString().Substring(8);
                    });

                mockDb.Setup(x => x.Remove<Vertex>(
                    It.IsAny<Vertex>(),
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
    }
}
