using System;

namespace Sparkle.Data
{
    /// <summary>
    /// Marks an interface as a repository for code generation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    sealed class RepositoryAttribute : Attribute
    {
        /// <summary>
        /// Marks an interface as a repository for code generation.
        /// </summary>
        public RepositoryAttribute()
        {
        }

        /// <summary>
        /// Marks an interface as a repository for code generation.
        /// </summary>
        public RepositoryAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
