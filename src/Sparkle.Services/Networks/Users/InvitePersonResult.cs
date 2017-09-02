
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public class InvitePersonResult
    {
        public ResultCode Code { get; set; }

        public Company Company { get; set; }

        public Exception Error { get; set; }

        public static InvitePersonResult QuotaReached
        {
            get { return new InvitePersonResult { Code = ResultCode.QuotaReached, }; }
        }

        public static InvitePersonResult UserExists
        {
            get { return new InvitePersonResult { Code = ResultCode.UserExists, }; }
        }

        public static InvitePersonResult NoCompany
        {
            get { return new InvitePersonResult { Code = ResultCode.NoCompany, }; }
        }

        public static InvitePersonResult AlreadyInvited
        {
            get { return new InvitePersonResult { Code = ResultCode.AlreadyInvited, }; }
        }

        public static InvitePersonResult SmtpError(Exception ex)
        {
            return new InvitePersonResult { Code = ResultCode.SmtpError, Error = ex, };
        }

        public static InvitePersonResult OtherError(Exception ex)
        {
            return new InvitePersonResult { Code = ResultCode.Error, Error = ex, };
        }

        public Invited Invitation { get; set; }

        public enum ResultCode
        {
            Done = 0,
            InvalidAddress,
            UserExists,
            AlreadyInvited,
            NoCompany,
            SmtpError,
            NotAuthorized,
            QuotaReached,
            CreateCompanyRequestSend,
            Error,
        }
    }
}
