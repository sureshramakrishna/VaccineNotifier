using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VaccineNotifier.Models
{
    public class Session
    {
        [JsonProperty("session_id")] public string SessionId { get; set; }
        [JsonProperty("date")] public string Date { get; set; }
        [JsonProperty("available_capacity")] public int AvailableCapacity { get; set; }
        [JsonProperty("min_age_limit")] public int MinAgeLimit { get; set; }
        [JsonProperty("vaccine")] public string Vaccine { get; set; }
        [JsonProperty("slots")] public List<string> Slots { get; set; }
    }
}
