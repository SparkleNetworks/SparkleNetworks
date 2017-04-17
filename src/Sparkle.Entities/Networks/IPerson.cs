
namespace Sparkle.Entities.Networks
{
    using System;

    public interface IPerson
    {
        Guid UserId { get; set; }
        string Email { get; set; }
        string Username { get; set; }
    }
}
