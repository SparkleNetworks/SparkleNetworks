
namespace Sparkle.WebStatus.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    public class NetworksService : BaseService<NetworkModel>
    {
        private const string internalApiKeyHeaderName = "X-Sparkle-InternalApiKey";
        private static readonly Guid internalApiKey = new Guid("741EC152-4E25-4891-B837-A4CE3298A653");

        public NetworksService(ServiceConfiguration configuration)
            : base(configuration, "Networks")
        {
        }

        public Dictionary<Guid, NetworkModel> GetAll()
        {
            return this.Read();
        }

        public NetworkModel Create(NetworkModel model)
        {
            using (var transaction = this.Write())
            {
                model.DateCreatedUtc = DateTime.UtcNow;
                if (model.Guid == Guid.Empty)
                {
                    model.Guid = Guid.NewGuid();
                }

                if (transaction.Items.ContainsKey(model.Guid))
                    throw new ArgumentException("An item with id '" + model.Guid + "' already exists");

                transaction.Items.Add(model.Guid, model);
                transaction.Save();
            }

            return model;
        }

        public IList<NetworkStatusModel> GetAllStatus()
        {
            var items = this.GetAll()
                .Select(x => new NetworkStatusModel(x.Value))
                .ToList();

            Parallel.ForEach(items, item =>
            {
                GetNetworkStatus(item);
            });

            return items;
        }

        internal NetworkModel GetById(Guid id)
        {
            return this.GetAll()[id];
        }

        internal NetworkStatusModel GetOnlineUsernames(Guid id)
        {
            var network = this.GetById(id);
            var item = new NetworkStatusModel(network);

            var url = string.Format("http://{0}/api/1.0/OnlineUsernames", network.InternalDomainName);
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Accept = "application/json";
                request.Headers.Add(internalApiKeyHeaderName, internalApiKey.ToString());
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                var json = reader.ReadToEnd();
                item.RawStatus = json;

                var list = JsonConvert.DeserializeObject<List<string>>(json);
                item.OnlineUsernames = list;
            }
            catch (Exception ex)
            { 
                item.StatusException = ex;
            }

            return item;
        }

        internal void Update(NetworkModel model)
        {
            using (var transaction = this.Write())
            {
                var network = transaction.Items.Single(n => n.Key == model.Guid);

                network.Value.InstanceName = model.InstanceName;
                network.Value.InternalDomainName = model.InternalDomainName;
                network.Value.MainDomainName = model.MainDomainName;
                network.Value.NetworkName = model.NetworkName;
                network.Value.UniverseName = model.UniverseName;

                transaction.Save();
            }
        }

        internal NetworkStatusModel GetStatus(Guid id)
        {
            var network = this.GetById(id);
            var item = new NetworkStatusModel(network);

            GetNetworkStatus(item);

            return item;
        }

        private static void GetNetworkStatus(NetworkStatusModel item)
        {
            var url = string.Format("http://{0}/api/1.0/BuildInfo", item.Network.InternalDomainName);

            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Accept = "application/json";
                request.Headers.Add(internalApiKeyHeaderName, internalApiKey.ToString());
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                var json = reader.ReadToEnd();
                item.RawStatus = json;

                dynamic obj = JsonConvert.DeserializeObject(json);
                string buildDateUtc = obj.BuildDateUtc;
                string assFileVersion = obj.AssemblyFileVersion;

                item.AssemblyVersion = obj.AssemblyVersion != null ? new Version(obj.AssemblyVersion) : null;
                item.AssemblyFileVersion = assFileVersion != null ? new Version(assFileVersion) : null;
                item.BuildDateUtc = DateTime.ParseExact(buildDateUtc, "yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture).AsUtc();
                item.BuildConfiguration = (string)obj.BuildConfiguration;
                item.OnlineUsers = (int?)obj.OnlineUsers;
                item.ServicesVerified = (bool?)obj.ServicesVerified;
                item.ServicesVerifyErrors = obj.ServicesVerifyErrors != null ? ((Newtonsoft.Json.Linq.JArray)obj.ServicesVerifyErrors).Values<string>().ToArray() : null;
                item.ActiveUsers = obj.Numbers != null ? (int)obj.Numbers.ActiveUsers : default(int?);
                item.ActivitiesH24 = obj.Activities != null ? (int)obj.Activities.H24 : default(int?);
            }
            catch (Exception ex)
            {
                item.StatusException = ex;
            }
        }

        internal void InitializeNetwork(Guid guid)
        {
            var network = this.GetById(guid);
            var url = string.Format("http://{0}/api/1.0/InitializeNetwork", network.InternalDomainName);

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Accept = "application/json";
            request.Headers.Add(internalApiKeyHeaderName, internalApiKey.ToString());
            var response = (HttpWebResponse)request.GetResponse();
        }
    }
}