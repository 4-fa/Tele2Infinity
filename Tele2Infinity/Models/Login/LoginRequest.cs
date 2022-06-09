using Newtonsoft.Json;

namespace Tele2Infinity.Models.Login
{
    public class LoginRequest
    {

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Scope")]
        public string Scope { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("ClientId")]
        public string ClientId { get; set; }
    }
}
