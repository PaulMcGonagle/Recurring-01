﻿using ArangoDB.Client;
using NodaTime;
using TechTalk.SpecRun.Helper;

namespace Scheduler.Persistance
{
    public class EdgeVertex<T> : IEdgeVertex<T> where T : IVertex
    {
        public EdgeVertex(T toVertex, string label = null)
        {
            ToVertex = toVertex;
        }

        public IEdge Edge { get; set; }

        public T ToVertex {
            get => (T) Edge.ToVertex;
            set
            {
                if (Edge == null) Edge = new Edge();

                Edge.ToVertex = value;
            }
        }

        #region Save

        public void Save(IArangoDatabase db, IClock clock, IVertex fromVertex, string label = null)
        {
            if (label != null)
                Edge.Label = label;

            Edge.Save(db, clock, fromVertex);
        }

        #endregion
    }
}
