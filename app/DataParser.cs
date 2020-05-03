
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using CovidStudy.ViewModels;
using System;
using System.Globalization;

public class DataParser
{
    public List<CovidRecord> ParseData()
    {
        List<CovidRecord> records = new List<CovidRecord>();

        using (WebClient wc = new WebClient())
        {
            var json = wc.DownloadString("https://api.covid19india.org/raw_data3.json");
            QuickType.raw_data deserializedRecords = JsonConvert.DeserializeObject<QuickType.raw_data>(json);
            var rows = deserializedRecords.RawData;
            CultureInfo MyCultureInfo = new CultureInfo("hi-IN");

            foreach(Dictionary<string, string> row in rows)
            {
               if (string.IsNullOrWhiteSpace(row["dateannounced"]))
                    continue;     

                CovidRecord covidRecord = new CovidRecord {
                    CurrentStatus =  row["currentstatus"],
                    DateAnnounced = DateTime.Parse(row["dateannounced"], MyCultureInfo),  
                    State = row["detectedstate"],
                    District = row["detecteddistrict"]
                };
                if (string.IsNullOrWhiteSpace(covidRecord.District))
                    covidRecord.District = "Unknown";

                records.Add(covidRecord);
            } 
        }

        return records;
    }
}