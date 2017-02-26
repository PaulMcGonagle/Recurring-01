using System;
using ArangoDB.Client;
using ArangoDB.Client.Data;
using Moq;
using Scheduler;
using Scheduler.Persistance;

namespace ScheduleGeneration.Test
{
    internal static class MockVertexFactory<T> where T : Vertex
    {
        public static string Id { get; } = Guid.NewGuid().ToString().Replace("-", "").Substring(8);
        public static string Rev { get; } = Guid.NewGuid().ToString().Replace("-", "").Substring(8);

        private static string Key => typeof(T).Name + "\\" + Id;

        public static Mock<IArangoDatabase> GetArangoDatabase()
        {
            var mockDb = new Mock<IArangoDatabase>();

            mockDb.Setup(x => x.Insert<T>(
                It.IsAny<T>(),
                It.IsAny<bool?>(),
                It.IsAny<Action<BaseResult>>()))
                .Callback((
                    object document,
                    bool? waitForSync,
                    Action<BaseResult> baseResult) =>
                {
                    ((T)document).Id = Id;
                    ((T)document).Key = Key;
                    ((T)document).Rev = Rev;
                })
                .Returns(new DocumentIdentifierBaseResult
                {
                    Id = Id,
                    Key = Key,
                    Rev = Rev,
                    ErrorMessage = null,
                });

            mockDb.Setup(x => x.Remove<T>(
                It.IsAny<T>(),
                It.IsAny<bool?>(),
                It.IsAny<string>(),
                It.IsAny<Action<BaseResult>>()))
                .Callback((
                    object vertex,
                    bool? waitForSync,
                    string ifMatchRev,
                    Action<BaseResult> baseResult) => { });

            mockDb.Setup(x => x.Update<T>(
                    It.IsAny<T>(),
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
                    ((T)document).Id = Id;
                    ((T)document).Key = Key;
                    ((T)document).Rev = Rev;
                });

            var mockRetrieved = new Mock<T>();

            mockRetrieved.SetupGet(x => x.Id).Returns(Id);
            mockRetrieved.SetupGet(x => x.Key).Returns(Key);
            mockRetrieved.SetupGet(x => x.Rev).Returns(Rev);

            mockDb.Setup(x => x.Document<T>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<BaseResult>>()))
                .Callback((
                    string id,
                    string ifMatchRev,
                    string ifNoneMatchRev,
                    Action<BaseResult> baseResult) =>
                { })
                .Returns(() => mockRetrieved.Object);

            var mockRetrievedEvent = new Mock<Event>();

            mockRetrievedEvent.SetupGet(x => x.Id).Returns(Id);
            mockRetrievedEvent.SetupGet(x => x.Key).Returns(Key);
            mockRetrievedEvent.SetupGet(x => x.Rev).Returns(Rev);

            mockDb.Setup(x => x.Document<Event>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<BaseResult>>()))
                .Callback((
                    string id,
                    string ifMatchRev,
                    string ifNoneMatchRev,
                    Action<BaseResult> baseResult) =>
                { })
                .Returns(() => mockRetrievedEvent.Object);

            return mockDb;
        }
    }
}
