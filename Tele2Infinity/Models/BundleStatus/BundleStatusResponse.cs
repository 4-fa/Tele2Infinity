using Newtonsoft.Json;

namespace Tele2Infinity.Models.BundleStatus
{
    public class Bucket
    {

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("LimitValue")]
        public int LimitValue { get; set; }

        [JsonProperty("LimitValuePresentation")]
        public string LimitValuePresentation { get; set; }

        [JsonProperty("UsedValue")]
        public int UsedValue { get; set; }

        [JsonProperty("UsedValuePresentation")]
        public string UsedValuePresentation { get; set; }

        [JsonProperty("RemainingValue")]
        public int RemainingValue { get; set; }

        [JsonProperty("RemainingValuePresentation")]
        public string RemainingValuePresentation { get; set; }

        [JsonProperty("Unit")]
        public string Unit { get; set; }

        [JsonProperty("SortOrder")]
        public string SortOrder { get; set; }

        [JsonProperty("IsUnlimitedFullSpeed")]
        public bool IsUnlimitedFullSpeed { get; set; }

        [JsonProperty("IsUnlimitedSpeedStepDown")]
        public bool IsUnlimitedSpeedStepDown { get; set; }

        [JsonProperty("IsDataRoamLikeHome")]
        public bool IsDataRoamLikeHome { get; set; }

        [JsonProperty("DoNotSumInOverview")]
        public bool DoNotSumInOverview { get; set; }

        [JsonProperty("IsDailyBundle")]
        public bool IsDailyBundle { get; set; }

        [JsonProperty("IsSsdPass")]
        public bool IsSsdPass { get; set; }

        [JsonProperty("Zones")]
        public IList<string> Zones { get; set; }

        [JsonProperty("OverrulesAlways")]
        public bool OverrulesAlways { get; set; }

        [JsonProperty("ActiveFrom")]
        public object ActiveFrom { get; set; }

        [JsonProperty("ActiveTo")]
        public object ActiveTo { get; set; }

        [JsonProperty("ReActivatePercentage")]
        public object ReActivatePercentage { get; set; }
    }

    public class Bundle
    {

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("StartDate")]
        public object StartDate { get; set; }

        [JsonProperty("ValidUntilDate")]
        public DateTime ValidUntilDate { get; set; }

        [JsonProperty("SortOrder")]
        public string SortOrder { get; set; }

        [JsonProperty("IsDayBundle")]
        public bool IsDayBundle { get; set; }

        [JsonProperty("IsFlexCaps")]
        public bool IsFlexCaps { get; set; }

        [JsonProperty("Buckets")]
        public IList<Bucket> Buckets { get; set; }
    }

    public class Resource
    {

        [JsonProperty("Label")]
        public string Label { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("Availability")]
        public string Availability { get; set; }
    }

    public class BundleStatusResponse
    {

        [JsonProperty("BalanceDate")]
        public DateTime BalanceDate { get; set; }

        [JsonProperty("NextReset")]
        public DateTime NextReset { get; set; }

        [JsonProperty("HasNewTypeMBAanvullers")]
        public bool HasNewTypeMBAanvullers { get; set; }

        [JsonProperty("IsInTheNetherlands")]
        public bool IsInTheNetherlands { get; set; }

        [JsonProperty("IsInBlockRedirect")]
        public bool IsInBlockRedirect { get; set; }

        [JsonProperty("Bundles")]
        public IList<Bundle> Bundles { get; set; }

        [JsonProperty("Resources")]
        public IList<Resource> Resources { get; set; }
    }


}
