
namespace Sparkle.WebBase.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IBaseResponse
    {
        string ErrorMessage { get; }

        string ErrorCode { get; }

        string Exception { get; }

        Dictionary<string, string[]> ModelState { get; }
    }
}
