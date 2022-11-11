using System.Collections.Generic;
using Newtonsoft.Json;

namespace VaccineNotifier.Models
{
    public class Center
    {
        [JsonProperty("center_id")] public int Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("address")] public string Address { get; set; }
        [JsonProperty("state_name")] public string StateName { get; set; }
        [JsonProperty("district_name")] public string DistrictName { get; set; }
        [JsonProperty("block_name")] public string BlockName { get; set; }
        [JsonProperty("pincode")] public int Pincode { get; set; }
        [JsonProperty("lat")] public int Latitude { get; set; }
        [JsonProperty("long")] public int Longitude { get; set; }
        [JsonProperty("from")] public string From { get; set; }
        [JsonProperty("to")] public string To { get; set; }
        [JsonProperty("fee_type")] public string FeeType { get; set; }
        [JsonProperty("sessions")] public List<Session> Sessions { get; set; }
        [JsonProperty("vaccine_fees")] public List<VaccineFee> VaccineFees { get; set; }
    }
}

