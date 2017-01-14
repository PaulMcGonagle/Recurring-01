using Scheduler.Persistance;

namespace Scheduler.Users
{
    public interface IUser : IVertex
    {
        string Forename { get; set; }
        string Surname { get; set; }
    }
}