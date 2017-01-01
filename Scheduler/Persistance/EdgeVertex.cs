using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using Scheduler.ScheduleInstances;

namespace Scheduler.Persistance
{
    public class EdgeVertex<T> where T : Vertex
    {
        public EdgeVertex(T toVertex)
        {
            ToVertex = toVertex;
        }

        public Edge Edge { get; set; }

        public T ToVertex {
            get { return (T) Edge.ToVertex; }
            set
            {
                if (Edge == null) Edge = new Edge();

                Edge.ToVertex = value;
            }
        }

        public Vertex.SaveResult Save(IArangoDatabase db, Vertex fromVertex)
        {
            return Edge.Save(db, fromVertex);
        }

        public static explicit operator EdgeVertex<T>(EdgeVertex<SingleDay> v)
        {
            return (EdgeVertex<T>)v;
        }

        public static explicit operator EdgeVertex<T>(EdgeVertex<DateList> v)
        {
            return (EdgeVertex<T>)v;
        }

        public static explicit operator EdgeVertex<T>(EdgeVertex<ByWeekday> v)
        {
            return (EdgeVertex<T>)v;
        }

        public static explicit operator EdgeVertex<T>(EdgeVertex<CompositeSchedule> v)
        {
            return (EdgeVertex<T>)v;
        }
    }
}
