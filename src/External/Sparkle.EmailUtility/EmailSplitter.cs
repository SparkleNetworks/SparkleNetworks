
namespace Sparkle.EmailUtility
{
    using HtmlAgilityPack;
    using Sparkle.EmailUtility.Models;
    using Sparkle.EmailUtility.Splitters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class EmailSplitter
    {
        private static IEnumerable<ISplitter> splitters;

        public MessageBodyParts GetMessageFromEmailHtml(string html, string[] genericRules = null)
        {
            return LoopOnSplitters(html, genericRules);
        }

        /// <summary>
        /// Get all types that implement ISplitter using reflection
        /// </summary>
        /// <returns>The splitter types as an enumerable of ISplitter</returns>
        private IEnumerable<ISplitter> GetAllSplitters()
        {
            if (splitters == null)
                splitters = (from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.GetInterfaces().Contains(typeof(ISplitter)) && t.GetConstructor(Type.EmptyTypes) != null
                    select Activator.CreateInstance(t) as ISplitter).ToList();
            return splitters;
        }

        private MessageBodyParts LoopOnSplitters(string html, string[] genericRules)
        {
            var model = new MessageBody(html, genericRules);
            var splitters = GetAllSplitters().OrderBy(o => o.Priority).ToList();
            foreach (var splitter in splitters)
            {
                if (splitter.IsMatch(model))
                {
                    return splitter.Process(model);
                }
            }

            throw new OperationCanceledException("There is no splitter defined or they cannot be retrieved");
        }
    }
}
