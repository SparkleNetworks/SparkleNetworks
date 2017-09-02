
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
#if SSC
#else
    using Sparkle.Infrastructure.Data;
#endif

    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                return Error(1, "Expected command", showCommands: true);
            }

            AppConfiguration.RepositoryFactory = () => new Data.WcfConfigurationFactory();
            AppConfiguration.RepositoryFactory = () =>
            {
                using (var wcf = new Data.WcfConfigurationFactory())
                    return new Data.FileCachedConfigurationFactory(wcf);
            };

            var repo = AppConfiguration.NewRepository;
            try
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "keys":
                        var keys = repo.FetchKeys();
                        foreach (var key in keys)
                        {
                            Console.WriteLine(key.Key);
                        }
                        break;

                    case "key":
                        if (args.Length < 2)
                            return Error(3, "Missing key id");

                        keys = repo.FetchKeys();
                        var key1 = keys.SingleOrDefault(k => k.Key == args[1]);
                        Console.WriteLine("Id:        " + key1.Id);
                        Console.WriteLine("Key:       " + key1.Key);
                        Console.WriteLine("KeyId:     " + key1.KeyId);
                        Console.WriteLine("Summary:   " + key1.Summary);
                        Console.WriteLine("Type:      " + key1.BlittableType);
                        Console.WriteLine("Default:   " + key1.DefaultRawValue);
                        break;

                    case "apps":
                        if (args.Length < 3)
                            return Error(4, "Missing product and host");

                        var apps = repo.FindApplications(args[1], args[2]);
                        foreach (var app in apps)
                        {
                            ShowDetails(app);
                        }
                        break;

                    case "find-app":
                        if (args.Length < 4)
                            return Error(5, "Missing product, host and universe");

                        var findApp = repo.FindApplicationId(args[1], args[2], args[3]);
                        Console.WriteLine(findApp);
                        break;

                    case "find-app-domain":
                        if (args.Length < 4)
                            return Error(6, "Missing product, host and domain name");

                        var findAppDn = repo.FindApplicationIdByDomainName(args[1], args[2], args[3]);
                        Console.WriteLine(findAppDn);
                        break;

                    case "app":
                        if (args.Length < 2)
                            return Error(7, "Missing app id");

                        var theApp = repo.FetchValues(int.Parse(args[1]));
                        foreach (var theApp1 in theApp)
                        {
                            ShowDetails(theApp1);
                        }
                        break;

                    default:
                        return Error(2, "Unknown command", showCommands: true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                repo.Dispose();
            }

            return 0;
        }

        private static void ShowDetails(KeyValuePair<string, AppConfigurationEntry> app)
        {
            Console.WriteLine("Key:          " + app.Key);
            Console.WriteLine("KeyId:        " + app.Value.KeyId);
            Console.WriteLine("RawValue:     " + app.Value.RawValue);
            Console.WriteLine();
        }

        private static void ShowDetails(Application app)
        {
            Console.WriteLine("Id:           " + app.Id);
            Console.WriteLine("HostId:       " + app.HostId);
            Console.WriteLine("HostName:     " + app.HostName);
            Console.WriteLine("ProductId:    " + app.ProductId);
            Console.WriteLine("ProductName:  " + app.ProductName);
            Console.WriteLine("UniverseId:   " + app.UniverseId);
            Console.WriteLine("UniverseName: " + app.UniverseName);
            Console.WriteLine();
        }

        private static int Error(int code, string message, bool showCommands = false)
        {
            Console.WriteLine(message);

            if (showCommands)
            {
                Console.WriteLine("Available commands: ");
                Console.WriteLine("  keys");
                Console.WriteLine("  apps");
                Console.WriteLine("  apps");
                Console.WriteLine();
            }

            Environment.Exit(code);
            return code;
        }
    }
}
