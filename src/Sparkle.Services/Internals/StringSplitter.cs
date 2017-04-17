
namespace Sparkle.Services.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StringSplitter
    {
        public IList<Split1> Split(string input)
        {
            var seps = new char[] { ' ', '\t', '\r', '\n', };
            var ponctuation = new char[] { ',', '?', '.', ';', '!', '\'', };
            var parts = new List<Split1>();
            int start = 0;
            Split1Type context = Split1Type.Start;
            var GetType = new Func<char, Split1Type>(c => seps.Contains(c) ? Split1Type.Whitespace : ponctuation.Contains(c) ? Split1Type.Ponctuation : Split1Type.Word);
            for (int i = 0; i <= input.Length; i++)
            {
                // match
                char c = i < input.Length ? input[i] : ' ';
                Split1Type current = GetType(c);

                // 
                if (current != context || current == Split1Type.Ponctuation && i > 0 && c == input[i - 1])
                {
                    if (context != Split1Type.Start)
                    {
                        parts.Add(new Split1(input.Substring(start, i - start), context));
                        start = i;
                    }
                }

                context = current;
            }

            return parts;
        }
    }

    public class Split1
    {
        public Split1(string value, Split1Type type)
        {
            this.Value = value;
            this.SplitType = type;
        }

        public string Value { get; set; }

        public Split1Type SplitType { get; set; }

        public bool Ignore
        {
            get { return this.SplitType == Split1Type.Start || this.SplitType == Split1Type.Whitespace; }
        }

        public override string ToString()
        {
            return this.Value ?? string.Empty;
        }
    }

    public enum Split1Type
    {
        Start,
        Word,
        Ponctuation,
        Whitespace,
    }
}
