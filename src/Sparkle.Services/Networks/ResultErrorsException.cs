
namespace Sparkle.Services.Networks
{
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    public class ResultErrorsException : System.Exception
    {
        private IList<IResultError> errors;

        public ResultErrorsException(IEnumerable<IResultError> errors)
            : base("Multiple domain errors occured" + Environment.NewLine + string.Join(Environment.NewLine, errors.Select(e => e.DisplayMessage)))
        {
            this.errors = errors.ToList();
        }

        public ResultErrorsException(IResultError error)
            : base(error.DisplayMessage)
        {
            this.errors = new List<IResultError>()
            {
                error,
            };
        }

        public IList<IResultError> Errors
        {
            get { return this.errors; }
        }

        protected ResultErrorsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
