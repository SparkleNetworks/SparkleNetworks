
namespace Sparkle.Services.Networks
{
    using Sparkle.Data.Networks;
    using System;
    using System.Collections.Generic;

    public interface INetworksTransaction : IDisposable
    {
        IRepositoryFactory Repositories { get; }
        IServiceFactory Services { get; }
        IList<Action<INetworksTransaction>> PostSaveActions { get; }
        IDataTransaction DataTransaction { get; }

        IDisposable BeginTransaction();
        void CompleteTransaction();
        void AbortTransaction();
    }
}
