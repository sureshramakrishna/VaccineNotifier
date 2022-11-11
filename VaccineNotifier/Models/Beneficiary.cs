using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VaccineNotifier.Models
{
    public class Beneficiary
    {
        [JsonProperty("beneficiary_reference_id")]
        public string BeneficiaryReferenceId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("birth_year")]
        public string BirthYear { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("mobile_number")]
        public string MobileNumber { get; set; }
        [JsonProperty("photo_id_type")]
        public string PhotoIdType { get; set; }
        [JsonProperty("photo_id_number")]
        public string PhotoIdNumber { get; set; }
        [JsonProperty("comorbidity_ind")]
        public string ComorbidityInd { get; set; }
        [JsonProperty("vaccination_status")]
        public string VaccinationStatus { get; set; }
        [JsonProperty("vaccine")]
        public string Vaccine { get; set; }
        [JsonProperty("dose1_date")]
        public string Dose1Date { get; set; }
        [JsonProperty("dose2_date")]
        public string Dose2Date { get; set; }
        [JsonProperty("appointments")]
        public List<object> Appointments { get; set; }
    }
}
