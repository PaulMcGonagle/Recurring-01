﻿using System;
using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public class EdgeVertexs<T> : List<IEdgeVertex<T>>, IEdgeVertexs<T> where T : IVertex
    {
        public EdgeVertexs()
        {

        }

        public EdgeVertexs(IEnumerable<IEdgeVertex<T>> edgeVertexs)
            : base(edgeVertexs)
        {
            
        }

        public EdgeVertexs(T toVertex)
        {
            Add(new EdgeVertex<T>(toVertex: toVertex));
        }

        public EdgeVertexs(IEnumerable<T> toVertexs)
            : this(toVertexs.Select(t => new EdgeVertex<T>(toVertex: t)))
        {

        }

        public T Add(T item)
        {
            Add(new EdgeVertex<T>(item));

            return item;
        }

        public new void AddRange(IEnumerable<IEdgeVertex<T>> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(new EdgeVertex<T>(item));
            }
        }

        public IEnumerable<T> ToVertexs => this.Select(t => t.ToVertex);

        public void Save(IArangoDatabase db, IClock clock, IVertex fromVertex)
        {
            Save(db, clock, fromVertex, null);
        }

        public void Save(IArangoDatabase db, IClock clock, IVertex fromVertex, string label)
        {
            foreach (var edge in this)
            {
                edge.Save(db, clock, fromVertex, label);
            }

            RemoveAll(e => e.ToVertex.IsDeleted);
        }

        public void SetToDelete()
        {
            ForEach(e => e.ToVertex.SetToDelete());

        }

        public void Validate()
        {
            
        }

        public void Save(IArangoDatabase db, IClock clock)
        {
            throw new NotImplementedException();
        }
    }
}
