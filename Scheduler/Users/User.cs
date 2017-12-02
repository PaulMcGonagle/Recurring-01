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

        public override void Validate()
        {
            Guard.AgainstNullOrWhiteSpace(Forename, nameof(Forename));
            Guard.AgainstNullOrWhiteSpace(Surname, nameof(Surname));
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<User>(db);
            base.Save(db, clock);
        }

        public class Builder : Builder<User>
        {
            public string Forename
            {
                set => _target.Forename = value;
            }

            public string Surname
            {
                set => _target.Surname = value;
            }
        }
    }}
