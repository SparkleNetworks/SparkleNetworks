
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class StringSearchAspect : AspectObject, ISingleAspectOf<UserModel>, ISingleAspectOf<PlaceModel>
    {
        private string[] strings;
        private string content;

        public StringSearchAspect(params string[] strings)
        {
            this.strings = strings ?? new string[0];
            this.content = string.Join(" ", strings).RemoveDiacritics().ToLowerInvariant();
        }

        public bool Contains(string match)
        {
            if (string.IsNullOrEmpty(match))
                throw new ArgumentException("The value cannot be empty", "match");

            var compare = match.RemoveDiacritics().ToLowerInvariant();
            return this.content.Contains(compare);

            for (int i = 0; i < this.strings.Length; i++)
            {
                var str = strings[i].RemoveDiacritics().ToLowerInvariant();
                var ok = str.Contains(compare);
                if (ok)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(string[] words)
        {
            if (words == null)
                throw new ArgumentNullException("words");

            return words.All(w => this.content.Contains(w.RemoveDiacritics().ToLowerInvariant()));
        }
    }
}
