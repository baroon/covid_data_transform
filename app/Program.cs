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
            parsedData = parsedData.OrderBy(x=>x.DateAnnounced).ToList();

            // Growth Data
            DataFormatter dataFormatter = new DataFormatter();
            dataFormatter.GenerateGrowthData(parsedData);

            // Active cumulative data
            dataFormatter.GenerateActiveData(parsedData);

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
