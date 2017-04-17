
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AspectList////<TItem>
    ////where TItem : AspectObject
    {
        private readonly List<IAspectObject> items = new List<IAspectObject>();
        private readonly Type type;

        public AspectList(Type type)
        {
            this.type = type;
        }

        public void Add(IAspectObject item)
        {
            this.items.Add(item);
        }

        public IEnumerable<IAspectObject> Values
        {
            get { return this.items.AsEnumerable(); }
        }

        public void SetSingle(IAspectObject aspect)
        {
            var aspectType = aspect.AspectManager.type;
            var itemsOfType = this.items.Where(i => i.AspectManager.type == aspectType).ToArray();
            for (int i = 0; i < itemsOfType.Length; i++)
            {
                this.items.Remove(itemsOfType[i]);
            }

            this.items.Add(aspect);
        }

        internal T GetSingle<T>()
            where T : IAspectObject
        {
            var aspectType = typeof(T);
            return (T)this.items.SingleOrDefault(a => a.AspectManager.type == aspectType);
        }
    }
}
