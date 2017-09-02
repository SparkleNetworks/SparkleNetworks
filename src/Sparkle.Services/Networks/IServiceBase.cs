
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface IServiceBase : IDisposable
    {
        IList<string> OptionsList { get; set; }
        ServiceIdentity Identity { get; set; }
    }
}
