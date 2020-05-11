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
        // Group by district and state
        var stateDisctrictGrouping = parsedData
            .GroupBy(x => new {x.State, x.District})
            .Select(y => new
                {
                    State = y.Key.State,
                    District = y.Key.District,
                    Records = y.Sum( x => x.NoCases)
                }
            ).OrderByDescending(x=>x.Records).ToList();

        // Select only top 25 districts from each state
        var states = stateDisctrictGrouping.Select(y=>y.State).Distinct();        
        var districtParsedData = new CovidRecord[parsedData.Count()];
        parsedData.CopyTo(districtParsedData);
        var districtParsedDataList = districtParsedData.ToList();
        foreach(var state in states)
        {
            var bottomDistricts = stateDisctrictGrouping.Where(y=>y.State == state)
                .Select(y=>y.District).Skip(25).ToList();
            
            foreach(var district in bottomDistricts)
            {
                var count = districtParsedDataList.RemoveAll(y=>y.District == district);
            }
        }
        districtParsedDataList = districtParsedDataList.OrderBy(y=>y.DateAnnounced).ToList();
        var districtGroup = districtParsedDataList
            .GroupBy(x => new {x.DateAnnounced, x.District})
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
        // Group by district and state
        var stateDisctrictGrouping = parsedData
            .GroupBy(x => new {x.State, x.District})
            .Select(y => new
                {
                    State = y.Key.State,
                    District = y.Key.District,
                    Records = y.Sum( x => x.NoCases)
                }
            ).OrderByDescending(x=>x.Records).ToList();
        // Select only top 25 districts from each state
        var states = stateDisctrictGrouping.Select(y=>y.State).Distinct();        
        var districtParsedData = new CovidRecord[parsedData.Count()];
        parsedData.CopyTo(districtParsedData);
        var districtParsedDataList = districtParsedData.ToList();
        foreach(var state in states)
        {
            var bottomDistricts = stateDisctrictGrouping.Where(y=>y.State == state)
                .Select(y=>y.District).Skip(25).ToList();
            
            foreach(var district in bottomDistricts)
            {
                var count = districtParsedDataList.RemoveAll(y=>y.District == district);
            }
        }
        districtParsedDataList = districtParsedDataList.OrderBy(y=>y.DateAnnounced).ToList();
        List<CovidRecord> districtGroup = districtParsedDataList
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
        List<CovidRecord> stateGroup = parsedData
            .GroupBy(x => new {x.DateAnnounced, x.State})
            .Select(y => new CovidRecord
                {
                    DateAnnounced = y.Key.DateAnnounced,
                    State = y.Key.State, 
                    NoCases = y.Sum( x => x.NoCases)
                }
            ).ToList();
        // Create cumulative counts
        for(int i = 0; i < stateGroup.Count(); i++)
        {
            var group = stateGroup[i];
            var lastLocation = stateGroup.Where(x=> x.DateAnnounced < group.DateAnnounced 
                && x.State == group.State).LastOrDefault();

            if (lastLocation != null)
            {
                group.NoCases = lastLocation.NoCases + group.NoCases;
            }    
        }
        var stateGroupAnony = stateGroup.Select(x=> new 
        {
            DateAnnounced = x.DateAnnounced,
            Location = x.State,
            Records = x.NoCases  
        });
        string jsonState = JsonConvert.SerializeObject(stateGroupAnony);
        System.IO.File.WriteAllText("output/output_states_active.json", jsonState);

        // Group by country
        List<CovidRecord> countryGroup = parsedData
            .GroupBy(x => new {x.DateAnnounced})
            .Select(y => new CovidRecord
                {
                    DateAnnounced = y.Key.DateAnnounced,
                    State = "India",
                    NoCases = y.Sum( x => x.NoCases)
                }
            ).ToList();
        // Create cumulative counts
        for(int i = 0; i < countryGroup.Count(); i++)
        {
            var group = countryGroup[i];
            var lastLocation = countryGroup.Where(x=> x.DateAnnounced < group.DateAnnounced 
                && x.State == group.State).LastOrDefault();

            if (lastLocation != null)
            {
                group.NoCases = lastLocation.NoCases + group.NoCases;
            }    
        }
        var countryGroupAnony = countryGroup.Select(x=> new 
        {
            DateAnnounced = x.DateAnnounced,
            Location = x.State,
            Records = x.NoCases 
        });
    
        string jsonCountry = JsonConvert.SerializeObject(countryGroupAnony);
        System.IO.File.WriteAllText("output/output_country_active.json", jsonCountry);        
    }
}