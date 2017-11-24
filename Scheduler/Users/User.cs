using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class User : Vertex, IUser
    {
        public string Forename { get; set; }
        public string Surname { get; set; }

        public void Validate()
        {
            Guard.AgainstNullOrWhiteSpace(Forename, nameof(Forename));
            Guard.AgainstNullOrWhiteSpace(Surname, nameof(Surname));
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<User>(db);
            base.Save(db, clock);
        }
    }

    public class UserBuilder
    {
        private readonly User _user;

        public UserBuilder()
        {
            _user = new User();
        }

        public string Forename
        {
            set => _user.Forename = value;
        }

        public string Surname
        {
            set => _user.Surname = value;
        }

        public IUser Build()
        {
            return _user;
        }
    }
}
