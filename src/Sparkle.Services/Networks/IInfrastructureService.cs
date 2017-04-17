
namespace Sparkle.Services.Networks
{
    using Sparkle.Services.Networks.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IInfrastructureService
    {
        FullCheckModel GetFullCheck(bool executeChecks);
    }
}
