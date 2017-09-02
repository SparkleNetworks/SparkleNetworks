using System;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface IRelationshipService
    {
        System.Collections.Generic.IList<Relationship> SelectAll();
        Relationship SelectById(int id);
    }
}
