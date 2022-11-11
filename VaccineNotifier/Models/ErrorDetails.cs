using Newtonsoft.Json;

namespace VaccineNotifier.Models
{
    public class ErrorDetails
    {
        [JsonProperty("errorCode")]
        public string ErrorCode;
        [JsonProperty("error")]
        public string Error;
    }
}
