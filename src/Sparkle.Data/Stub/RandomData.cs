
namespace Sparkle.Data.Stub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class RandomData
    {
        static readonly char[] consumns = new char[] { 'z', 'r', 't', 'p', 'q', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'w', 'x', 'c', 'v', 'b', 'n' };
        static readonly char[] voyels = new char[] { 'a', 'e', 'y', 'u', 'i', 'o' };

        static readonly string lorem = @"Lorem ipsum dolor sit amet, condimentum ultricies faucibus nulla justo mauris, proin auctor quisque. Ante nullam feugiat vitae nulla dolor morbi, ac volutpat et enim suspendisse, varius amet, voluptatem etiam mus eget nulla arcu. Sit ac sit suspendisse ultricies at risus, ut nulla turpis arcu mus elit arcu, nostra duis eu erat. Imperdiet et nulla mi nisl, et tincidunt, consectetuer pellentesque natoque amet vestibulum rhoncus. Venenatis est mi tristique massa, vitae facilisis odio turpis proin, orci reiciendis. A non erat, id ad aliquet, ut potenti libero quis nihil metus mauris, proin feugiat eget accumsan euismod, velit congue morbi. Ullamcorper viverra at, sit nullam placerat leo a, dui mauris lobortis praesent, posuere ligula, auctor pede cras. Fusce tincidunt, a augue sed nostra, ipsum amet eleifend. Auctor nulla libero adipiscing, augue metus mauris pharetra felis. Lectus amet, fermentum lacus, mi duis eleifend lectus nam.";
        static readonly string lorems = @"Lorem ipsum dolor sit amet, condimentum ultricies faucibus nulla justo mauris, proin auctor quisque. Ante nullam feugiat vitae nulla dolor morbi, ac volutpat et enim suspendisse, varius amet, voluptatem etiam mus eget nulla arcu. " + Environment.NewLine + Environment.NewLine + @"Sit ac sit suspendisse ultricies at risus, ut nulla turpis arcu mus elit arcu, nostra duis eu erat. Imperdiet et nulla mi nisl, et tincidunt, consectetuer pellentesque natoque amet vestibulum rhoncus. Venenatis est mi tristique massa, vitae facilisis odio turpis proin, orci reiciendis. A non erat, id ad aliquet, ut potenti libero quis nihil metus mauris, proin feugiat eget accumsan euismod, velit congue morbi. " + Environment.NewLine + Environment.NewLine + @"Ullamcorper viverra at, sit nullam placerat leo a, dui mauris lobortis praesent, posuere ligula, auctor pede cras. Fusce tincidunt, a augue sed nostra, ipsum amet eleifend. Auctor nulla libero adipiscing, augue metus mauris pharetra felis. Lectus amet, fermentum lacus, mi duis eleifend lectus nam.";
        static readonly string ipsus = @"Arcu velit urna, integer wisi sodales sit faucibus dignissim vel, mus augue vitae turpis et fusce, sed saepe augue tellus aliquam lorem tempor, porta in rutrum. Dolor ac ante consequat, pellentesque proin sem diam, nunc arcu in cras nibh, nonummy rutrum dis quis lobortis pellentesque pede, lacus ornare.";

        static readonly Random r = new Random();
        static readonly List<string> firstNames = new List<string>();
        static readonly List<string> lastNames = new List<string>();
        static readonly List<string> cities = new List<string>();
        static readonly List<string> buildings = new List<string>();
        static readonly List<string> tlds = new List<string>();
        static readonly List<string> jobNames = new List<string>();

        static RandomData()
        {
            tlds.AddRange(new string[]{ ".com", ".net", ".fr", ".eu", ".biz", ".mobi", ".de", ".co.uk", ".info", ".me" });
            firstNames.AddRange(new string[] { "Antoine", "Amélie", "Sandra", "Anna", "Pedro", "Ludovic", "Kévin", "Ulicia", "Armina", "Richard", "Zedd", "Yuki", "Michel", "Jean-Yvers", "Luc", "Dmitri", "Gregory", "Lucien", "François", "Pierre", "Mathias", "Mathis", "Sébastien", "Véronique", "Sandrine", "Lydia", "Céline", "Cécile", "Lucile", "Anabelle", "Charlotte", "Pascal", "Dominique", "Manuel", "Manuelle", "André", "Daniel", "David", "Etienne", "Hugues", "Isaac", "Grégoire", "Julien", "Léon", "Louis", "Lucas", "Marc", "Vincent", "Xavier", "Anne", "Aurélie", "Aurore", "Christelle", "Clémence", "Danielle", "Diane", "Gabrielle", "Jeanne", "Julie", "Juliette", "Laury", "Martine", "Nathalie", "Gemako", "Odette", "Sabine", "Michel", "Luc", "Sabri", "Amandine", "Jules", "Rémi", "Remmy", "Louis", "Jean-Louis", "Jean-Micheng", "Jean-Michel", "Jean-Albert", "Albert-Jean", "Alban", "Allo", "Nah", "Kristeva", "Nikolev", "Nico", "Bruno", "Argo", "Margo", "Miguel", "Paolo", "Thierry", "Henry", "Patrick", "Patoche", "Phillipe", "Hugo", "Kevin", "Apolline", "Marianne", "Anna", "Alice", "Amanda", "Amande", "Sabine", "Céline", "Corinne", "Alphonse", "Sophie", });
            lastNames.AddRange(new string[] { "Dupont", "Dupond", "Duhamel", "Dubuisson", "Duchef", "Duruisseau", "Duchantier", "Dubatiment", "Dudur", "Dupaté", "Dusaucisson", "Durand", "Durée", "Duflan", "Dupontlevis", "Duresque", "Durlot", "Dubrifiant", "Dumachin", "Dulac", "Ducontinent", "Dupays", "Durable", "Dulong", "Ducourt", "Durance", "Dumestique", "Dulit", "Duvergé", "Duchemin", "Durluberlu", "Dududududu", "Dumarché", "Duclergé", "Duvergé", "Durouge", "Duvert", "Dubleu", "Dujaune", "Duorange", "Duo", "Maille", "Mailliet", "Lehaut", "Lemilieu", "Gorce", "Lorthoy", "Cogez", "Tomczyk", "Duprez", "Wozniak", "Mauvaisvoisin", });
            cities.AddRange(new string[] { "Lille", "Roubaix", "Villeneuve", "Amiens", "Paris", "Lille", "Valenciennes", "Douai", "Lille", "Lille", "Lambersart", "Lomme", "Feignies", "Maubeuge", "Bruxelles", "Douai", "Reins", "Amiens", "Bordeaux", "Dunkerque", "Calais", "Brest", "Lyon", "Dinant", "Roubaix", "Wambrechies", "New-York City", "London", "Versailles", "Mons", });
            jobNames.AddRange(new string[] { "Développeur", "Designer", "Webdesigner", "Metteur en scène", "Chef de projet ", "Leader technique", "Consultant", "Directeur", "CIO", "CTO", "Développeur", "Développeur", "Designer", "RH", "Comptable", "Contrôlleur de gestion", "Community manager", "Stagiaire", "CEO", "Chef de produit", "Commercial", "Fleuriste", "Dessinateur", "Animateur", "Journaliste", "Autoentrepreuneur", });
        }

        public static string AnyJobName
        {
            get { return jobNames[r.Next(jobNames.Count)]; }
        }

        public static string AnyTld
        {
            get { return tlds[r.Next(tlds.Count)]; }
        }

        public static string AnyConsumn
        {
            get { return consumns[r.Next(consumns.Length)].ToString(); }
        }

        public static string AnyVoyel
        {
            get { return voyels[r.Next(voyels.Length)].ToString(); }
        }

        public static string AnyName
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

        public static string AnyCompanyName
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

        public static string AnyPhone
        {
            get
            {
                string name = "+";
                for (int i = 6; i < r.Next(0, 6); i++)
                {
                    name += r.Next(0, 10);
                }
                return name;
            }
        }

        public static string AnyFirstname
        {
            get { return firstNames[r.Next(firstNames.Count)]; }
        }

        public static string AnyLastname
        {
            get { return lastNames[r.Next(lastNames.Count)]; }
        }

        public static string AnyCity
        {
            get { return cities[r.Next(cities.Count)]; }
        }

        public static string AnyBuilding
        {
            get { return buildings[r.Next(buildings.Count)]; }
        }

        public static IList<string> AvailableJobNames
        {
            get { return jobNames.ToList(); }
        }

        public static string Lorem
        {
            get { return lorem; }
        }

        public static string Lorems
        {
            get { return lorems; }
        }

        public static string Ipsum
        {
            get { return ipsus; }
        }

        public static DateTime AnyPastDateTime(int yearsRange)
        {
            return DateTime.Now
                .AddYears(-r.Next(yearsRange))
                .AddMonths(-r.Next(13))
                .AddDays(-r.Next(30))
                .AddHours(-r.Next(24));
        }
    }
}
