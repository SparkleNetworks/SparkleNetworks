
namespace Sparkle.Commands.Main
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using Sparkle.Common.CommandLine;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;

    public class ConfirmRegisteredEmail : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;

        #region ISparkleCommandsInitializer members

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle ConfirmRegisteredEmail <firstname.lastname> ...
    Sends the email to the specified usernames.";

            registry.Register(
                "ConfirmRegisteredEmail",
                "Sends a registration confirmation email to the specified persons.",
                () => new ConfirmRegisteredEmail(),
                longDesc);
        }

        #endregion

        public override void RunUniverse(SparkleCommandArgs args)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;
            var log = args.SysLogger;
            var results = new List<Tuple<string, bool>>();

            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "ConfirmRegisteredEmail";
            services.HostingEnvironment.RemoteClient = this.remoteClient;
            IList<User> people = null;

            // network
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("ConfirmRegisteredEmail", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return;
            }

            services.NetworkId = network.Id;

            // fetch people
            try
            {
                if (args.Arguments.Count < 2)
                {
                    this.Out.WriteLine("Please specify usernames.");
                    return;
                }
                else
                {
                    people = new List<User>();
                    foreach (var arg in args.Arguments.Skip(1))
                    {
                        if (string.IsNullOrWhiteSpace(arg))
                            continue;

                        //var item = services.People.SelectForWeeklyMailByLogin(arg);
                        var person = services.People.SelectWithLogin(arg);
                        if (person != null)
                        {
                            people.Add(person);
                        }
                        else
                        {
                            this.Error.WriteLine("ERROR: user " + arg + " does not exist.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Error.WriteLine(ex.Message);
                return;
            }

            if (people.Count == 0)
            {
                this.Out.WriteLine("Nobody.");
                return;
            }

            // send email
            int success = 0, failSmtp = 0, failData = 0, failOther = 0;
            foreach (var person in people)
            {
                if (!this.Simulate)
                {
                    try
                    {
                        services.Email.SendRegistred(person);
                        success++;
                        this.Out.WriteLine("OK:    " + person.Email);
                        args.SysLogger.Info("ConfirmRegisteredEmail", remoteClient, Environment.UserName, ErrorLevel.Success, person.Email);
                    }
                    catch (DataException ex)
                    {
                        failData++;
                        this.Error.WriteLine("ERROR: " + person.Email + " -> " + ex.GetType().Name + Environment.NewLine + "  " + ex.Message);
                        args.SysLogger.Error("ConfirmRegisteredEmail", remoteClient, Environment.UserName, ErrorLevel.Data, person.Email + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                    catch (InvalidOperationException ex)
                    {
                        failSmtp++;
                        this.Error.WriteLine("ERROR: " + person.Email + " -> " + ex.GetType().Name + Environment.NewLine + "  " + ex.Message);
                        args.SysLogger.Error("ConfirmRegisteredEmail", remoteClient, Environment.UserName, ErrorLevel.ThirdParty, person.Email + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                    catch (Exception ex)
                    {
                        failOther++;
                        this.Error.WriteLine("ERROR: " + person.Email + " -> " + ex.GetType().Name + Environment.NewLine + "  " + ex.Message);
                        args.SysLogger.Error("ConfirmRegisteredEmail", remoteClient, Environment.UserName, ErrorLevel.Internal, person.Email + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                        if (ex.IsFatal())
                        {
                            this.Error.WriteLine("FATAL ERROR. Exiting.");
                            throw;
                        }
                    }
                }
                else
                {
                    this.Out.WriteLine("OK (simulation): " + person.Email);
                }
            }

            this.Out.WriteLine();
            this.Out.WriteLine("Results: ");
            this.Out.WriteLine("-------- ");
            if (success > 0)
                this.Out.WriteLine("OK:          {0}", success);
            if (failSmtp > 0)
                this.Out.WriteLine("SMTP ERROR:  {0}", failSmtp);
            if (failData > 0)
                this.Out.WriteLine("DATA ERROR:  {0}", failData);
            if (failOther > 0)
                this.Out.WriteLine("OTHER ERROR: {0}", failOther);
        }
    }
}
