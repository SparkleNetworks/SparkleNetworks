
namespace Sparkle.Services.Networks.Objects
{
    using Newtonsoft.Json;
    using Sparkle.Infrastructure.Crypto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ApplyInviteCode
    {
        // TODO: [OSP-Security] ApplyInviteCode.superSecretKey: NO. 
        private static readonly string superSecretKey = ;

        public ApplyInviteCode(int userId, DateTime date, int networkId)
        {
            this.UserId = userId;
            this.CreateDateUtc = date;
            this.NetworkId = networkId;
        }

        public int UserId { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public int NetworkId { get; set; }

        public static ApplyInviteCode FromHex(string hex, bool decrypt = true)
        {
            int total = hex.Length;
            byte[] bytes = new byte[total / 2];
            for (int i = 0; i < total; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            if (decrypt)
                bytes = SimpleCrypt.Decrypt(bytes, superSecretKey);
            var original = Encoding.UTF8.GetString(bytes);

            var dto = new ApplyInviteCodeDto();
            foreach (var arg in original.Split(new string[] { ";", }, StringSplitOptions.None))
            {
                int value;
                DateTime date;

                if (arg.StartsWith("U=") && int.TryParse(arg.Substring(2), out value))
                    dto.U = value;
                else if (arg.StartsWith("N=") && int.TryParse(arg.Substring(2), out value))
                    dto.N = value;
                else if (arg.StartsWith("D=") && DateTime.TryParseExact(arg.Substring(2), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                    dto.D = date;
            }

            return dto;
        }

        private string ToOriginalString()
        {
            return string.Format("U={0};D={1};N={2}",
                this.UserId,
                this.CreateDateUtc.ToString("yyyyMMdd"),
                this.NetworkId);
        }

        public string ToHex()
        {
            var bytes = Encoding.UTF8.GetBytes(this.ToOriginalString());
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        public string ToEncryptedHex()
        {
            var bytes = Encoding.UTF8.GetBytes(this.ToOriginalString());
            bytes = SimpleCrypt.Encrypt(bytes, superSecretKey);
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        public class ApplyInviteCodeDto
        {
            /// <summary>
            /// Stores the UserId
            /// </summary>
            public int? U { get; set; }

            /// <summary>
            /// Stores the CreateDateUtc
            /// </summary>
            public DateTime? D { get; set; }

            /// <summary>
            /// Stores the NetworkId
            /// </summary>
            public int? N { get; set; }

            public bool IsValid
            {
                get { return U.HasValue && N.HasValue && D.HasValue; }
            }

            public static implicit operator ApplyInviteCode(ApplyInviteCodeDto dto)
            {
                if (dto.IsValid)
                    return new ApplyInviteCode(dto.U.Value, dto.D.Value, dto.N.Value);

                return null;
            }
        }
    }
}
