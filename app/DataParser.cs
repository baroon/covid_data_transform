
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using CovidStudy.ViewModels;
using System;
using System.Globalization;
using System.IO;
using System.Data;
using System.Linq;

public class DataParser
{
    public List<CovidRecord> ParseGrowthData()
    {
        List<CovidRecord> records = new List<CovidRecord>();

        records.AddRange(ParseNewData());

        records = records.OrderBy(x=>x.DateAnnounced).ToList();
        return records;
    }

    private List<CovidRecord> ParseNewData()
    {
        List<CovidRecord> records = new List<CovidRecord>();

        for (int i = 3; i <= 5; i++)
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://api.covid19india.org/raw_data" + i.ToString() + ".json");
                System.IO.File.WriteAllText("raw1.json", json);
                //var json = System.IO.File.ReadAllText("raw1.json");

                QuickType.raw_data deserializedRecords = JsonConvert.DeserializeObject<QuickType.raw_data>(json);
                var rows = deserializedRecords.RawData;
                CultureInfo MyCultureInfo = new CultureInfo("hi-IN");

                foreach(Dictionary<string, string> row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row["dateannounced"]) ||  
                                    string.IsNullOrWhiteSpace(row["currentstatus"]) || 
                                    string.IsNullOrWhiteSpace(row["numcases"]))
                        continue;     

                    if (row["currentstatus"] == "Hospitalized")
                    {
                        CovidRecord covidRecord = new CovidRecord {
                            CurrentStatus =  row["currentstatus"],
                            DateAnnounced = DateTime.Parse(row["dateannounced"], MyCultureInfo),  
                            State = row["detectedstate"],
                            District = (string.IsNullOrWhiteSpace(row["detecteddistrict"]) ? "Unknown" : row["detecteddistrict"]) + " [" + row["detectedstate"] +"]",
                            NoCases = int.Parse(row["numcases"])
                        };

                        records.Add(covidRecord);
                    }
                } 
            }
        }

        return records;
    }
}
