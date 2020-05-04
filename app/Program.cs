using System;
using System.Collections.Generic;
using CovidStudy.ViewModels;
using System.Linq;

namespace app
{
    class Program
    {
        static void Main(string[] args)
        {
            DataParser dataParser = new DataParser();
            List<CovidRecord> parsedData = dataParser.ParseData();
            parsedData = parsedData.Where( x=> (x.CurrentStatus == "Hospitalized")).ToList();

            var districtGroup = parsedData
                .GroupBy(x => new {x.DateAnnounced, x.State, x.District})
                .Select(y => new
                    {
                        DateAnnounced = y.Key.DateAnnounced,
                        State = y.Key.State,
                        District = y.Key.District,
                        Records = y.Sum( x => x.NoCases)
                    }
                );

            var stateGroup = parsedData
                .GroupBy(x => new {x.DateAnnounced, x.State})
                .Select(y => new
                    {
                        DateAnnounced = y.Key.DateAnnounced,
                        State = y.Key.State,
                        Records = y.Sum( x => x.NoCases)
                    }
                );

            var countryGroup = parsedData
                .GroupBy(x => new {x.DateAnnounced})
                .Select(y => new
                    {
                        DateAnnounced = y.Key.DateAnnounced,
                        Records = y.Sum( x => x.NoCases)
                    }
                );

            Console.WriteLine(countryGroup);
        }
    }
}
