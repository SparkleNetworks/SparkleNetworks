
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class AspectObject : IAspectObject
    {
        ////private readonly Dictionary<Type, Aspect> items = new Dictionary<Type, Aspect>();
        private readonly AspectList root;

        public AspectObject()
        {
            this.root = new AspectList(this.GetType());
        }

        public AspectList AspectManager
        {
            get { return this.root; }
        }

        public IEnumerable<IAspectObject> Aspects
        {
            get { return this.root.Values; }
        }
    }
}
