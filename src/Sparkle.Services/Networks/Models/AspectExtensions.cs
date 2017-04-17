
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class AspectExtensions
    {
        public static TSubject SetAspect<TSubject, TAspect>(this TSubject subject, TAspect aspect)
            where TAspect : ISingleAspectOf<TSubject>
            where TSubject : IAspectObject
        {
            subject.AspectManager.SetSingle(aspect);
            return subject;
        }

        public static TAspect GetAspect<TSubject, TAspect>(this TSubject subject)
            where TAspect : IAspectObject, ISingleAspectOf<TSubject>
            where TSubject : IAspectObject
        {
            return subject.AspectManager.GetSingle<TAspect>();
        }
    }
}
