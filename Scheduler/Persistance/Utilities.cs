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

        public static IEnumerable<T> GetEdges<T>(IArangoDatabase db, string fromId = null, string label = null, string toId = null) where T : IVertex
        {
            var result = db.Query<Edge>()
                .For(ed => db.Query<T>()
                    .Where(
                        t => t.Id == ed.ToId
                            && (fromId == null || ed.FromId == fromId)
                            && (toId == null || ed.ToId == toId)
                            && (label == null || ed.Label == label))
                    .Select(t => t))
                .ToList();

            return result;
        }

        public static IEnumerable<T> GetByTag<T>(this IEnumerable<T> vertexs, string ident = null, string value = null) where T : IVertex
        {
            return vertexs
                .Where(vertex => vertex
                    .Tags
                    .Any(tag => (tag.ToVertex.Ident == ident || ident == null) && (tag.ToVertex.Value == value || value == null)));
        }

        public static string GetTagValue(this IVertex vertex, string ident)
        {
            return vertex
                .Tags
                .GetToVertexs()
                .SingleOrDefault(tag => tag.Ident == ident)
                ?.Value;
        }

        public static IEnumerable<T> GetToVertexs<T>(this IEnumerable<IEdgeVertex<T>> edgeVertexs) where T : IVertex
        {
            return edgeVertexs
                .Select(ev => ev.ToVertex);
        }
    }
}
