
namespace Sparkle.Commands.Main
{
    using Ionic.Zip;
    using MySql.Data.MySqlClient;
    using Newtonsoft.Json;
    using Sparkle.Commands.Main.Internals;
    using Sparkle.Common.CommandLine;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Configuration.Provider;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Policy;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Security;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Entities = Sparkle.Entities.Networks;

    public class GoogleGroupImport : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        // Init command
        private string remoteClient;
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.None,
        };

        public override void RunUniverse(SparkleCommandArgs args)
        {
            var context = new Context();
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;
            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "GoogleGroupImport";
            services.HostingEnvironment.RemoteClient = this.remoteClient;
            this.Out.WriteLine();
            context.Services = services;

            // network
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("GoogleGroupImport", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return;
            }

            // simulation: not supported
            if (args.Simulation)
            {
                this.Out.WriteLine("Simulation is not supported; not runnin'");
                args.SysLogger.Info("GoogleGroupImport", remoteClient, Environment.UserName, ErrorLevel.Success, "Simulation is not supported; not running.");
                return;
            }

            services.NetworkId = network.Id;

            var myArgs = args.Arguments.SkipWhile(a => a.ToLowerInvariant() != this.GetType().Name.ToLowerInvariant()).ToArray();
            if (myArgs.Length != 4)
            {
                this.Error.WriteLine("Usage: GoogleGroupImport trafic.saz ninja.html userId");
                return;
            }

            if (!File.Exists(myArgs[1]) || !File.Exists(myArgs[2]))
            {
                this.Error.WriteLine("File doesn't exist");
                return;
            }

            int userId;
            User user;
            if (int.TryParse(myArgs[3], out userId))
            {
                user = context.Services.People.SelectWithId(userId);
            }
            else
            {
                user = context.Services.People.SelectWithLogin(myArgs[3]);
            }

            if (user == null)
            {
                this.Error.WriteLine("User doesn't exist");
                return;
            }

            context.MessageOwner = user;
            this.context = context;
            MakeDataParsingFromGoogleGroup(myArgs);
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle GoogleGroupImport
    Import data from GoogleGroups exports.
";

            registry.Register(
                "GoogleGroupImport",
                "Import data from GoogleGroups exports.",
                () => new GoogleGroupImport(),
                longDesc);
        }
        
        // Import google group topics
        static Regex tagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>"); // <zoefijze/>
        static Regex tagAloneRegex = new Regex(@"</?[^>]+/?>");
        static Regex quoteRegex = new Regex(@"[0-9]{4}.+[0-9]{2}.+@.+[:]", RegexOptions.Singleline);
        static Regex quoteRegex4 = new Regex(@"[0-9]{2}.+[0-9]{4}.+@.+[:]", RegexOptions.Singleline);
        static Regex quoteRegex2 = new Regex(@"(Lundi|Mardi|Mercredi|Jeudi|Vendredi|Samedi|Dimanche|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday).*[0-9]{4}.*:.{,3}$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        static Regex quoteRegex3 = new Regex(@"[0-9]{,2}/[0-9]{,2}/[0-9]{,2}.*:.{,3}", RegexOptions.Singleline);
        static Regex mail64Regex = new Regex(@"[a-zA-Z0-9_-]+@.+\..+");

        IList<GoogleGroupTopic> topicList = new List<GoogleGroupTopic>();
        private Context context;

        private bool IsEntrySubjectResponse(string entry)
        {
            return entry.StartsWith("Re: ");
        }

        private bool IsEntryUrl(string entry)
        {
            return entry.StartsWith("http");
        }

        private bool IsEntryOnlyDigit(string entry)
        {
            foreach (var c in entry)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }

        private bool IsEntryHtml(string entry)
        {
            if (tagRegex.IsMatch(entry) || tagAloneRegex.IsMatch(entry))
                return true;
            return false;
        }

        private IList<string> RemoveUselessHtml(IList<string> data)
        {
            bool previousIsHtml = false;
            IList<string> ret = new List<string>(data);
            foreach (var entry in data)
            {
                if (previousIsHtml)
                {
                    if (IsEntryHtml(entry))
                        ret.Remove(entry);
                    else
                        previousIsHtml = false;
                }
                else if (IsEntryHtml(entry))
                    previousIsHtml = true;
            }
            return ret;
        }

        private bool IsHashAfterAuthor(string entry)
        {
            if (entry.Length != 12)
                return false;
            if (entry.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').Count() != 12)
                return false;
            return true;
        }

        private bool IsTopicId(string entry)
        {
            if (entry.Length != 11)
                return false;
            if (entry.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').Count() != 11)
                return false;
            return true;
        }

        public class RequestResponseEntry
        {
            public ZipEntry Request { get; set; }
            public ZipEntry Response { get; set; }
        }

        private IList<RequestResponseEntry> GetWantedFilenames(ZipFile zip)
        {
            IList<RequestResponseEntry> luffy = new List<RequestResponseEntry>();
            int i = 0;
            foreach (var item in zip.Where(s => s.FileName.EndsWith("_c.txt")))
            {
                if (++i % 100 == 0)
                    Console.WriteLine("Checking trafic file " + i + " / " + zip.Where(s => s.FileName.EndsWith("_c.txt")).Count() + "...");

                var stream = item.OpenReader();
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var line = reader.ReadLine();
                    if (line.StartsWith("POST https://groups.google.com/forum/msg_bkg?") || line.StartsWith("POST https://groups.google.com/forum/msg?"))
                    {
                        var filename = item.FileName.Split(new char[] { '/' })[1];
                        var response = zip
                            .Where(o => o.FileName.Contains(filename.Split(new char[] { '_' })[0] + "_s.txt"))
                            .SingleOrDefault();
                        var entry = new RequestResponseEntry
                        {
                            Request = item,
                            Response = response,
                        };
                        luffy.Add(entry);
                    }
                }
            }

            return luffy;
        }

        private string GetSubjectName(IList<string> data)
        {
            bool takeNext = false;
            for (int i = 0; i < data.Count; i++)
            {
                if (takeNext)
                    return data[i];
                if ((data[i].EndsWith("@mail.gmail.com") || data[i].EndsWith("@gmail.com"))
                    && data[i + 1] != null && !data[i + 1].EndsWith("@mail.gmail.com") && !data[i + 1].EndsWith("@gmail.com"))
                    takeNext = true;
            }
            return null;
        }

        private bool IsTopicAlone(IList<string> data)
        {
            for (int i = 0; i < data.Count - 2; i++)
            {
                if (data[i] == "moi")
                {
                    if (IsTopicId(data[i + 2]))
                        return false;
                    else
                        break;
                }
            }
            return true;
        }

        private GoogleGroupTopic GetGoogleGroupTopic(RequestResponseEntry item, IList<string> data, IDictionary<string, TopicInfo> topicIds)
        {
            var toRet = new GoogleGroupTopic
                {
                    Origin = item.Response,
                    FileData = data,
                };
            toRet.Messages = new List<GoogleGroupMessage>();
            string firstTopic = null;
            int i;
            for (i = 0; i < data.Count - 1; i++)
            {
                if (data[i] == "moi")
                {
                    firstTopic = data[i + 1];
                    i += 2;
                    break;
                }
            }

            if (topicIds.Where(o => o.Key == firstTopic).ToList().Count <= 0)
                return null;

            var theTopic = topicIds.ContainsKey(firstTopic) ? topicIds[firstTopic] : null;
            ////toRet.Subject =      topicIds.Where(o => o.Key == firstTopic).Select(o => o.Value.Title).SingleOrDefault();
            ////toRet.OriginId =     topicIds.Where(o => o.Key == firstTopic).Select(o => o.Key).Single();
            ////toRet.LastModified = topicIds.Where(o => o.Key == firstTopic).Select(o => o.Value.LastUpdatedDate).Single();
            toRet.Subject =      theTopic != null ? theTopic.Title : null;
            toRet.OriginId =     firstTopic;
            toRet.LastModified = theTopic.LastUpdatedDate;
            toRet.Url = theTopic.Url;

            var msgTmp = data[i];
            string prev = "";
            for (int j = i; j < data.Count; j++)
            {
                if (IsHashAfterAuthor(data[j]) && !msgTmp.StartsWith("APn2wQ") && !msgTmp.StartsWith("~APn2wQ"))
                {
                    var toAdd = new GoogleGroupMessage
                        {
                            Author = data[j - 1],
                            //Message = UserProfile.CleanHtml(),
                            Message = GetCleanMessage(msgTmp),
                            oMessage = msgTmp,
                        };
                    if (string.IsNullOrEmpty(toAdd.Message))
                    {
                        toAdd.Message = GetCleanMessage(prev);
                        toAdd.oMessage = prev;
                    }
                    

                    toRet.Messages.Add(toAdd);
                    msgTmp = "APn2wQ";
                }
                else if (IsEntryHtml(data[j]))
                {
                    prev = msgTmp;
                    msgTmp = data[j];
                }
            }

            return toRet;
        }

        private static Regex sigImageLinksRegex = new Regex(@"\[\(image\)\]\((.*?)\)");
        private static Regex sigEmailLinksRegex = new Regex(@"\[(.+?@.+?)\]\((http://)javascript:?\)"); // [jul...@wepopp.com](http://javascript:)
        private static string[] truncateInCleanMessage = new string[]
        {
            "(image)(image)",
            "****",
        };
        public static string GetCleanMessage(string html)
        {
            // make clean line breaks
            var withLineBreaks = (html ?? "")
                .Replace("\\n", "\n")
                .Replace("<br>", "\n")
                .Split(new string[] { "\n" }, StringSplitOptions.None)
                .Select(r => r.Trim())
                .ToArray();
            int takeLines = withLineBreaks.Length;
            for (int i = 0; i < withLineBreaks.Length; i++)
            {
                var line = withLineBreaks[withLineBreaks.Length - i - 1];
                bool isQuote = quoteRegex.IsMatch(line) || quoteRegex2.IsMatch(line) || quoteRegex3.IsMatch(line) || quoteRegex4.IsMatch(line);
                if (isQuote)
                {
                    if (i == 0)
                        takeLines = withLineBreaks.Length - 2;
                    else
                        takeLines = withLineBreaks.Length - i - 1;
                    break;
                }
            }

            var linedHtml = string.Join("\n", withLineBreaks.Take(takeLines + 1));

            // handle signature
            var noSignatureSplit = linedHtml.Split(new string[] { "-- \n", }, StringSplitOptions.None);
            var elements = new string[noSignatureSplit.Length];
            for (int i = 0; i < noSignatureSplit.Length; i++)
           { 
                bool allowBold = true, allowItalics = false;
                if ((i + 1) == noSignatureSplit.Length && noSignatureSplit.Length > 1)
                {
                    // signature
                    allowBold = allowItalics = false;
                }

                var element = SuperNinjaCleanHtmlFromDeath(
                    string.Join("\n", noSignatureSplit[i]
                        .Replace("\\n", "\n")
                        .Replace("<br>", "\n")
                        .Split(new string[] { "\n" }, StringSplitOptions.None)
                        .TakeWhile(s =>
                            !quoteRegex.IsMatch(s) && !quoteRegex2.IsMatch(s) &&
                            !quoteRegex3.IsMatch(s) && !quoteRegex4.IsMatch(s))
                    .Select(r => r.Trim())), allowBold, allowItalics);

                elements[i] = element;
            }

            var noHtml = string.Join("\n", elements);

            // special cases
            noHtml = HttpUtility.HtmlDecode(noHtml);

            noHtml = sigImageLinksRegex.Replace(noHtml, new MatchEvaluator(match =>
            {
                return "[" + match.Groups[1].Captures[0].Value + "](" + match.Groups[1].Captures[0].Value + ")";
            }));

            noHtml = sigEmailLinksRegex.Replace(noHtml, new MatchEvaluator(match =>
            {
                return match.Groups[1].Captures[0].Value;
            }));

            for (int i = 0; i < truncateInCleanMessage.Length; i++)
            {
                noHtml = noHtml.Replace(truncateInCleanMessage[i], "");
            }

            while (noHtml.Contains("\n\n\n"))
            {
                noHtml = noHtml.Replace("\n\n\n", "\n\n");
            }

            return noHtml.Trim();
        }

        private static string SuperNinjaCleanHtmlFromDeath(string html, bool allowBold, bool allowItalics)
        {
            var lessHtml = UserProfile.CleanHtml(html, allowBold: allowBold, allowItalics: allowItalics);
            var noHtml = tagAloneRegex.Replace(lessHtml, "");
            noHtml = tagRegex.Replace(noHtml, "");

            noHtml = string.Join(
                "\n",
                noHtml.Split(new string[] { "\n", }, StringSplitOptions.None)
                .Select(r => r.Replace("\\t", "\t")
                    .Trim()));
            return noHtml;
        }

        private void GetGoogleGroupTopicNotAlone(RequestResponseEntry item, IList<string> data, IDictionary<string, TopicInfo> topicIds)
        {
            var toRet = new GoogleGroupTopic
            {
                Origin = item.Response,
                FileData = data,
            };
            var toRet2 = new GoogleGroupTopic
            {
                Origin = item.Response,
                FileData = data,
            };
            toRet.Messages = new List<GoogleGroupMessage>();
            toRet2.Messages = new List<GoogleGroupMessage>();
            string firstTopic = null;
            string secTopic = null;
            int i;
            for (i = 0; i < data.Count - 1; i++)
            {
                if (data[i] == "moi")
                {
                    firstTopic = data[i + 1];
                    secTopic = data[i + 2];
                    i += 3;
                    break;
                }
            }
            if (topicIds.Where(o => o.Key == firstTopic).ToList().Count <= 0 || topicIds.Where(o => o.Key == secTopic).ToList().Count <= 0)
                return;

            var theFirst = topicIds.ContainsKey(firstTopic) ? topicIds[firstTopic] : null;
            var theSecon = topicIds.ContainsKey(secTopic) ? topicIds[secTopic] : null;
            ////toRet.Subject =       topicIds.Where(o => o.Key == firstTopic).Select(o => o.Value.Item1).SingleOrDefault();
            ////toRet.LastModified =  topicIds.Where(o => o.Key == firstTopic).Select(o => o.Value.Item2).Single();
            ////toRet.OriginId =      topicIds.Where(o => o.Key == firstTopic).Select(o => o.Key).Single();
            ////toRet2.Subject =      topicIds.Where(o => o.Key == secTopic).Select(o => o.Value.Item1).SingleOrDefault();
            ////toRet2.LastModified = topicIds.Where(o => o.Key == secTopic).Select(o => o.Value.Item2).Single();
            ////toRet2.OriginId =     topicIds.Where(o => o.Key == secTopic).Select(o => o.Key).Single();
            toRet.Subject =       theFirst.Title;
            toRet.LastModified =  theFirst.LastUpdatedDate;
            toRet.OriginId =      theFirst.Id;
            toRet2.Subject =      theSecon.Title;
            toRet2.LastModified = theSecon.LastUpdatedDate;
            toRet2.OriginId =     theSecon.Id;

            IList<GoogleGroupMessage> toConcat = new List<GoogleGroupMessage>();
            var concatPhase = true;
            string firstSubjectEncounter = null;
            var msgTmp = data[i];
            string prev = "";
            for (int j = i; j < data.Count; j++)
            {
                if (IsHashAfterAuthor(data[j]) && !msgTmp.StartsWith("APn2wQ") && !msgTmp.StartsWith("~APn2wQ"))
                {
                    var toAdd = new GoogleGroupMessage
                    {
                        Author = data[j - 1],
                        Message = GetCleanMessage(msgTmp),
                        oMessage = msgTmp,
                    };
                    if (string.IsNullOrEmpty(toAdd.Message))
                    {
                        toAdd.Message = GetCleanMessage(prev);
                        toAdd.oMessage = prev;
                    }
                    if (concatPhase)
                    {
                        if (toRet.Subject == firstSubjectEncounter)
                            toRet.Messages.Add(toAdd);
                        else
                            toRet2.Messages.Add(toAdd);
                    }
                    else
                    {
                        if (toRet.Subject == firstSubjectEncounter)
                            toRet2.Messages.Add(toAdd);
                        else
                            toRet.Messages.Add(toAdd);
                    }
                }
                else if (IsEntryHtml(data[j]))
                {
                    prev = msgTmp;
                    msgTmp = data[j];
                }
                else if (firstSubjectEncounter != null && data[j] != firstSubjectEncounter && (data[j] == toRet.Subject || data[j] == toRet2.Subject))
                    concatPhase = false;
                if (firstSubjectEncounter == null && (data[j] == toRet.Subject || data[j] == toRet2.Subject))
                    firstSubjectEncounter = data[j];
            }

            if (this.topicList.Where(o => o.Subject == toRet.Subject).SingleOrDefault() != null)
            {
                this.topicList.Where(o => o.Subject == toRet.Subject).SingleOrDefault().Messages = this.topicList.Where(o => o.Subject == toRet.Subject).SingleOrDefault().Messages.Concat(toRet.Messages).ToList();
            }
            else
                this.topicList.Add(toRet);
            if (this.topicList.Where(o => o.Subject == toRet2.Subject).SingleOrDefault() != null)
            {
                this.topicList.Where(o => o.Subject == toRet2.Subject).SingleOrDefault().Messages = this.topicList.Where(o => o.Subject == toRet2.Subject).SingleOrDefault().Messages.Concat(toRet2.Messages).ToList();
            }
            else
                this.topicList.Add(toRet2);
        }

        private IList<GoogleGroupTopic> GetResponseJsonContents(IList<RequestResponseEntry> wanted, IDictionary<string, TopicInfo> topicIds)
        {
            int n = 0;
            foreach (var item in wanted)
            {
                var stream = item.Response.OpenReader();
                if (++n % 100 == 0)
                    Console.WriteLine("Analyzing entry " + n + " / " + wanted.Count + "...");
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var fileFullContent = reader.ReadToEnd();
                    var fileContent = fileFullContent.Split(new string[] { "//OK" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    int contentLength = fileContent.Length;
                    var fileContentSplit = fileContent.Split(new string[] { ",[\"", "\"]," }, StringSplitOptions.RemoveEmptyEntries);
                    if (fileContentSplit.Length < 2)
                        continue;
                    fileContent = fileContentSplit[1];

                    var fileData = (IList<string>)fileContent.Split(new string[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(o => o.Length > 2 && !IsEntryOnlyDigit(o) && !IsEntrySubjectResponse(o) && !IsEntryHtml(o))
                        //.Select(o => SrkStringTransformer.UnescapeUnicodeSequences(o))
                        .Select(o => UnescapeJsonValue(o))
                        .ToList();
                    fileData = RemoveUselessHtml(fileData);

                    if (IsTopicAlone(fileData))
                    {
                        var topic = GetGoogleGroupTopic(item, fileData, topicIds);
                        if (topic != null)
                        {
                            var theOne = this.topicList.Where(o => o.Subject == topic.Subject).SingleOrDefault();
                            if (theOne != null)
                            {
                                theOne.Messages = theOne.Messages.Concat(topic.Messages).ToList();
                            }
                            else if (topic.Subject != null || topic.Messages.Count > 0)
                            {
                                this.topicList.Add(topic);
                            }
                        }
                    }
                    else
                    {
                        GetGoogleGroupTopicNotAlone(item, fileData, topicIds);
                    }
                }
            }

            foreach (var item in this.topicList)
            {
                var idToDelete = new List<GoogleGroupMessage>();
                for (int i = 0; i < item.Messages.Count; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (item.Messages[j].Message == item.Messages[i].Message)
                            idToDelete.Add(item.Messages[i]);
                    }
                }
                idToDelete.ForEach(o => item.Messages.Remove(o));
            }

            
            int totalMessages = this.topicList.Select(o => o.Messages.Count).Sum();
            var emptyTopic = topicIds
                .Select(o => new
                {
                    id = o,
                    topic = this.topicList.Where(s => s.Subject == o.Value.Title).SingleOrDefault(),
                })
                .Where(o => o.topic == null)
                .ToList();

            Console.WriteLine("Analyze complete !");
            Console.WriteLine("Found " + this.topicList.Count + " topics match for " + totalMessages + " messages.");

            return this.topicList;
        }

        private static Regex jsonEscapedQuote = new Regex("\\\"");
        private static string UnescapeJsonValue(string value)
        {
            string newValue = SrkStringTransformer.UnescapeUnicodeSequences(value);
            newValue = newValue.Replace("\\\"", "\"");
            return newValue;
        }

        public class TopicInfo
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public DateTime LastUpdatedDate { get; set; }
            public string Url { get; set; }
        }

        // Get ID/Subject from html ninja
        private IDictionary<string, TopicInfo> GetSubjectListFromXHtml(string path)
        {
            string xml;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var reader = new StreamReader(stream, Encoding.UTF8);
                xml = reader.ReadToEnd();
                xml = xml.Replace("&nbsp;", " ");
            }

            IDictionary<string, TopicInfo> topicWithId = new Dictionary<string, TopicInfo>();
            var doc = XDocument.Parse(xml);
            var root = doc.Root;
            var lines = root.Element("tbody").Elements("tr");
            foreach (var line in lines)
            {
                var link = line.Descendants("a").ToArray().Where(e => e.Attribute("href") != null && e.Attribute("href").Value.StartsWith("#!")).Single();
                var lastDate = line.Descendants("span").ToArray().Where(e => e.Attribute("title") != null && e.Descendants().ToArray().Length == 0).ToArray()[0];
                var key = link.Attribute("href").Value.Split(new char[] { '/' })[2];
                var title = link.Value;
                var date = Convert.ToDateTime(lastDate.Attribute("title").Value.Split(new string[] { " UTC" }, StringSplitOptions.None)[0]);
                ////topicWithId.Add(key, new Tuple<string, DateTime>(title, date));
                topicWithId.Add(key, new TopicInfo
                {
                    Id = key,
                    Title = title,
                    LastUpdatedDate = date,
                    Url = "https://groups.google.com/forum/" + link.Attribute("href").Value,
                });
            }

            return topicWithId;
        }

        private IDictionary<string, int> GetPeoplesStats(IList<GoogleGroupTopic> topics)
        {
            IDictionary<string, int> toRet = new Dictionary<string, int>();

            foreach (var item in topics)
            {
                foreach (var m in item.Messages)
                {
                    var key = m.Author.MakeUrlFriendly(false);
                    if (toRet.ContainsKey(key))
                        toRet[key] = toRet[key] + 1;
                    else
                        toRet.Add(key, 1);
                }
            }

            return toRet;
        }
        
        private void ExportToCamping(IList<GoogleGroupTopic> topics, User user)
        {
            int n = 0;
            var toAdd = topics.Sum(o => o.Messages.Count);
            int userId = user.Id;

            var increment = new Action(() =>
            {
                n++;
                if (n % 100 == 0){
                    this.Out.WriteLine("Importing item " + n + " / " + toAdd);
                }
            });

            foreach (var item in topics)
            {
                var text = item.Subject + "\r\n\r\n" + item.Messages.First().Message;
                if (text.Length > 4000)
                    text = text.Substring(0, 3995) + "...";

                var subjectAuthor = item.Messages.First().Author;
                var extraData = "Type:GoogleGroupsImportedMessage" + Environment.NewLine
                    + "Owner:undefined" + Environment.NewLine
                    + "TopicUrl:" + item.Url + Environment.NewLine
                    + "TopicId:" + item.OriginId + Environment.NewLine
                    + "Subject:" + item.Subject + Environment.NewLine;

                var wallitem = new TimelineItem
                {
                    CreateDate = item.LastModified.AddMinutes(-item.Messages.Count),
                    ImportedId = subjectAuthor.MakeUrlFriendly(false) + "||||" + subjectAuthor + "[{}]" + item.OriginId,
                    TimelineItemType = Entities.TimelineItemType.TextPublication,
                    PostedByUserId = userId,
                    PrivateMode = 0,
                    Text = text,
                    ExtraTypeValue = TimelineItemExtraType.GoogleGroupImportedMessage,
                    Extra = extraData,
                };
                var wallId = context.Services.Wall.Import(wallitem);
                increment();

                for (int i = 1; i < item.Messages.Count; i++)
                {
                    var text2 = item.Messages[i].Message;
                    if (text2.Length > 4000)
                        text2 = text2.Substring(0, 3995) + "...";

                    var messageAuthor = item.Messages[i].Author;
                    extraData = "Type:GoogleGroupsImportedMessage" + Environment.NewLine
                        + "Owner:undefined" + Environment.NewLine
                        + "TopicUrl:" + item.Url + Environment.NewLine
                        + "TopicId:" + item.OriginId + Environment.NewLine
                        + "Subject:" + item.Subject + Environment.NewLine;

                    var comment = new TimelineItemComment
                    {
                        CreateDate = wallitem.CreateDate.AddMinutes(i),
                        ImportedId = messageAuthor.MakeUrlFriendly(false) + "||||" + messageAuthor + "[{}]" + item.OriginId,
                        PostedByUserId = userId,
                        Text = text2,
                        TimelineItemId = wallitem.Id,
                        ExtraTypeValue = TimelineItemExtraType.GoogleGroupImportedMessage,
                        Extra = extraData,
                    };
                    context.Services.WallComments.Import(comment);
                    increment();
                }
            }
        }

        private bool SanifyJsonFromFiddler(string[] args)
        {
            var serializer = new JsonSerializer();
            var topic = new GoogleGroupTopic();

            Console.WriteLine("Opening trafic archive for entries...");
            Console.WriteLine("Press any key to begin...");
            //Console.ReadKey();

            var zip = Ionic.Zip.ZipFile.Read(args[1]);
            var wanted = GetWantedFilenames(zip);
            
            Console.WriteLine("Found " + wanted.Count + " usefull entries !");

            Console.WriteLine("\nOpening super ninja html file for topics list...");
            Console.WriteLine("Press any key to begin...");
            //Console.ReadKey();
            var topicId = GetSubjectListFromXHtml(args[2]);
            Console.WriteLine("Found " + topicId.Count + " topics !");
            Console.WriteLine("\nBegining entries analyze...");
            Console.WriteLine("Press any key to begin...");
            //Console.ReadKey();


            var topicList = GetResponseJsonContents(wanted, topicId).OrderBy(o => o.LastModified).ToList();

            var peoplesStats = GetPeoplesStats(topicList).OrderByDescending(o => o.Key).OrderByDescending(o => o.Value).ToList();
            var peoplesStatsSum = peoplesStats.Sum(x => x.Value);
            var peoplesStatsUsers = peoplesStats.Count(x => x.Value > 1);

            var sigblacklist = new List<Func<string, bool>>()
            {
                str => str.Contains("Sent from my"),
                str => str.Contains("Envoyé depu"),
                str => str.Length < 11,
                str => str.StartsWith("<blockq") && str.EndsWith (">"),
                str => string.IsNullOrWhiteSpace(str),
                str => str.Contains("<p class=\"MsoNormal\">"),
                str => str.Contains("(image)(image)"),
                str => str.Contains("<p style=\"margin-top:0px;margin-right:0px;margin-bottom:0px;margin-left:0px\">"),
                str => str.Contains("Envoyé de mon"),
                str => str.Contains("[(image)](http://(image))"),
                str => str.Contains("Le Camping is a project created by [Silicon Sentier](http://Silicon Sentier)"),
                str => str.Contains("\t\t\t\t\t"),
                str => str.Contains("<p style=\"margin:0px\">[]()(image)(image)(image)(image)"),
                str => str.Contains("<p style=\"margin:0px\">(image)"),
                str => str.Contains("<p style=\"margin:0px\">[(image)](http://(image))"),
                str => str.Contains("</blockquote>"),
                str => str.Contains("****************"),
                str => str.Contains("<b style=\\\"font-family:arial,helvetica,sans-serif\\\">"),
                str => str.Contains("For more options")&&str.Contains("groups/opt_out"),
                str => str.Contains("**(image)"),
                str => str.StartsWith("<p style="),
                str => str.StartsWith("> an email to le-camping-brotherhood"),

            };
            var notGroupedByUsername = peoplesStats.Where(s => s.Value < 2).Select(mm => mm.Key).ToList();
            var peoplesStatsSig = topicList
                .SelectMany(t => t.Messages)
                .Where(m => notGroupedByUsername.Contains(m.Author))
                .GroupBy(m => m.Message
                    .Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Reverse()
                    .SkipWhile(str => sigblacklist.Any(x => x(str)))
                    .FirstOrDefault() ?? "empty")
                .ToDictionary(g => g.Key, g => g.ToArray())
                .OrderByDescending(d => d.Value.Length)
                .ToDictionary(g => g.Key, g => g.Value);

            var allSigSeparationLines = topicList
                .SelectMany(t => t.Messages)
                .Where(m => m.Message.Contains("]("))
                .ToArray();

            ExportToCamping(topicList, context.MessageOwner);

            return true;
        }

        private bool MakeDataParsingFromGoogleGroup(string[] args)
        {
            SanifyJsonFromFiddler(args);

            //SanifyJsonFromFiddler(@"C:\Users\remim_000\Desktop\googlegroups\before_prod_topiclist.saz");

            return true;
        }

        public class Context
        {
            public Context()
            {
            } 

            public SparkleCommandArgs Args { get; set; }
            public IServiceFactory Services { get; set; }

            public string GoogleGroupSite { get; set; }
            public string GoogleGroupUsername { get; set; }
            public string GoogleGroupPassword { get; set; }

            public User MessageOwner { get; set; }
        }

        public class GoogleGroupTopic
        {
            public string Subject { get; set; }

            public IList<GoogleGroupMessage> Messages { get; set; }

            public ZipEntry Origin { get; set; }

            public IList<string> FileData { get; set; }

            public DateTime LastModified { get; set; }

            public string OriginId { get; set; }

            public override string ToString()
            {
                return "Subject: " + Subject + " with " + Messages.Count + " messages";
            }

            public string Url { get; set; }
        }

        public class GoogleGroupMessage
        {
            public string Author { get; set; }

            public string Message { get; set; }

            public string oMessage { get; set; }

            public override string ToString()
            {
                return "From: " + Author + ", msg: " + Message;
            }
        }
    }
}
