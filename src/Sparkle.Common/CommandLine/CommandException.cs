
namespace Sparkle.Common.CommandLine
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Occurs when an exception occurs within a command.
    /// </summary>
    [Serializable]
    public class CommandException : Exception
    {
        public CommandException()
        {
        }

        public CommandException(string commandName, Exception inner)
            : base("Command execution failed", inner)
        {
            this.CommandName = commandName;
        }

        protected CommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string CommandName { get; private set; }
    }
}
