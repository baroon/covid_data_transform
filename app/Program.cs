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
            
            parsedData = FillBlankValues(parsedData);

            // Group by district
            var districtGroup = parsedData
                .GroupBy(x => new {x.DateAnnounced, x.State, x.District})
                .Select(y => new
                    {
                        DateAnnounced = y.Key.DateAnnounced,
                        Location = y.Key.District,
                        Records = y.Sum( x => x.NoCases)
                    }
                );

            string jsonDistrict = JsonConvert.SerializeObject(districtGroup);
            System.IO.File.WriteAllText("output/output_districts_active.json", jsonDistrict);

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
            System.IO.File.WriteAllText("output/output_states_active.json", jsonState);

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
            System.IO.File.WriteAllText("output/output_country_active.json", jsonCountry);
            
            Console.WriteLine("Done!");
        }

        private static List<CovidRecord> FillBlankValues(List<CovidRecord> parsedData)
        {
            List<CovidRecord> uniqueDistrictList = parsedData
                .GroupBy(p => new {p.District} )
                .Select(g => g.First())
                .ToList();

            List<CovidRecord> uniqueDateList = parsedData
                .GroupBy(p => new {p.DateAnnounced} )
                .Select(g => g.First())
                .ToList();

            foreach(var date in uniqueDateList){
                foreach(var district in uniqueDistrictList){
                    if (!parsedData.Exists(x => 
                        x.DateAnnounced == date.DateAnnounced && x.District == district.District)){
                            parsedData.Add(
                                new CovidRecord { 
                                        DateAnnounced = date.DateAnnounced,  
                                        State = district.State, 
                                        District = district.District,
                                        CurrentStatus = "Hospitalised",
                                        NoCases = 0
                                    }
                        );
                    }
                }        
            }            
            return parsedData; 
        }
    }
}
