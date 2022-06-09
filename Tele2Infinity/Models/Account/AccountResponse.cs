using Newtonsoft.Json;

namespace Tele2Infinity.Models.Account
{
    public class Resource
    {

        [JsonProperty("Label")]
        public string Label { get; set; }

        [JsonProperty("Url")]
        public string? Url { get; set; }

        [JsonProperty("Availability")]
        public string Availability { get; set; }
    }

    public class AccountResponse
    {

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Brand")]
        public string Brand { get; set; }

        [JsonProperty("VisitorKeyForExternals")]
        public string VisitorKeyForExternals { get; set; }

        [JsonProperty("IsEligibleForMigration")]
        public bool IsEligibleForMigration { get; set; }

        [JsonProperty("IsCompanyAdmin")]
        public bool IsCompanyAdmin { get; set; }

        [JsonProperty("IsTwoFactorAuthenticationEnabled")]
        public bool IsTwoFactorAuthenticationEnabled { get; set; }

        [JsonProperty("ContactId")]
        public string ContactId { get; set; }

        [JsonProperty("Resources")]
        public IList<Resource> Resources { get; set; }

        [JsonProperty("IsEligibleForFixedAutoAdd")]
        public bool IsEligibleForFixedAutoAdd { get; set; }
    }
}
