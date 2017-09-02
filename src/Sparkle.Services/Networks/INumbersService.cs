
namespace Sparkle.Services.Networks
{
    using System;
    using Sparkle.Entities.Networks;

    public interface INumbersService
    {
        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(Number item);

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Insert(Number item);

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        System.Collections.Generic.IList<Number> SelectAll();

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        Number Update(Number item);
    }
}
