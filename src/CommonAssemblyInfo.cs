using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

#if SSC
[assembly: AssemblyCompany(SparkleSystems.Configuration.AppVersion.Company)]
[assembly: AssemblyProduct(SparkleSystems.Configuration.AppVersion.Name)]
[assembly: AssemblyCopyright(SparkleSystems.Configuration.AppVersion.Copyright)]

[assembly: AssemblyVersion(SparkleSystems.Configuration.AppVersion.Full)]
[assembly: AssemblyFileVersion(SparkleSystems.Configuration.AppVersion.Full)]
#else
[assembly: AssemblyCompany(Sparkle.Infrastructure.AppVersion.Company)]
[assembly: AssemblyProduct(Sparkle.Infrastructure.AppVersion.Name)]
[assembly: AssemblyCopyright(Sparkle.Infrastructure.AppVersion.Copyright)]

[assembly: AssemblyVersion(Sparkle.Infrastructure.AppVersion.Full)]
[assembly: AssemblyFileVersion(Sparkle.Infrastructure.AppVersion.Full)]
#endif
