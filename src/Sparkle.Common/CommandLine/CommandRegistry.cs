
namespace Sparkle.Common.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    /// <summary>
    /// Permits to register commands, expose them through a console and execute them.
    /// </summary>
    /// <typeparam name="TParams"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    public class CommandRegistry<TParams, TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Contains the registered commands.
        /// Indexed by the commands uppercased name.
        /// </summary>
        private readonly Dictionary<string, Container<TCommand>> registry = new Dictionary<string, Container<TCommand>>();

        public CommandRegistry()
        {
        }

        /// <summary>
        /// Gets or sets the standard error output stream.
        /// </summary>
        public TextWriter Out { get; set; }

        /// <summary>
        /// Gets or sets the standard input stream.
        /// </summary>
        public TextReader In { get; set; }

        /// <summary>
        /// Gets or sets the standard output stream.
        /// </summary>
        public TextWriter Error { get; set; }

        /// <summary>
        /// Set this in an inherited class to define the way a command is executed.
        /// </summary>
        protected Action<Container<TCommand>, TCommand, TParams> Executor { get; set; }

        /// <summary>
        /// Registers a command.
        /// </summary>
        /// <param name="name">the command name</param>
        /// <param name="description">the description</param>
        /// <param name="commandFactory">a delegate to construct the command object</param>
        /// <exception cref="ArgumentException">the name is empty</exception>
        /// <exception cref="ArgumentNullException">the constructor is null</exception>
        public void Register(string name, string description, Func<TCommand> commandFactory, string help = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The value cannot be empty", "name");
            if (commandFactory == null)
                throw new ArgumentNullException("commandFactory");

            this.registry.Add(name.ToUpperInvariant(), new Container<TCommand>
            {
                Name = name,
                Description = description,
                Ctor = commandFactory,
                Help = help,
            });
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="name">the command name</param>
        /// <param name="options">the options</param>
        /// <exception cref="ArgumentNullException">if any argument is null</exception>
        /// <exception cref="ArgumentException">if the command could not be found</exception>
        /// <exception cref="InvalidOperationException">if the executor is not configured or the command could not be instantiated</exception>
        /// <exception cref="CommandException">if the command fails during execution</exception>
        public void Execute(string name, TParams options)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (options == null)
                throw new ArgumentNullException("options");
            if (this.Executor == null)
                throw new InvalidOperationException("This registry contains no executor.");

            name = name.ToUpperInvariant();

            if (name == "COMMANDS")
            {
                this.PrintCommands();
                return;
            }

            if (name == "HELP")
            {
                this.PrintCommandHelp(options);
                return;
            }

            if (!this.registry.ContainsKey(name))
                throw new ArgumentException("No such command", "name");

            TCommand instance = default(TCommand);
            var container = this.registry[name];
            try
            {
                instance = this.CreateCommand(name, container, options);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not create the command", ex);
            }

            if (instance == null)
                throw new InvalidOperationException("Could not create the command");

            try
            {
                this.Executor(container, instance, options);
            }
            catch (Exception ex)
            {
                throw new CommandException(name, ex);
            }
        }

        protected virtual void PrintCommandHelp(TParams options)
        {
        }

        public IEnumerable<Container<TCommand>> Commands
        {
            get
            {
                foreach (var item in this.registry)
                {
                    yield return item.Value;
                }
            }
        }

        /// <summary>
        /// Override this to change the way the command is instantiated or to attach objects to it.
        /// Default implementation instantiates the command.
        /// </summary>
        /// <param name="name">the command name</param>
        /// <param name="options">the options</param>
        /// <param name="container"></param>
        /// <returns>the command ready for execution</returns>
        protected virtual TCommand CreateCommand(string name, Container<TCommand> container, TParams options)
        {
            var command = this.registry[name].Ctor();

            command.Error = this.Error;
            command.In = this.In;
            command.Out = this.Out;

            return command;
        }

        protected virtual void PrintCommands()
        {
            var nameMax = Math.Max(this.registry.Values.Max(v => v.Name.Length), 16);
            Out.WriteLine("Available commands: ");
            Out.WriteLine();
            foreach (var cmd in this.registry.Values)
            {
                Out.WriteLine("{0,-" + nameMax + "} {1}", cmd.Name, cmd.Description);
            }
        }
    }

    public class Container<TCommand>
        where TCommand : ICommand
    {
        internal Func<TCommand> Ctor { get; set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public string Help { get; internal set; }
    }
}
