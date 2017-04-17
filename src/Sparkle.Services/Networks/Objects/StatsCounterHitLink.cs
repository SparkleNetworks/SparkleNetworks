
namespace Sparkle.Services.Networks.Objects
{
    using Newtonsoft.Json;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure.Crypto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Neutral = Sparkle.Entities.Networks.Neutral;

    public class StatsCounterHitLink
    {
        // TODO: [OSP-Security] StatsCounterHitLink.statsTrackingMotDePasse: NO. 
        private static readonly string statsTrackingMotDePasse = "12IUGBH345765ghfghv768678IUHGYUT4567NJHGYFTYCGVHJB3456iuytfdfvghikjghkjbghkjgJHBVFDErtyuIJHGVFDe456789876rf";

        [Obsolete("For deserialization only")]
        public StatsCounterHitLink()
        {
        }

        public StatsCounterHitLink(StatsCounter counter, string identifier = null, int? userId = null, int? networkId = null)
        {
            this.Category = counter.Category;
            this.Name = counter.Name;
            this.Id = counter.Id;

            this.Identifier = identifier;
            this.UserId = userId;
            this.NetworkId = networkId;
        }

        public StatsCounterHitLink(Neutral.StatsCounter counter, string identifier = null, int? userId = null, int? networkId = null)
        {
            this.Category = counter.Category;
            this.Name = counter.Name;
            this.Id = counter.Id;

            this.Identifier = identifier;
            this.UserId = userId;
            this.NetworkId = networkId;
        }

        public StatsCounterHitLink(string category, string name, int id, string identifier = null, int? userId = null, int? networkId = null)
        {
            this.Category = category;
            this.Name = name;
            this.Id = id;

            this.Identifier = identifier;
            this.UserId = userId;
            this.NetworkId = networkId;
        }

        public int Id { get; set; }

        internal string Name { get; set; }

        internal string Category { get; set; }

        public int? NetworkId { get; set; }
        public int? UserId { get; set; }
        public string Identifier { get; set; }

        public static StatsCounterHitLink FromJson(string json)
        {
            var dto = JsonConvert.DeserializeObject<StatsCounterHitLinkDto>(json);
            return dto;
        }

        public static StatsCounterHitLink FromEncryptedJson(string cryptedJson)
        {
            var decoded = SimpleCrypt.Decrypt(cryptedJson, statsTrackingMotDePasse);
            var dto = JsonConvert.DeserializeObject<StatsCounterHitLinkDto>(decoded);
            return dto;
        }

        public override string ToString()
        {
            return "Hit (ID: " + this.Identifier + " U:" + this.UserId + " N:" + this.NetworkId + ") on Counter (" + this.Id + " " + this.Category + "/" + this.Name + ")";
        }

        public string ToJson()
        {
            var json = JsonConvert.SerializeObject(new StatsCounterHitLinkDto(this), Formatting.None);
            return json;
        }

        public string ToEncryptedJson()
        {
            var encoded = SimpleCrypt.Encrypt(this.ToJson(), statsTrackingMotDePasse, true);
            return encoded;
        }

        public string ToUrlParameter()
        {
            return "tc=" + this.ToEncryptedJson();
        }

        public class StatsCounterHitLinkDto
        {
            public StatsCounterHitLinkDto()
            {
            }

            public StatsCounterHitLinkDto(StatsCounterHitLink link)
            {
                this.C = link.Id;
                this.N = link.NetworkId;
                this.U = link.UserId;
                this.I = link.Identifier;
            }

            public int C { get; set; }
            public int? N { get; set; }
            public int? U { get; set; }
            public string I { get; set; }

            public static implicit operator StatsCounterHitLink(StatsCounterHitLinkDto dto)
            {
                return new StatsCounterHitLink
                {
                    Id = dto.C,
                    Identifier = dto.I,
                    NetworkId = dto.N,
                    UserId = dto.U,
                };
            }
        }
    }
}
