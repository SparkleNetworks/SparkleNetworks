
namespace Sparkle.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class CompositeTextWriter : TextWriter
    {
        private readonly TextWriter[] writers;

        public CompositeTextWriter(params TextWriter[] writers)
        {
            this.writers = writers;
        }

        public override Encoding Encoding
        {
            get { throw new NotSupportedException(); }
        }

        public override void Close()
        {
            this.Do(x => x.Close());
        }

        protected override void Dispose(bool disposing)
        {
            this.Do(x => x.Dispose());
        }

        public override void Flush()
        {
            this.Do(x => x.Flush());
        }

        public override void Write(char value)
        {
            this.Do(x => x.Write(value));
        }

        private void Do(Action<TextWriter> action)
        {
            foreach (var item in this.writers)
            {
                action(item);
            }
        }
    }
}
