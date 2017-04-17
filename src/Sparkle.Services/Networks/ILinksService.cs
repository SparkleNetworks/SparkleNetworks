using System;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface ILinksService
    {
        System.Collections.Generic.IList<Link> SelectAll();
        System.Collections.Generic.IList<Link> SelectWithId(int userId);
    }
}
