using System;
using System.Collections.Generic;
using CovidStudy.ViewModels;
using System.Linq;
using Newtonsoft.Json;

namespace app
{
    class Program
    {
        static void Main(string[] args)
        {
            DataParser dataParser = new DataParser();
            List<CovidRecord> parsedData = dataParser.ParseData();
            parsedData = parsedData.Where( x=> (x.CurrentStatus == "Hospitalized")).ToList();
            // parsedData = parsedData.Where( x=> (x.State == "Odisha")).ToList();

            // Group by district
            var districtGroup = parsedData
                .GroupBy(x => new {x.DateAnnounced, x.State, x.District})
                .Select(y => new
                    {
                        DateAnnounced = y.Key.DateAnnounced,
                        Location = y.Key.District + " [" + y.Key.State + "]",
                        Records = y.Sum( x => x.NoCases)
                    }
                );
            string jsonDistrict = JsonConvert.SerializeObject(districtGroup);
            System.IO.File.WriteAllText("output_districts_active.json", jsonDistrict);

            // Group by State
            var stateGroup = parsedData
                .GroupBy(x => new {x.DateAnnounced, x.State})
                .Select(y => new
                    {
                        DateAnnounced = y.Key.DateAnnounced,
                        Location = y.Key.State,
                        Records = y.Sum( x => x.NoCases)
                    }
                );
            string jsonState = JsonConvert.SerializeObject(stateGroup);
            System.IO.File.WriteAllText("output_states_active.json", jsonState);

            // Group by country
            var countryGroup = parsedData
                .GroupBy(x => new {x.DateAnnounced})
                .Select(y => new
                    {
                        DateAnnounced = y.Key.DateAnnounced,
                        Location = "India",
                        Records = y.Sum( x => x.NoCases)
                    }
                );
            string jsonCountry = JsonConvert.SerializeObject(countryGroup);
            System.IO.File.WriteAllText("output_country_active.json", jsonCountry);
            
            Console.WriteLine("Done!");
        }
    }
}
