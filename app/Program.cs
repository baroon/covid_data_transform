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
            List<CovidRecord> parsedGrowthData = dataParser.ParseGrowthData();
            List<CovidRecord> parsedCumulativeData = dataParser.ParseCumulativeData();

            parsedGrowthData = FillBlankValues(parsedGrowthData);
            parsedCumulativeData = FillBlankValues(parsedCumulativeData);

            // Growth Data
            DataFormatter dataFormatter = new DataFormatter();
            dataFormatter.GenerateGrowthData(parsedGrowthData);

            // Cumulative Data
            dataFormatter.GenerateActiveData(parsedCumulativeData);

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
            parsedData = parsedData.OrderBy(x=>x.DateAnnounced).ToList();      
            return parsedData; 
        }

        private static List<CovidRecord> RemoveDistrictsWithLowCounts(List<CovidRecord> parsedData)
        {   
            var districtGroup = parsedData
                .GroupBy(x => new {x.District})
                .Select(y => new
                    {
                        District = y.Key.District, 
                        Records = y.Sum( x => x.NoCases)
                    }
                ).Where(x=>x.Records <= 3);

            foreach(var district in districtGroup)
            {
                parsedData.RemoveAll(x=>x.District == district.District);
            }

            return parsedData;
        }
    }
}
