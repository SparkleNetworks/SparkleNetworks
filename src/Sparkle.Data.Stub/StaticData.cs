
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

        readonly char[] consumns = new char[] { 'z', 'r', 't', 'p', 'q', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'w', 'x', 'c', 'v', 'b', 'n' };
        readonly char[] voyels = new char[] { 'a', 'e', 'y', 'u', 'i', 'o' };

        readonly string lorem = @"Lorem ipsum dolor sit amet, condimentum ultricies faucibus nulla justo mauris, proin auctor quisque. Ante nullam feugiat vitae nulla dolor morbi, ac volutpat et enim suspendisse, varius amet, voluptatem etiam mus eget nulla arcu. Sit ac sit suspendisse ultricies at risus, ut nulla turpis arcu mus elit arcu, nostra duis eu erat. Imperdiet et nulla mi nisl, et tincidunt, consectetuer pellentesque natoque amet vestibulum rhoncus. Venenatis est mi tristique massa, vitae facilisis odio turpis proin, orci reiciendis. A non erat, id ad aliquet, ut potenti libero quis nihil metus mauris, proin feugiat eget accumsan euismod, velit congue morbi. Ullamcorper viverra at, sit nullam placerat leo a, dui mauris lobortis praesent, posuere ligula, auctor pede cras. Fusce tincidunt, a augue sed nostra, ipsum amet eleifend. Auctor nulla libero adipiscing, augue metus mauris pharetra felis. Lectus amet, fermentum lacus, mi duis eleifend lectus nam.";
        readonly string lorems = @"Lorem ipsum dolor sit amet, condimentum ultricies faucibus nulla justo mauris, proin auctor quisque. Ante nullam feugiat vitae nulla dolor morbi, ac volutpat et enim suspendisse, varius amet, voluptatem etiam mus eget nulla arcu. "+Environment.NewLine+Environment.NewLine+@"Sit ac sit suspendisse ultricies at risus, ut nulla turpis arcu mus elit arcu, nostra duis eu erat. Imperdiet et nulla mi nisl, et tincidunt, consectetuer pellentesque natoque amet vestibulum rhoncus. Venenatis est mi tristique massa, vitae facilisis odio turpis proin, orci reiciendis. A non erat, id ad aliquet, ut potenti libero quis nihil metus mauris, proin feugiat eget accumsan euismod, velit congue morbi. "+Environment.NewLine+Environment.NewLine+@"Ullamcorper viverra at, sit nullam placerat leo a, dui mauris lobortis praesent, posuere ligula, auctor pede cras. Fusce tincidunt, a augue sed nostra, ipsum amet eleifend. Auctor nulla libero adipiscing, augue metus mauris pharetra felis. Lectus amet, fermentum lacus, mi duis eleifend lectus nam.";
        readonly string ipsus = @"Arcu velit urna, integer wisi sodales sit faucibus dignissim vel, mus augue vitae turpis et fusce, sed saepe augue tellus aliquam lorem tempor, porta in rutrum. Dolor ac ante consequat, pellentesque proin sem diam, nunc arcu in cras nibh, nonummy rutrum dis quis lobortis pellentesque pede, lacus ornare.";

        readonly List<string> firstNames = new List<string>();
        readonly List<string> lastNames = new List<string>();
        readonly List<string> cities = new List<string>();
        readonly List<string> buildings = new List<string>();
        readonly List<string> tlds = new List<string>();
        readonly List<string> jobNames = new List<string>();

        readonly List<User> people = new List<User>();
        readonly List<Company> companies = new List<Company>();
        readonly List<Job> jobs = new List<Job>();
        readonly List<SeekFriend> friends = new List<SeekFriend>();

        readonly Random r = new Random();

        int _nextPersonId;

        public StaticData()
        {
            this.Init(150, 50);
        }

        private string AnyFirstname
        {
            get { return firstNames[r.Next(firstNames.Count)]; }
        }

        private string AnyLastname
        {
            get { return lastNames[r.Next(lastNames.Count)]; }
        }

        private string AnyCity
        {
            get { return cities[r.Next(cities.Count)]; }
        }

        private string AnyJobName
        {
            get { return jobNames[r.Next(jobNames.Count)]; }
        }

        private string AnyBuilding
        {
            get { return buildings[r.Next(buildings.Count)]; }
        }

        private string AnyTld
        {
            get { return tlds[r.Next(tlds.Count)]; }
        }

        private string AnyName
        {
            get
            {
                string name = string.Empty;
                for (int i = 0; i < r.Next(5, 12); i++)
                {
                    if (i % 2 == 0)
                        name += consumns[r.Next(consumns.Length)];
                    else
                        name += voyels[r.Next(voyels.Length)];
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
            tlds.AddRange(new string[]{ ".com", ".net", ".fr", ".eu", ".biz", ".mobi", ".de", ".co.uk", ".info", ".me" });
            firstNames.AddRange(new string[] { "Antoine", "Amélie", "Sandra", "Anna", "Pedro", "Ludovic", "Kévin", "Ulicia", "Armina", "Richard", "Zedd", "Yuki", "Michel", "Jean-Yvers", "Luc", "Dmitri", "Gregory", "Lucien", "François", "Pierre", "Mathias", "Mathis", "Sébastien", "Véronique", "Sandrine", "Lydia", "Céline", "Cécile", "Lucile", "Anabelle", "Charlotte", "Pascal", "Dominique", "Manuel", "Manuelle", "André", "Daniel", "David", "Etienne", "Hugues", "Isaac", "Grégoire", "Julien", "Léon", "Louis", "Lucas", "Marc", "Vincent", "Xavier", "Anne", "Aurélie", "Aurore", "Christelle", "Clémence", "Danielle", "Diane", "Gabrielle", "Jeanne", "Julie", "Juliette", "Laury", "Martine", "Nathalie", "Gemako", "Odette", "Sabine" });
            lastNames.AddRange(new string[] { "Dupont", "Dupond", "Duhamel", "Dubuisson", "Duchef", "Duruisseau", "Duchantier", "Dubatiment", "Dudur", "Dupaté", "Dusaucisson", "Durand", "Durée", "Duflan", "Dupontlevis", "Duresque", "Durlot", "Dubrifiant", "Dumachin", "Dulac", "Ducontinent", "Dupays", "Durable", "Dulong", "Ducourt", "Durance", "Dumestique", "Dulit", "Duvergé", "Duchemin", "Durluberlu", "Dududududu", "Dumarché", "Duclergé", "Duvergé", "Durouge", "Duvert", "Dubleu", "Dujaune", "Duorange", "Duo" });
            cities.AddRange(new string[] { "Lille", "Roubaix", "Villeneuve", "Amiens", "Paris", "Lille", "Valenciennes", "Douai", "Lille", "Lille", "Lambersart", "Lomme" });
            jobNames.AddRange(new string[] { "Développeur", "Designer", "Webdesigner", "Metteur en scène", "Chef de projet ", "Leader technique", "Consultant", "Directeur", "CIO", "CTO", "Développeur", "Développeur", "Designer", "RH", "Comptable", "Contrôlleur de gestion", "Community manager", "Stagiaire", "CEO", "Chef de produit", "Commercial", "Fleuriste", "Dessinateur", "Animateur", "Journaliste", "Autoentrepreuneur" });

            for (int i = 0; i < 6; i++)
			{
			    buildings.Add(AnyName);
			}

            int jobId = 0;
            foreach (var jobName in jobNames)
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
                    About = lorems,
                    Access = null,
                    Alias = i%5==0 ? AnyName : null,
                    Baseline = lorem,
                    Building = AnyBuilding,
                    Email = "contact@" + domain,
                    EmailDomain = domain,
                    Floor = null,
                    ID = i + 1,
                    Name = name,
                    Phone = AnyPhone,
                    Website = "http://" + domain,
                });
			}

            for (int i = 0; i < peopleNumber; i++)
            {
                string first = AnyFirstname, last = AnyLastname;
                var company = AnyCompany;
                var job = AnyJob;

                people.Add(new User
                {
                    About = lorems,
                    AccountClosed = i%20 ==0,
                    Birthday = new DateTime(1990 - i % 50),
                    City = AnyCity,
                    CompanyID = AnyCompany.ID,
                    Email = first + "." + last + "@" + company.EmailDomain,
                    FavoriteQuotes = lorems,
                    FirstName = first,
                    Gender = i%2,
                    InvitationsLeft = i%10,
                    IsDisplayWithCurrentId = i%5 == 0,
                    IsFriendWithCurrentId = i%3 == 0,
                    IsTeamMember = i%4==0,
                    JobId = job.Id,
                    LastName = last,
                    Login = (first + "." + last).ToLower(),
                    PersonalEmail = i%5== 0 ? (first + "." + last + "@" + AnyName + "." + AnyTld) : null,
                    Phone = i%2 == 0 ? AnyPhone : null,
                    Picture = i%2 == 0 ? "pic" : null,
                    Site = i%6==0 ? ("http://" + AnyName + "." + AnyTld) : null,
                    Twitter = i%3==0 ? AnyName : null,
                    UserId = GetPersonId(NextPersonId),
                    ZipCode = AnyCity,
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
