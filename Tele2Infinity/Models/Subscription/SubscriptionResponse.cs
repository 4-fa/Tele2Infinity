using Newtonsoft.Json;

namespace Tele2Infinity.Models.Subscription
{
    public class Resource
    {

        [JsonProperty("Label")]
        public string Label { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("Availability")]
        public string Availability { get; set; }
    }

    public class SubscriptionResponse
    {

        [JsonProperty("CustomerNumber")]
        public string CustomerNumber { get; set; }

        [JsonProperty("Msisdn")]
        public string Msisdn { get; set; }

        [JsonProperty("Resources")]
        public IList<Resource> Resources { get; set; }
    }


}
