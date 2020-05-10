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
            //List<CovidRecord> parsedGrowthData = dataParser.ParseGrowthData();
            List<CovidRecord> parsedCumulativeData = dataParser.ParseCumulativeData();
            
            //parsedData = FillBlankValues(parsedGrowthData);
            parsedCumulativeData = FillBlankValues(parsedCumulativeData);

            // Growth Data
            DataFormatter dataFormatter = new DataFormatter();
            //dataFormatter.GenerateGrowthData(parsedCumulativeData);

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
    }
}
