namespace Scheduler.Persistance
{
    public class Link<T> : ILink<T> where T : IVertex
    {
        public T ToVertex { get; set; }

        public Link(T toVertex)
        {
            ToVertex = toVertex;
        }
    }
}
