
namespace Sparkle.Entities.Networks.Neutral
{
    using System;

    public class Tag : ITag
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public DateTime Date { get; set; }
    }
}
