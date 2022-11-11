using Newtonsoft.Json;

namespace VaccineNotifier.Models
{
    class State
    {
        [JsonProperty("state_id")]
        public int Id;
        [JsonProperty("state_name")]
        public string Name;
    }
}
