using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Sparkle.Common;
using Sparkle.Common.CommandLine;
using Sparkle.Entities;
using Sparkle.Entities.Networks;
using Sparkle.Infrastructure;
using Sparkle.Services;
using Sparkle.Services.Main.Networks;
using Sparkle.Services.Networks;
using Sparkle.Services.Networks.Users;
using SrkToolkit.Common.Validation;

namespace Sparkle.Commands.Main
{
    public class Invite : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;

        public Invite()
        {
        }

        public override void RunUniverse(SparkleCommandArgs args)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;
            Stream stream = null;
            TextReader reader = null;
            var log = args.SysLogger;
            var companyDomains = new Dictionary<string, Company>();
            var results = new List<InvitePersonResult.ResultCode?>();

            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "Invite";
            services.HostingEnvironment.RemoteClient = this.remoteClient;

            // network
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("Invite", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return;
            }

            services.NetworkId = network.Id;

            try
            {
                if (args.InFile != null)
                {
                    stream = new FileStream(args.InFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    reader = new StreamReader(stream);
                }
                else
                {
                    reader = new StringReader(args.Arguments.Skip(1).Aggregate((s1, s2) => string.Concat(s1, Environment.NewLine, s2)));
                }

                string line = null;
                for (;;)
                {
                    line = reader.ReadLine();
                    if (line == null)
                        break;
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    foreach (var address in Validate.ManyEmailAddresses(line))
                    {
                        string msg = null;

                        if (!this.Simulate)
                        {
                            results.Add(
                                RealInviteAddress(log, services, address));
                        }
                        else
                        {
                            results.Add(
                                FakeInviteAddress(companyDomains, services, address));
                        }
                    }
                }
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
                if (reader != null)
                    reader.Dispose();
                if (services != null)
                    services.Dispose();
            }

            this.Out.WriteLine();
            this.Out.WriteLine("Results: ");
            this.Out.WriteLine("-------- ");
            foreach (var result in results.Distinct())
            {
                this.Out.WriteLine("{0}:\t{1}", result.HasValue ? result.Value.ToString() : "ERROR", results.Count(r => r == result));
            }
        }

        private InvitePersonResult.ResultCode? FakeInviteAddress(Dictionary<string, Company> companyDomains, IServiceFactory services, string address)
        {
            var domain = address.Substring(address.IndexOf('@') + 1);
            var user = services.People.SelectWithProMail(address);
            var company = GetCompanyByDomain(services, companyDomains, domain);
            var invite = services.Repositories.Invited.Select().FirstOrDefault(i => i.Email == address);

            if (user != null)
            {
                this.Out.WriteLine("UEXISTS: {0,-38} {1}", address, company.Name);
                return InvitePersonResult.ResultCode.UserExists;
            }
            else if (invite != null)
            {
                this.Out.WriteLine("IEXISTS: {0,-38}", address);
                return InvitePersonResult.ResultCode.AlreadyInvited;
            }
            else if (company != null)
            {
                this.Out.WriteLine("INVITED: {0,-38} {1}", address, company.Name);
                return InvitePersonResult.ResultCode.Done;
            }
            else
            {
                this.Error.WriteLine("NOCOMP:  {0,-38}", address);
                return InvitePersonResult.ResultCode.NoCompany;
            }
        }

        private InvitePersonResult.ResultCode? RealInviteAddress(SysLogger log, IServiceFactory services, string address)
        {
            InvitePersonResult result;
            try
            {
                result = services.Invited.Invite(null, address);

                switch (result.Code)
                {
                    case InvitePersonResult.ResultCode.Done:
                        log.Info("Invite", remoteClient, Environment.UserName, ErrorLevel.Success, address);
                        this.Out.WriteLine("INVITED: {0}\r\n  Company: {1}", address, result.Company.Name);
                        break;
                    case InvitePersonResult.ResultCode.InvalidAddress:
                        this.Out.WriteLine("INVALID: {0}", address);
                        break;
                    case InvitePersonResult.ResultCode.UserExists:
                        this.Out.WriteLine("UEXISTS: {0}", address);
                        break;
                    case InvitePersonResult.ResultCode.AlreadyInvited:
                        this.Out.WriteLine("IEXISTS: {0}", address);
                        break;
                    case InvitePersonResult.ResultCode.NoCompany:
                        this.Error.WriteLine("NOCOMP:  {0}", address);
                        break;
                    case InvitePersonResult.ResultCode.SmtpError:
                        this.Error.WriteLine("SMTP.ER: {0}", address);
                        break;
                    case InvitePersonResult.ResultCode.NotAuthorized:
                        this.Error.WriteLine("FORBID:  {0}", address);
                        break;
                    case InvitePersonResult.ResultCode.QuotaReached:
                        this.Error.WriteLine("QUOTAR:  {0}", address);
                        break;
                    default:
                        this.Error.WriteLine("??????:  {0}", address);
                        break;
                }
                return result.Code;
            }
            catch (ForbiddenException)
            {
                log.Warning("Invite", remoteClient, Environment.UserName, ErrorLevel.Authz, address);
                this.Error.WriteLine("FORBID:  {0}", address);
                return null;
            }
            catch (DataException ex)
            {
                log.Warning("Invite", remoteClient, Environment.UserName, ErrorLevel.Data, ex);
                this.Error.WriteLine("DATA.ER: {0}", address);
                return null;
            }
            catch (Exception ex)
            {
                log.Warning("Invite", remoteClient, Environment.UserName, ErrorLevel.Data, ex);
                this.Error.WriteLine("ERROR.E: {0}", address);
                return null;
            }
        }

        #region ISparkleCommandsInitializer members

        public void Register(SparkleCommandRegistry registry)
        {
            registry.Register("Invite", "Invite people with email addresses.", () => new Invite());
        }

        #endregion

        private Company GetCompanyByDomain(IServiceFactory services, Dictionary<string, Company> cache, string hostname)
        {
            if (cache.ContainsKey(hostname))
                return cache[hostname];

            return cache[hostname] = services.Company.SelectByDomainName(hostname);
        }
    }
}
