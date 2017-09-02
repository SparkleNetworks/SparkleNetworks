
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure.Data
#endif
{
    /// <summary>
    /// A stub logging repository doing nothing.
    /// </summary>
    public sealed class FakeSysLogRepository : ISysLogRepository
    {
        public int FindApplicationId(string product, string host, string universe)
        {
            return 1;
        }

        public Application FindApplication(string product, string host, string universe)
        {
            return new Application(1, 1, 1, 1)
            {
                HostName = host,
                ProductName = product,
                UniverseName = universe,
            };
        }

        public void Write(short logEntryType, int applicationId, string applicationVersion, string path, string remoteClient, string identity, short errorLevel, string data)
        {
        }

        public void Dispose()
        {
        }
    }
}
