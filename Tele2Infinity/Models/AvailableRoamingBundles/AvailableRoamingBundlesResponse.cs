using Newtonsoft.Json;

namespace Tele2Infinity.Models.AvailableRoamingBundles
{
    public class Bundle
    {

        [JsonProperty("Zones")]
        public IList<string> Zones { get; set; }

        [JsonProperty("BundleCode")]
        public string BundleCode { get; set; }

        [JsonProperty("BuyingCode")]
        public string BuyingCode { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Size")]
        public int Size { get; set; }

        [JsonProperty("SizePresentation")]
        public string SizePresentation { get; set; }

        [JsonProperty("SpeedStepDown")]
        public object SpeedStepDown { get; set; }

        [JsonProperty("OriginalPriceExclVat")]
        public int OriginalPriceExclVat { get; set; }

        [JsonProperty("OriginalPriceExclVatPresentation")]
        public string OriginalPriceExclVatPresentation { get; set; }

        [JsonProperty("OriginalPriceInclVat")]
        public int OriginalPriceInclVat { get; set; }

        [JsonProperty("OriginalPriceInclVatPresentation")]
        public string OriginalPriceInclVatPresentation { get; set; }

        [JsonProperty("PriceExclVat")]
        public int PriceExclVat { get; set; }

        [JsonProperty("PriceExclVatPresentation")]
        public string PriceExclVatPresentation { get; set; }

        [JsonProperty("PriceInclVat")]
        public int PriceInclVat { get; set; }

        [JsonProperty("PriceInclVatPresentation")]
        public string PriceInclVatPresentation { get; set; }

        [JsonProperty("HasDiscount")]
        public bool HasDiscount { get; set; }

        [JsonProperty("IsRecurring")]
        public bool IsRecurring { get; set; }

        [JsonProperty("IsPayForUse")]
        public bool IsPayForUse { get; set; }

        [JsonProperty("ValidityInHours")]
        public object ValidityInHours { get; set; }

        [JsonProperty("ValidUntil")]
        public object ValidUntil { get; set; }

        [JsonProperty("SortOrder")]
        public string SortOrder { get; set; }

        [JsonProperty("IsSsdPass")]
        public bool IsSsdPass { get; set; }

        [JsonProperty("BuyingCodeAliases")]
        public IList<string> BuyingCodeAliases { get; set; }

        [JsonProperty("IsDayBundle")]
        public bool IsDayBundle { get; set; }
    }

    public class AvailableRoamingBundlesResponse
    {

        [JsonProperty("Bundles")]
        public IList<Bundle> Bundles { get; set; }
    }
}
