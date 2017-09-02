
namespace Sparkle.Entities.Networks
{
    /// <summary>
    /// Represents a data object with a integer primary key named "Id".
    /// </summary>
    public interface IEntityInt32Id
    {
        /// <summary>
        /// The primary key of this data entity.
        /// </summary>
        int Id { get; set; }
    }
}
