
namespace Sparkle.Services.Main.Networks
{
    using ARSoft.Tools.Net.Dns;
    using Newtonsoft.Json;
    using Sparkle.Data.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public class InfrastructureService : ServiceBase, IInfrastructureService
    {
        [DebuggerStepThrough]
        internal InfrastructureService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public FullCheckModel GetFullCheck(bool executeChecks)
        {
            var model = new FullCheckModel();
            model.DnsEntries = this.GetDnsCheck(executeChecks);
            return model;
        }

        private MandrillConfiguration GetMandrillConfiguration()
        {
            var json = this.Services.AppConfiguration.Tree.Externals.Mandrill.Configuration;
            if (string.IsNullOrEmpty(json))
            {
                return new MandrillConfiguration();
            }

            var item = JsonConvert.DeserializeObject<MandrillConfiguration>(json);
            return item;
        }

        private IList<DnsEntryModel> GetDnsCheck(bool executeChecks)
        {
            var items = new List<DnsEntryModel>(this.Services.AppConfiguration.DomainNames.Count);
            var mainDomainName = this.Services.AppConfiguration.Tree.Site.MainDomainName;
            var webHost = this.Services.AppConfiguration.Tree.Infrastructure.WebApp.Host;
            var mandrill = this.GetMandrillConfiguration() ?? new MandrillConfiguration();
            var mandrillRecords = mandrill.DnsRecords ?? new List<MandrillDnsRecord>(0);

            foreach (var item in this.Services.AppConfiguration.DomainNames)
            {
                var model = new DnsEntryModel(item.Name, "A");
                model.ExpectedValue = webHost;
                items.Add(model);

                if (item.Name.EndsWith(".local"))
                {
                    continue;
                }

                if (item.Name == mainDomainName || mandrillRecords.Any(r => r.Name == item.Name || r.Name.EndsWith("." + item.Name)))
                {
                    var modelSpf = new DnsEntryModel(item.Name, "SPF", "TXT");
                    ////modelSpf.ExpectedValue = "include:xxxxxxxx";
                    items.Add(modelSpf);

                    var modelDkim = new DnsEntryModel("mandrill._domainkey." + item.Name, "DKIM", "TXT");
                    ////modelDkim.ExpectedValue = "DKIM..........";
                    items.Add(modelDkim);

                    {
                        var mandrillSpf = mandrillRecords.SingleOrDefault(r => r.Type == "SPF" && r.Name == item.Name);
                        if (mandrillSpf != null)
                        {
                            modelSpf.ExpectedValue = mandrillSpf.Value;
                            modelSpf.ExpectedValueHint = "include:" + mandrillSpf.Value;
                        }

                        var mandrillDkim = mandrillRecords.SingleOrDefault(r => r.Type == "DKIM" && r.Name.EndsWith("." + item.Name));
                        if (mandrillDkim != null)
                        {
                            modelDkim.ExpectedValue = mandrillDkim.Value;
                        }
                    }
                }
            }

            if (executeChecks)
            {
                var openDnsServers = new List<IPAddress>() { IPAddress.Parse("208.67.222.222"), IPAddress.Parse("208.67.220.220"), };
                var client = new DnsClient(openDnsServers, 10000);
                Parallel.ForEach(items, item =>
                {
                    if (item.Name.EndsWith(".local"))
                    {
                        item.IsActualValueValid = null;
                        return;
                    }

                    item.DnsServers = string.Join(";", openDnsServers);
                    item.IsActualValueValid = false;
                    try
                    {
                        if (item.Type == "A")
                        {
                            var result = client.Resolve(item.Name, RecordType.A, RecordClass.INet);
                            if (result.AnswerRecords == null || result.AnswerRecords.Count == 0 || result.ReturnCode == ReturnCode.NxDomain)
                            {
                                item.Errors = item.Errors ?? new List<string>();
                                item.Errors.Add("No DNS record.");
                            }
                            else
                            {
                                item.ActualValues = result.AnswerRecords.Select(a => a.ToString()).ToArray();
                                var answers = result.AnswerRecords.Where(a => a.RecordType == RecordType.A).ToArray();
                                if (answers.Length == 0)
                                {
                                    item.Errors = item.Errors ?? new List<string>();
                                    item.Errors.Add("No DNS record of type A.");
                                }
                                else if (answers.Length == 1)
                                {
                                    var record = (ARecord)answers[0];
                                    item.ActualValue = record.Address.ToString();
                                    item.IsActualValueValid = item.ActualValue == item.ExpectedValue;
                                }
                                else
                                {
                                    item.Errors = item.Errors ?? new List<string>();
                                    item.Errors.Add("Wrong DNS records.");
                                }
                            }
                        }
                        else if (item.Type == "SPF")
                        {
                            var result = client.Resolve(item.Name, RecordType.Txt);

                            if (result.AnswerRecords == null || result.AnswerRecords.Count == 0 || result.ReturnCode == ReturnCode.NxDomain)
                            {
                                item.Errors = item.Errors ?? new List<string>();
                                item.Errors.Add("No DNS record.");
                            }
                            else if (result.AnswerRecords.Count == 1)
                            {
                                if (result.AnswerRecords[0] is TxtRecord)
                                {
                                    var record = (TxtRecord)result.AnswerRecords[0];
                                    item.ActualValue = record.TextData;

                                    ARSoft.Tools.Net.Spf.SpfRecord spf;
                                    if (ARSoft.Tools.Net.Spf.SpfRecord.TryParse(item.ActualValue, out spf))
                                    {
                                        if (spf.Terms != null && spf.Terms.OfType<ARSoft.Tools.Net.Spf.SpfMechanism>().Any(t => t.Type == ARSoft.Tools.Net.Spf.SpfMechanismType.Include && t.Domain == item.ExpectedValue))
                                        {
                                            item.IsActualValueValid = true;
                                        }
                                        else
                                        {
                                            item.Errors = item.Errors ?? new List<string>();
                                            item.Errors.Add("SPF record does not contain the expected term.");
                                        }
                                    }
                                    else
                                    {
                                        item.Errors = item.Errors ?? new List<string>();
                                        item.Errors.Add("SPF record is not valid (parse error).");
                                    }
                                }
                                else
                                {
                                    item.Errors = item.Errors ?? new List<string>();
                                    item.Errors.Add("SPF record is not valid (dns answer is of type " + result.AnswerRecords[0].GetType().Name + " instead of TxtRecord).");
                                }
                            }
                            else
                            {
                                item.ActualValues = result.AnswerRecords.Select(a => a.ToString()).ToArray();
                                item.Errors = item.Errors ?? new List<string>();
                                item.Errors.Add("Too many DNS records.");
                            }
                        }
                        else if (item.Type == "DKIM")
                        {
                            var result = client.Resolve(item.Name, RecordType.Txt);

                            if (result.AnswerRecords == null || result.AnswerRecords.Count == 0 || result.ReturnCode == ReturnCode.NxDomain)
                            {
                                item.Errors = item.Errors ?? new List<string>();
                                item.Errors.Add("No DNS record.");
                            }
                            else if (result.AnswerRecords.Count == 1)
                            {
                                var record = (TxtRecord)result.AnswerRecords[0];
                                item.ActualValue = record.TextData;
                                item.IsActualValueValid = item.ActualValue == item.ExpectedValue;
                            }
                            else
                            {
                                item.ActualValues = result.AnswerRecords.Select(a => a.ToString()).ToArray();
                                item.Errors = item.Errors ?? new List<string>();
                                item.Errors.Add("Too many DNS records.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        item.Errors = item.Errors ?? new List<string>();
                        item.Errors.Add("Internal error: " + ex.Message);
                    }
                });
            }

            return items;
        }
    }
}
