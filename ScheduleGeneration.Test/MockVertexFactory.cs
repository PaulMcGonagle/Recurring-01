using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using ArangoDB.Client.Data;
using Moq;
using Scheduler;
using Scheduler.Persistance;

namespace ScheduleGeneration.Test
{
    internal static class MockVertexFactory<T> where T : Vertex
    {
        private static readonly string Id = Guid.NewGuid().ToString().Replace("-", "").Substring(8);
        private static readonly string RevInitial = Guid.NewGuid().ToString().Replace("-", "").Substring(8);
        private static readonly string RevUpdated = Guid.NewGuid().ToString().Replace("-", "").Substring(8);
        private static readonly string Key = "Vertex\\" + Id;

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
                    ((T)document).Rev = RevInitial;
                })
                .Returns(new DocumentIdentifierBaseResult
                {
                    Id = Id,
                    Key = Key,
                    Rev = RevInitial,
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
                    ((T)document).Rev = RevUpdated;
                });

            var mockRetrieved = new Mock<T>();

            mockRetrieved.SetupGet(x => x.Id).Returns(Id);
            mockRetrieved.SetupGet(x => x.Key).Returns(Key);
            mockRetrieved.SetupGet(x => x.Rev).Returns(RevInitial);

            mockDb.Setup(x => x.Document<T>(
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

            return mockDb;
        }
    }
}
