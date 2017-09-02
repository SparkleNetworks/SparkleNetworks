
namespace Sparkle.Data.Stub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities;
    using System.Data.Objects;
    using Sparkle.Entities.Networks;

    public class StaticData
    {
        /// <summary>
        /// Lazy instantiation.
        /// </summary>
        public static StaticData Default
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _default ?? (_default = new StaticData()); }
        }
        private static StaticData _default;

        readonly List<User> people = new List<User>();
        readonly List<Company> companies = new List<Company>();
        readonly List<Job> jobs = new List<Job>();
        readonly List<SeekFriend> friends = new List<SeekFriend>();
        readonly List<string> buildings = new List<string>();

        readonly Random r = new Random();

        int _nextPersonId;

        public StaticData()
        {
            this.Init(150, 50);
        }

        private string AnyFirstname
        {
            get { return RandomData.AnyFirstname; }
        }

        private string AnyLastname
        {
            get { return RandomData.AnyLastname; }
        }

        private string AnyCity
        {
            get { return RandomData.AnyCity; }
        }

        private string AnyJobName
        {
            get { return RandomData.AnyJobName; }
        }

        private string AnyBuilding
        {
            get { return RandomData.AnyBuilding; }
        }

        private string AnyTld
        {
            get { return RandomData.AnyTld; }
        }

        private string AnyName
        {
            get
            {
                string name = string.Empty;
                for (int i = 0; i < r.Next(5, 12); i++)
                {
                    if (i % 2 == 0)
                        name += RandomData.AnyConsumn;
                    else
                        name += RandomData.AnyVoyel;
                }
                return name;
            }
        }

        private string AnyCompanyName
        {
            get
            {
                string name = string.Empty, s = string.Empty;
                for (int i = 0; i < r.Next(1, 3); i++)
                {
                    name += s + AnyName;
                    s = " ";
                }
                return name;
            }
        }

        private string AnyPhone
        {
            get
            {
                string name = "+";
                for (int i = 6; i < r.Next(0,6); i++)
                {
                    name += r.Next(0, 10);
                }
                return name;
            }
        }

        private Job AnyJob
        {
            get { return jobs[r.Next(jobs.Count)]; }
        }

        private User AnyPerson
        {
            get { return people[r.Next(people.Count)]; }
        }

        private Company AnyCompany {
            get { return companies[r.Next(companies.Count)]; }
        }

        internal List<User> People { get { return this.people; } }
        internal List<SeekFriend> Friendships { get { return this.friends; } }

        internal int NextPersonId
        {
            get { return ++this._nextPersonId; }
        }

        internal Guid NextPersonGuid
        {
            get { return GetPersonId(NextPersonId); }
        }

        internal Guid GetPersonId(int id)
        {
            return new Guid(id + 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        private void Init(int peopleNumber, int companiesNumber)
        {
            for (int i = 0; i < 6; i++)
			{
			    buildings.Add(AnyName);
			}

            int jobId = 0;
            foreach (var jobName in RandomData.AvailableJobNames)
            {
                jobs.Add(new Job
                {
                    Id = ++jobId,
                    Libelle = jobName,
                });
            }

            for (int i = 0; i < companiesNumber; i++)
			{
                string name = AnyName;
                string domain = name.ToLowerInvariant() + AnyTld;
			    companies.Add(new Company
                {
                    ////About = RandomData.Lorems,
                    Alias = i%5==0 ? AnyName : null,
                    Baseline = RandomData.Lorem,
                    ////Email = "contact@" + domain,
                    EmailDomain = domain,
                    ID = i + 1,
                    Name = name,
                    ////Phone = AnyPhone,
                    ////Website = "http://" + domain,
                });
			}

            for (int i = 0; i < peopleNumber; i++)
            {
                string first = AnyFirstname, last = AnyLastname;
                var company = AnyCompany;
                var job = AnyJob;

                people.Add(new User
                {
                    ////About = RandomData.Lorems,
                    AccountClosed = i % 20 == 0,
                    Birthday = new DateTime(1990 - i % 50),
                    ////City = AnyCity,
                    CompanyID = AnyCompany.ID,
                    Email = first + "." + last + "@" + company.EmailDomain,
                    ////FavoriteQuotes = RandomData.Lorems,
                    FirstName = first,
                    Gender = i % 2,
                    InvitationsLeft = i % 10,
                    IsDisplayWithCurrentId = i % 5 == 0,
                    IsFriendWithCurrentId = i % 3 == 0,
                    IsTeamMember = i % 4 == 0,
                    JobId = job.Id,
                    LastName = last,
                    Login = (first + "." + last).ToLower(),
                    PersonalEmail = i % 5 == 0 ? (first + "." + last + "@" + AnyName + "." + AnyTld) : null,
                    ////Phone = i % 2 == 0 ? AnyPhone : null,
                    Picture = i % 2 == 0 ? "pic" : null,
                    ////Site = i % 6 == 0 ? ("http://" + AnyName + "." + AnyTld) : null,
                    UserId = GetPersonId(NextPersonId),
                    ////ZipCode = AnyCity,
                });

            }

            foreach (var person in people.ToArray())
            {
                for (int i = 0; i < r.Next(50); i++) // 0 to 50 friends
                {
                    var any = this.AnyPerson;
                    var date = DateTime.Now.AddMonths(-r.Next(52));
                    friends.Add(new SeekFriend
                    {
                        SeekerId = person.Id,
                        TargetId = any.Id,
                        CreateDate = date,
                        ExpirationDate = date.AddMonths(1),
                        HasAccepted = r.Next(16) > 13,
                    });
                    friends.Add(new SeekFriend
                    {
                        TargetId = person.Id,
                        SeekerId = any.Id,
                        CreateDate = date,
                        ExpirationDate = date.AddMonths(1),
                        HasAccepted = r.Next(16) > 13,
                    });
                }
            }
        }
    }
}
