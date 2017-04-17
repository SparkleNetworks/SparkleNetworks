
#if !SSC
namespace Sparkle.Infrastructure.Crypto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class SimpleSecrets
    {
        public static string NewLongRandom
        {
            // TODO: [OSP-Security] SimpleSecrets.NewLongRandom: do not use Guid, use RNG crypto service
            get { return (Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()).Replace("-", ""); }
        }

        public static string NewMediumRandom
        {
            // TODO: [OSP-Security] SimpleSecrets.NewMediumRandom: do not use Guid, use RNG crypto service
            get { return (Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()).Replace("-", ""); }
        }

        public static string NewShortRandom
        {
            // TODO: [OSP-Security] SimpleSecrets.NewShortRandom: do not use Guid, use RNG crypto service
            get { return Guid.NewGuid().ToString().Replace("-", ""); }
        }

        // TODO: [OSP-Security] SimpleSecrets.GetUsersEmailNotificationActionHash: The result should not be publicly predictable. 
        public static string GetUsersEmailNotificationActionHash(int userId, int networkId)
        {
            var toHash = "UsersEmailNotificationActionHash" + userId + "HelloWorld" + networkId + "SecretKeyHashIsAwesome";
            using (var hasher = SHA256.Create())
            {
                var bytes = Encoding.Unicode.GetBytes(toHash);
                for (int i = 0; i < 8; i++)
                {
                    bytes = hasher.ComputeHash(bytes);
                }

                bytes = bytes.Reverse().ToArray();
                for (int i = 0; i < 16; i++)
                {
                    bytes = hasher.ComputeHash(bytes);
                }

                var userBytes = BitConverter.GetBytes(userId);
                bytes[9] = userBytes[0];
                bytes[13] = userBytes[1];
                bytes[23] = userBytes[2];
                bytes[29] = userBytes[3];

                toHash = Convert.ToBase64String(bytes);
            }

            return toHash;
        }

        public static int? VerifyUsersEmailNotificationActionHash(int networkId, string hash)
        {
            byte[] bytes;
            try
            {
                if (string.IsNullOrEmpty(hash))
                    throw new ArgumentException("The value cannot be empty", "hash");
                if ((bytes = Convert.FromBase64String(hash)).Length != 32)
                    return null;
            }
            catch (FormatException ex) { return null; }
            catch (ArgumentException ex) { return null; }

            var userId = BitConverter.ToInt32(new byte[] { bytes[9], bytes[13], bytes[23], bytes[29], }, 0);

            var reHash = SimpleSecrets.GetUsersEmailNotificationActionHash(userId, networkId);
            if (hash != reHash)
                return null;

            return userId;
        }
    }
}
#endif
