using System;
using System.Collections.Generic;
using CovidStudy.ViewModels;
using System.Linq;
using Newtonsoft.Json;
using System.Data;

public class DataFormatter
{
    internal void GenerateGrowthData(List<CovidRecord> parsedData)
    {
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
        System.IO.File.WriteAllText("output/output_districts_growth.json", jsonDistrict);

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
        System.IO.File.WriteAllText("output/output_states_growth.json", jsonState);

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
        System.IO.File.WriteAllText("output/output_country_growth.json", jsonCountry);
        
        //States list
        List<string> uniqueStatesList = parsedData
            .GroupBy(p => new {p.State} )
            .Select(g => g.First().State)
            .ToList();
        string jsonStatesList = JsonConvert.SerializeObject(uniqueStatesList);
        System.IO.File.WriteAllText("output/states_names.json", jsonStatesList);
    }

    internal void GenerateActiveData(List<CovidRecord> parsedData)
    {
        // Group by district - convert to List and non Anonymous type
        List<CovidRecord> districtGroup = parsedData
            .GroupBy(x => new {x.DateAnnounced, x.State, x.District})
            .Select(y => new CovidRecord
                {
                    DateAnnounced = y.Key.DateAnnounced,
                    District = y.Key.District,
                    NoCases = y.Sum( x => x.NoCases)
                }
            ).ToList();
        // Create cumulative counts
        for(int i = 0; i < districtGroup.Count(); i++)
        {
            var group = districtGroup[i];
            var lastLocation = districtGroup.Where(x=> x.DateAnnounced < group.DateAnnounced 
                && x.District == group.District).LastOrDefault();

            if (lastLocation != null)
            {
                group.NoCases = lastLocation.NoCases + group.NoCases;
            }    
        }
        var districtGroupAnony = districtGroup.Select(x=> new 
        {
            DateAnnounced = x.DateAnnounced,
            Location = x.District,
            Records = x.NoCases  
        });
        string jsonDistrict = JsonConvert.SerializeObject(districtGroupAnony);
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
    }
}