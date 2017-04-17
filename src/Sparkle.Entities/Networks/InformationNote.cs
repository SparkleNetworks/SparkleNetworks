
namespace Sparkle.Entities.Networks
{
    partial class InformationNote : IEntityInt32Id, INetworkEntity
    {
        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }

    public class InformationNotesFilter
    {
        public InformationNotesKnownFilter Filter { get; set; }
    }

    public enum InformationNotesKnownFilter
    {
        /// <summary>
        /// Do not filter, list everything.
        /// </summary>
        All,

        /// <summary>
        /// Display only currently active items.
        /// </summary>
        Active,
    }

    public struct InformationNoteOptions
    {
    }
}
