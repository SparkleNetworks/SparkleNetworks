
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Newtonsoft.Json;

    public class DomainNameRecordsService : BaseService<DomainNameRecord>
    {
        public DomainNameRecordsService(ServiceConfiguration configuration)
            : base(configuration, "DomainNameRecords")
        {
        }

        public Dictionary<Guid, DomainNameRecord> GetAll()
        {
            return this.Read();
        }

        public DomainNameRecord Create(DomainNameRecord model)
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
            throw new NotImplementedException();
            /*
            var items = this.GetAll().Select(x => new DomainNameRecordStatusModel(x.Value)).ToList();

            Parallel.ForEach(items, item =>
            {
                var url = string.Format("http://{0}/api/1.0/BuildInfo", item.Network.InternalDomainName);
                try
                {
                    var request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Accept = "application/json";
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
                }
                catch (Exception ex)
                {
                    item.StatusException = ex;
                }
            });

            return items;
            */
        }
    }
}
