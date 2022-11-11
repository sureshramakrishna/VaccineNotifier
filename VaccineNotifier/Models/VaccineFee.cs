using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VaccineNotifier.Models
{
    public class VaccineFee
    {
        [JsonProperty("vaccine")] public string Vaccine { get; set; }
        [JsonProperty("fee")] public string Fee { get; set; }
    }
}
