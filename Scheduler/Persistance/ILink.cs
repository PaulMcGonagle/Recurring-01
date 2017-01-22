namespace Scheduler.Persistance
{
    public interface ILink<T> where T : IVertex
    {
        T ToVertex { get; set; }
    }
}