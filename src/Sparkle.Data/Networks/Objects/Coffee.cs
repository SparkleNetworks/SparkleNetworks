
namespace Sparkle.Data.Networks.Objects
{
    using System;

    public class CoffeeRequest
    {
        public int From { get; set; }

        public int To { get; set; }

        public DateTime DateTime { get; set; }

        public string Message { get; set; }
    }

    public class ProposalRequest
    {
        public string Title { get; set; }

        public string Type { get; set; }

        public int From { get; set; }

        public int To { get; set; }

        public DateTime DateTime { get; set; }

        public string Message { get; set; }

        public string EmailTitle { get; set; }

        public string EmailText { get; set; }

        public DateTime DateProposal { get; set; }
    }
}
