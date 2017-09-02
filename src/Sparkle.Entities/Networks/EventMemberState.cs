
namespace Sparkle.Entities.Networks
{
    public enum EventMemberState
    {
        None = int.MinValue,

        /// <summary>
        /// 0: needs action or invited
        /// </summary>
        IsInvited = 0,

        /// <summary>
        /// 1: present
        /// </summary>
        HasAccepted = 1,

        /// <summary>
        /// 2: tentative
        /// </summary>
        MaybeJoin = 2,

        /// <summary>
        /// 3: won't come
        /// </summary>
        WontCome = 3,

        /// <summary>
        /// 4: WTF?
        /// </summary>
        WantJoin = 4,
    }
}
