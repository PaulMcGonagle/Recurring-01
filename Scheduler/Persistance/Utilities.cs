using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;

namespace Scheduler.Persistance
{
    public static class Utilities
    {
        public static T GetByExternalId<T>(IArangoDatabase db, string id) where T : IVertex
        {
            var result = db.Query<T>()
                .For(t => db.Query<Edge>()
                    .For(ed => db.Query<ExternalId>()
                        .Where(ex => ed.FromId == ex.Id && ed.ToId == t.Id && ex.Uid == id)
                        .Select(ex => t)))
                .SingleOrDefault();

            return result;
        }

        public static IEnumerable<T> GetByFromId<T>(IArangoDatabase db, string fromId, string label = null) where T : IVertex
        {
            var result = db.Query<T>()
                .For(t => db.Query<Edge>()
                    .Where(
                        ed => ed.FromId == fromId 
                        && ed.ToId == t.Id
                        && (ed.Label == label || label == null))
                    .Select(ex => t))
                .ToList();

            return result;
        }

        public static IEnumerable<T> GetByToId<T>(IArangoDatabase db, string toId, string label = null) where T : IVertex
        {
            var result = db.Query<T>()
                .For(t => db.Query<Edge>()
                    .Where(
                        ed => ed.ToId == toId
                        && ed.FromId == t.Id 
                        && (ed.Label == label || label == null))
                    .Select(ex => t))
                .ToList();

            return result;
        }
    }
}
