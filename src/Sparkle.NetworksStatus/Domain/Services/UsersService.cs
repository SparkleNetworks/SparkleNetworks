
namespace Sparkle.NetworksStatus.Domain.Services
{
    using Sparkle.NetworksStatus.Data;
    using Sparkle.NetworksStatus.Domain.Lang;
    using Sparkle.NetworksStatus.Domain.Messages;
    using Sparkle.NetworksStatus.Domain.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Security.Cryptography;
    using System.Diagnostics;
    using System.Net;

    partial class UsersService : IUsersService
    {
        private SHA512Managed sha512;
        private Random random;
        public int Count()
        {
            return this.Repos.Users.Count();
        }

        public IList<UserModel> GetAllSortedById(int id, int pageSize)
        {
            var query = this.Repos.Users.GetAllSortedById(id, pageSize)
                .Select(u => new UserModel(u, null))
                .ToList();
            return query;
        }

        public BasicResult<EditUserError, UserModel> Create(UserModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var result = new BasicResult<EditUserError, UserModel>();

            var transaction = this.Services.NewTransaction;
            try
            {
                var now = DateTime.UtcNow;
                var emailAddress = new SrkToolkit.Common.Validation.EmailAddress(model.EmailAddressString);
                var email = new EmailAddress(emailAddress);
                email.DateCreatedUtc = now;
                transaction.EmailAddresss.Insert(email);

                var user = new User
                {
                    Country = model.Country,
                    Culture = model.Culture,
                    DateCreatedUtc = now,
                    DisplayName = model.DisplayName,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    PrimaryEmailAddressId = email.Id,
                    Status = model.Status,
                    Timezone = model.Timezone,
                };
                transaction.Users.Insert(user);

                throw new NotImplementedException();
                ////var auth = new EmailAddressAuthentication
                ////{
                ////    DateCreatedUtc = now,
                ////    IsDeleted = false,
                ////    UserId = user.Id,
                ////    EmailAddressId = email.Id,
                ////};
                ////transaction.EmailAddressAuthentications.Insert(auth);

                transaction.CompleteTransaction();
                result.Succeed = true;
                result.Data = new UserModel(user, email);
            }
            finally
            {
                transaction.Dispose();
            }

            return result;
        }

        public UserModel GetById(int id, bool withPrimaryEmail = true, bool withEmailAuths = false)
        {
            var user = this.Repos.Users.GetById(id);
            if (user == null)
                return null;

            var email = this.Repos.EmailAddresss.GetById(user.PrimaryEmailAddressId);
            var model = new UserModel(user, email);

            if (withEmailAuths)
            {
                throw new NotSupportedException();
                ////var emailAuths = this.Repos.EmailAddressAuthentications.GetByUserId(user.Id);
            }

            return model;
        }

        public IList<EmailAddressAuthenticationModel> GetEmailAddressAuthentications(int userId)
        {
            throw new NotSupportedException();
            ////var items = this.Repos.EmailAddressAuthentications.GetByUserId(userId);

            ////var emailIds = items.Select(i => i.EmailAddressId).ToArray();
            ////var emails = this.Repos.EmailAddresss.GetById(emailIds);

            ////var model = new List<EmailAddressAuthenticationModel>(items.Count);
            ////foreach (var item in items)
            ////{
            ////    model.Add(new EmailAddressAuthenticationModel(item, emails.Single(e => e.Id == item.EmailAddressId)));
            ////}

            ////return model;
        }

        public EmailAddressAuthenticationModel GetEmailAddressAuthenticationById(int id)
        {
            throw new NotSupportedException();
            ////var item = this.Repos.EmailAddressAuthentications.GetById(id);
            ////if (item == null)
            ////    return null;

            ////var email = this.Repos.EmailAddresss.GetById(item.EmailAddressId);
            ////return new EmailAddressAuthenticationModel(item, email);
        }

        public BasicResult<EmailAddressAuthenticationError, EmailAddressAuthenticationModel> CreateEmailAddressAuthentications(EmailAddressAuthenticationModel model)
        {
            throw new NotSupportedException();
            ////if (model == null)
            ////    throw new ArgumentNullException("model");

            ////var result = new BasicResult<EmailAddressAuthenticationError, EmailAddressAuthenticationModel>();

            ////var transaction = this.Services.NewTransaction;
            ////try
            ////{
            ////    var now = DateTime.UtcNow;

            ////    var email = EmailAddress.Create(model.EmailAddressString);
            ////    email.DateCreatedUtc = now;
            ////    transaction.EmailAddresss.Insert(email);

            ////    var auth = new EmailAddressAuthentication
            ////    {
            ////        DateCreatedUtc = now,
            ////        IsDeleted = false,
            ////        UserId = model.UserId,
            ////        EmailAddressId = email.Id,
            ////    };
            ////    transaction.EmailAddressAuthentications.Insert(auth);

            ////    transaction.CompleteTransaction();
            ////    result.Succeed = true;
            ////    result.Data = new EmailAddressAuthenticationModel(auth, email);
            ////}
            ////finally
            ////{
            ////    transaction.Dispose();
            ////}

            ////return result;
        }

        public BasicResult<EmailAddressAuthenticationError, EmailAddressAuthenticationModel> EditEmailAddressAuthentications(EmailAddressAuthenticationModel model)
        {
            throw new NotSupportedException();
            ////if (model == null)
            ////    throw new ArgumentNullException("model");

            ////var result = new BasicResult<EmailAddressAuthenticationError, EmailAddressAuthenticationModel>();

            ////var transaction = this.Services.NewTransaction;
            ////try
            ////{
            ////    var now = DateTime.UtcNow;

            ////    var auth = transaction.EmailAddressAuthentications.GetById(model.Id);
            ////    var email = transaction.EmailAddresss.GetById(auth.EmailAddressId);

            ////    auth.IsDeleted = model.IsDeleted;
            ////    email.IsClosed = model.EmailAddress.IsClosed;
            ////    email.DateConfirmedUtc = model.EmailAddress.DateConfirmedUtc;

            ////    transaction.EmailAddressAuthentications.Update(auth);
            ////    transaction.EmailAddresss.Update(email);

            ////    transaction.CompleteTransaction();
            ////    result.Succeed = true;
            ////    result.Data = new EmailAddressAuthenticationModel(auth, email);
            ////}
            ////finally
            ////{
            ////    transaction.Dispose();
            ////}

            ////return result;
        }

        public EmailPasswordAuthenticateResult EmailPasswordAuthenticate(EmailPasswordAuthenticateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            DateTime now = DateTime.UtcNow;
            var result = new EmailPasswordAuthenticateResult(request);

            SrkToolkit.Common.Validation.EmailAddress emailAddress;
            if ((emailAddress = SrkToolkit.Common.Validation.EmailAddress.TryCreate(request.Email)) == null)
            {
                result.Errors.Add(EmailPasswordAuthenticateError.InvalidEmailAddress, DomainStrings.ResourceManager);
                return result;
            }

            IPAddress remoteAddress;
            if (!IPAddress.TryParse(request.RemoteAddress, out remoteAddress))
            {
                result.Errors.Add(EmailPasswordAuthenticateError.InvalidRemoteAddress, DomainStrings.ResourceManager);
                return result;
            }

            var remoteAddressBytes = remoteAddress.GetAddressBytes();
            string remoteAddressLocation = null;
            try
            {
                var info = this.Services.Cache.GetIpAddressInfo(request.RemoteAddress);
                if (info != null)
                {
                    remoteAddressLocation = info.City + " ; " + info.ZipCode + " ; " + info.RegionName + " ; " + info.RegionCode + " ; " + info.CountryName + " ; " + info.CountryCode + " ; " + info.Latitude + " ; " + info.Longitude;
                }
            }
            catch (InvalidOperationException)
            {
            }

            var transaction = this.Repos.BeginTransaction();
            try
            {
                var emailEntity = transaction.EmailAddresss.Get(emailAddress);
                if (emailEntity == null)
                {
                    result.Errors.Add(EmailPasswordAuthenticateError.NoSuchEmail, DomainStrings.ResourceManager);
                    return result;
                }
                else if (emailEntity.IsClosed)
                {
                    result.Errors.Add(EmailPasswordAuthenticateError.ClosedEmailAddress, DomainStrings.ResourceManager);
                    result.EmailAddress = new EmailAddressModel(emailEntity);
                    return result;
                }
                else if (emailEntity.DateConfirmedUtc == null)
                {
                    result.Errors.Add(EmailPasswordAuthenticateError.UnconfirmedEmailAddress, DomainStrings.ResourceManager);
                    result.EmailAddress = new EmailAddressModel(emailEntity);
                    return result;
                }

                var user = transaction.Users.GetByPrimaryEmailId(emailEntity.Id);
                if (user == null)
                {
                    result.Errors.Add(EmailPasswordAuthenticateError.NoUserForEmailAddress, DomainStrings.ResourceManager);
                    return result;
                }

                var attempt = new UserAuthenticationAttempt
                {
                    DateUtc = now,
                    RemoteAddressLocation = remoteAddressLocation.TrimToLength(250),
                    RemoteAddressValue = remoteAddressBytes,
                    UserAgent = request.UserAgent.TrimToLength(250),
                    UserId = user.Id,
                };

                var end = new Func<EmailPasswordAuthenticateError, EmailPasswordAuthenticateResult>(error =>
                {
                    result.Errors.Add(error, DomainStrings.ResourceManager);

                    attempt.ErrorCode = (byte)error;
                    transaction.UserAuthenticationAttempts.Insert(attempt);
                    transaction.CompleteTransaction();

                    return result;
                });

                var passwords = transaction.UserPasswords.GetUserId(user.Id);
                if (passwords.Count == 0)
                {
                    return end(EmailPasswordAuthenticateError.NoPassword);
                }
                else if (passwords.All(p => p.DateLockedUtc != null))
                {
                    return end(EmailPasswordAuthenticateError.PasswordIsLockedOut);
                }

                if (user.Status <= 0)
                {
                    return end(EmailPasswordAuthenticateError.IsNotAuthorized);
                }

                // check password
                UserPassword validPassword = null;
                var failedPasswords = new List<UserPassword>(passwords.Count);
                var lockedPasswords = new List<UserPassword>(passwords.Count);

                foreach (var password in passwords.Where(p => p.DateLockedUtc == null))
                {
                    var isValid = this.VerifyPasswordHash(request.Password, password.PasswordType, password.PasswordValue);
                    if (isValid)
                    {
                        validPassword = password;
                        break;
                    }
                    else
                    {
                        failedPasswords.Add(password);
                    }
                }

                if (validPassword != null)
                {
                    validPassword.VerifiedUsages += 1;
                    validPassword.LastVerifiedUsageDateUtc = DateTime.UtcNow;
                    transaction.UserPasswords.Update(validPassword);
                }
                else
                {
                    foreach (var password in failedPasswords)
                    {
                        if (this.TryLockoutPassword(password, true))
                        {
                            lockedPasswords.Add(password);
                        }

                        transaction.UserPasswords.Update(password);
                    }
                }

                if (lockedPasswords.Count > 0)
                {
                    result.Errors.Add(EmailPasswordAuthenticateError.PasswordIsLockedOut, DomainStrings.ResourceManager);
                }

                if (validPassword == null)
                {
                    result.Errors.Add(EmailPasswordAuthenticateError.WrongPassword, DomainStrings.ResourceManager);
                }

                result.User = new UserModel(user, emailEntity);
                if (result.Errors.Count == 0)
                {
                    attempt.ErrorCode = 0;

                    result.Succeed = true;
                    result.LastLoginDateUtc = passwords
                        .Where(p => p.LastVerifiedUsageDateUtc != null)
                        .Max(p => p.LastVerifiedUsageDateUtc);
                }
                else
                {
                    attempt.ErrorCode = (byte)result.Errors.First().Code;
                }

                transaction.CompleteTransaction();

                return result;
            }
            finally
            {
                transaction.ClearTransaction();
            }
        }

        private bool TryLockoutPassword(UserPassword password, bool addFailureNow)
        {
            if (password == null)
                throw new ArgumentNullException("password");
            
            int lockOutCountDays = 5;////this.ServiceGroup.Configuration.UserPasswordLockOutCountDays;
            int allowedUserPasswordTentativesBeforeLockOut = 15;////this.ServiceGroup.Configuration.AllowedUserPasswordTentativesBeforeLockOut;
            var now = DateTime.UtcNow;
            bool undoLockOutCounter = password.FirstFailedTentativeDateUtc != null && password.FirstFailedTentativeDateUtc.Value.AddDays(lockOutCountDays) < now;

            if (undoLockOutCounter)
            {
                password.FirstFailedTentativeDateUtc = null;
                password.LastFailedTentativeDateUtc = null;
                password.FailedTentatives = 0;
            }

            if (addFailureNow)
            {
                if (password.FirstFailedTentativeDateUtc == null)
                    password.FirstFailedTentativeDateUtc = now;

                password.LastFailedTentativeDateUtc = now;
                password.FailedTentatives += 1;
            }

            if (password.FailedTentatives > allowedUserPasswordTentativesBeforeLockOut)
            {
                password.DateLockedUtc = now;
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Crypto

        private void ComputePasswordHash(string clearPassword, out string passwordFormat, out string passwordResult)
        {
            passwordFormat = "md10";
            var hashed = this.Md11(clearPassword);
            passwordResult = Convert.ToBase64String(hashed);
        }

        private bool VerifyPasswordHash(string clearPassword, string passwordFormat, string passwordResult)
        {
            if (passwordFormat == "md10")
            {
                return this.VerifyPasswordHashMd10(clearPassword, Convert.FromBase64String(passwordResult));
            }
            else
            {
                throw new ArgumentException("Password format is not supported");
            }
        }

        private bool VerifyPasswordHashMd10(string clearPassword, byte[] md10Password)
        {
            //                               //
            //    ninja hash algorithm       //
            //                               //
            // clear password | random bytes //
            //      ↓                   ↓    //
            // xxxxxxxxxxxxx          zzzzz  // unicode bytes
            //      ↓                   ↓    // 3x SHA 512
            //    ####1   →   +   ←   ####2  // concatenate hashes
            //                ↓              // 
            //             #######           // bytes
            //                ↓              // 7x SHA 512
            //          ####2 + ######       // concatenate
            //                ↓              // 5x SHA 512
            //         ###### + ####2        // concatenate
            //                ↓              // 11x SHA 512
            //         ###### + ####2        // concatenate
            //                               //

            var clearPasswordBytes = Encoding.Unicode.GetBytes(clearPassword);
            var salt = md10Password.Skip(512 / 8).ToArray();

            var computed = this.Md11(clearPasswordBytes, salt);
            return computed.SequenceEqual(md10Password);
        }

        private byte[] CreateMd11Salt()
        {
            var sha = this.sha512 ?? (this.sha512 = new SHA512Managed());

            var random = this.random ?? (this.random = new Random());
            var randomSize = random.Next(111, 222);
            var randomBytes = new byte[randomSize];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(randomBytes);
            randomBytes = sha.ComputeHash(randomBytes);
            randomBytes = sha.ComputeHash(randomBytes);
            randomBytes = sha.ComputeHash(randomBytes);
            randomBytes = sha.ComputeHash(randomBytes);
            randomBytes = sha.ComputeHash(randomBytes);
            return randomBytes;
        }

        private byte[] Md11(string clearPassword)
        {
            var sha = this.sha512 ?? (this.sha512 = new SHA512Managed());

            var clearPasswordBytes = Encoding.Unicode.GetBytes(clearPassword);

            var randomBytes = this.CreateMd11Salt();

            return this.Md11(clearPasswordBytes, randomBytes);
        }

        private byte[] Md11(byte[] clearPasswordBytes, byte[] randomBytes)
        {
            var sha = this.sha512 ?? (this.sha512 = new SHA512Managed());
            Debug.Assert(randomBytes.Length == 64);

            var data1 = sha.ComputeHash(clearPasswordBytes);
            data1 = sha.ComputeHash(data1);
            data1 = sha.ComputeHash(data1);
            ////var data2 = sha.ComputeHash(randomBytes);
            ////data2 = sha.ComputeHash(data2);
            ////data2 = sha.ComputeHash(data2);
            var data2 = randomBytes;

            var data = data1.CombineWith(data2);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);

            data = data2.CombineWith(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);

            data = data.CombineWith(data2);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);
            data = sha.ComputeHash(data);

            data = data.CombineWith(data2);
            return data;
        }

        #endregion
    }
}
