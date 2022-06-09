using Newtonsoft.Json;

namespace Tele2Infinity.Models.RoamingBundles
{
    public class Bundle
    {

        [JsonProperty("buyingCode")]
        public string BuyingCode { get; set; }
    }

    public class RoamingBundlesRequest
    {

        [JsonProperty("bundles")]
        public IList<Bundle> Bundles { get; set; }
    }
}
