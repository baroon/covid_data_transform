using System;
using Newtonsoft.Json;

namespace CovidStudy.ViewModels 
{
    public class CovidRecord
    {
        [JsonProperty("currentstatus")]
        public string CurrentStatus { get; set; }

        [JsonProperty("dateannounced")]
        public DateTime DateAnnounced { get; set; }

        [JsonProperty("detectedstate")]
        public string State { get; set; }

        [JsonProperty("detecteddistrict")]
        public string District { get; set; }
    }
}