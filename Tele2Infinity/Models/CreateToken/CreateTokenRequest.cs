using Newtonsoft.Json;

namespace Tele2Infinity.Models.CreateToken
{
    public class CreateTokenRequest
    {

        [JsonProperty("AuthorizationCode")]
        public string AuthorizationCode { get; set; }
    }
}
