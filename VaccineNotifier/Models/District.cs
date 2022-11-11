using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VaccineNotifier.Models
{
    class District
    {
        [JsonProperty("district_id")]
        public int Id;
        [JsonProperty("state_id")]
        public int StateId;
        [JsonProperty("district_name")]
        public string Name;
    }
}
