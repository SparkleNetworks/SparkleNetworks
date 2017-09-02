
namespace Sparkle.Services.Networks.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Sparkle.Services.Internals;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

/*
 * Sample messages

mandrill_events=
[
  {
    "type":"blacklist",
    "action":"add",
    "reject":{
      "reason":"spam",
      "detail":null,
      "last_event_at":"2015-11-16 14:33:05",
      "email":"********.**************@gmail.com",
      "created_at":"2015-11-16 14:33:05",
      "expires_at":"2016-11-15 14:33:05",
      "expired":false,
      "subaccount":null,
      "sender": {
        "sent":113267,
        "hard_bounces":591,
        "soft_bounces":519,
        "rejects":2085,
        "complaints":4,
        "unsubs":0,
        "opens":79861,
        "clicks":125,
        "unique_opens":47321,
        "unique_clicks":101,
        "reputation":0,
        "address":"notification@sparklenetworks.net",
        "created_at":"2014-07-02 15:56:48.76423"
      }
    },
    "ts":1447684385
  }
]

*/

    public class InboundEmailModel
    {
        [JsonProperty("event")]
        [JsonConverter(typeof(StringEnumConverter))]
        public WebHookEventType Event { get; set; }
        ////public string Event { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public WebHookEventType Type { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("msg")]
        public InboundEmailMessage Msg { get; set; }
        
        [JsonProperty("ts")]
        public long Ts { get; set; }

        [JsonIgnore]
        public DateTime TimeStamp
        {
            get { return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Ts); }
        }

        [JsonProperty("reject")]
        public JsonRejectDetails Reject { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }
    }

    public class InboundEmailMessage
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("from_name")]
        public string FromName { get; set; }

        [JsonProperty("from_email")]
        public string FromEmail { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("raw_msg")]
        public string RawMsg { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("headers")]
        public IDictionary<string, object> Headers { get; set; }

        [JsonProperty("to")]
        public string[][] To { get; set; }

        [JsonProperty("spf")]
        public JsonEmailSpf Spf { get; set; }

        [JsonProperty("dkim")]
        public JsonEmailDkim Dkim { get; set; }

        [JsonProperty("sender")]
        public JsonSenderDetails Sender { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("text_flowed")]
        public bool TextFlowed { get; set; }

        [JsonProperty("spam_report")]
        public JsonEmailSpamReport SpamReport { get; set; }

        public InboundEmailMessage()
        {
            TextFlowed = false;
            Headers = new Dictionary<string, object>();
            Spf = new JsonEmailSpf();
            Dkim = new JsonEmailDkim();
        }
    }

    public class JsonRejectDetails
    {
        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("last_event_at"), JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime LastEventDate { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("created_at"), JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime CreateDate { get; set; }

        [JsonProperty("expires_at"), JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime ExpireDate { get; set; }

        [JsonProperty("subaccount")]
        public string SubAccount { get; set; }

        [JsonProperty("sender")]
        public JsonSenderDetails Sender { get; set; }
    }

    public class JsonSenderDetails
    {
        [JsonProperty("sent")]
        public int Sent { get; set; }

        [JsonProperty("hard_bounces")]
        public int HardBounces { get; set; }

        [JsonProperty("soft_bounces")]
        public int SoftBounces { get; set; }

        [JsonProperty("complaints")]
        public int Complaints { get; set; }

        [JsonProperty("unsubs")]
        public int Unsubs { get; set; }

        [JsonProperty("opens")]
        public int Opens { get; set; }

        [JsonProperty("clicks")]
        public int Clicks { get; set; }

        [JsonProperty("unique_opens")]
        public int UniqueOpens { get; set; }
        
        [JsonProperty("unique_clicks")]
        public int UniqueClicks { get; set; }

        [JsonProperty("reputation")]
        public double Reputation { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("created_at"), JsonConverter(typeof(SimpleDateTimeConverter))]
        public DateTime CreatedAt { get; set; }
    }

    public class JsonEmailSpamReport
    {
        [JsonProperty("score")]
        public float Score { get; set; }

        [JsonProperty("matched_rules")]
        public JsonEmailMatchedRules[] MatchedRules { get; set; }
    }

    public class JsonEmailMatchedRules
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("score")]
        public float Score { get; set; }
    }

    public class JsonEmailSpf
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }
    }

    public class JsonEmailDkim
    {
        [JsonProperty("signed")]
        public bool Signed { get; set; }

        [JsonProperty("valid")]
        public bool Valid { get; set; }

        public JsonEmailDkim()
        {
            Signed = false;
            Valid = false;
        }
    }

    // Report on message state after treating
    public class InboundEmailReport : BasicResult
    {
        public int ActionLevel { get; set; }

        public IList<string> Entries { get; set; }

        public string RawJson { get; set; }

        public InboundEmailMessage Message { get; set; }

        public int OnSucceedPublishId { get; set; }

        public bool OnSucceedNewItem { get; set; }

        public void Log(string toLog) { Entries.Add(++ActionLevel + "/ " + toLog); }

        public void Error(string error) { Entries.Add("ERROR: " + error); }

        public InboundEmailReport()
        {
            Entries = new List<string>();
            Message = new InboundEmailMessage();
            ActionLevel = 0;
        }

        public void Clear()
        {
            ActionLevel = 0;
            Entries.Clear();
            RawJson = "";
        }

        public ItemCaptureResult OnSucceedItemType { get; set; }
    }

    public enum ItemCaptureResult
    {
        None,
        TimelineItem,
        PrivateMessage,
    }

    public enum InboundLogType : byte
    {
        Main        = 0x0002,
        Html        = 0x0004,
        Raw         = 0x0008,
        Reply       = 0x0020,
        Noreply     = 0x0040,
    }

    // To fill for more possible handling
    public enum WebHookEventType
    {
        Unknown = 0,

        Send,
        Hard_bounce,
        Soft_bounce,
        Open,
        Click,
        Spam,
        Unsub,
        Reject,
        Deferral,
        Inbound,

        Blacklist,
    }
}
