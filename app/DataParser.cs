
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

        records.AddRange(ParseOlderData());
        records.AddRange(ParseOldData());
        records.AddRange(ParseNewData());

        records = records.OrderBy(x=>x.DateAnnounced).ToList();
        return records;
    }

    private List<CovidRecord> ParseNewData()
    {
        List<CovidRecord> records = new List<CovidRecord>();

        using (WebClient wc = new WebClient())
        {
            var json = wc.DownloadString("https://api.covid19india.org/raw_data4.json");
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

        return records;
    }
    
    private List<CovidRecord> ParseOldData()
    {
        List<CovidRecord> records = new List<CovidRecord>();
        var json = System.IO.File.ReadAllText("raw.json");

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

        return records;
    }

    private List<CovidRecord> ParseOlderData()
    {
        List<CovidRecord> records = new List<CovidRecord>();

        for(int i = 1 ; i < 27; i++)
        {
            StreamReader sr = new StreamReader("input/patients_data_2020-04-" + i + ".csv");
            string[] headers = sr.ReadLine().Split(','); 
            DataTable dt = new DataTable();
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }
            while (!sr.EndOfStream)
            {
                string[] rows = sr.ReadLine().Split(',');
                DataRow dr = dt.NewRow();
                for (int j = 0; j < headers.Length; j++)
                {
                    dr[j] = rows[j];
                }
                dt.Rows.Add(dr);
            }

            string selectedDate = "/04/2020";
            if (i < 10)
                selectedDate = "0" + i.ToString() + "/04/2020";
            else
                selectedDate = i.ToString() + "/04/2020";

            var selectedRows = from row in dt.AsEnumerable()
            where row.Field<string>("date_announced").Trim() == selectedDate
            select row;

            foreach(DataRow row in selectedRows)
            {
                if (row["current_status"].ToString() == "Hospitalized")
                {
                    CovidRecord record = new CovidRecord
                    {
                        CurrentStatus = "Hospitalized",
                        DateAnnounced = new DateTime(2020, 4, i),
                        State = row["detected_state"].ToString(),
                        District = (string.IsNullOrWhiteSpace(row["detected_district"].ToString()) ? "Unknown" : row["detected_district"].ToString()) + " [" + row["detected_state"].ToString() +"]",
                        NoCases = 1
                    };
                    records.Add(record);
                }
            }
        }

        return records;
    }

    public List<CovidRecord> ParseCumulativeData()
    {
        List<CovidRecord> records = new List<CovidRecord>();
        CultureInfo MyCultureInfo = new CultureInfo("hi-IN");

        StreamReader sr = new StreamReader("input/patients_data_2020-04-1.csv");
        string[] headers = sr.ReadLine().Split(','); 
        DataTable dt = new DataTable();
        foreach (string header in headers)
        {
            if (header == "date_announced")
                dt.Columns.Add(header, System.Type.GetType("System.DateTime"));
            else
                dt.Columns.Add(header);
        }
        while (!sr.EndOfStream)
        {
            string[] rows = sr.ReadLine().Split(',');
            DataRow dr = dt.NewRow();
            for (int j = 0; j < headers.Length; j++)
            {
                if (headers[j] == "date_announced")
                    dr[j] = DateTime.Parse(rows[j], MyCultureInfo);  
                else
                    dr[j] = rows[j];
            }

            dt.Rows.Add(dr);
        }

        var selectedRows = from row in dt.AsEnumerable()
        where row.Field<DateTime>("date_announced") <= DateTime.Parse("31/03/2020", MyCultureInfo)
        select row;

        foreach(DataRow row in selectedRows)
        {
            if (row["current_status"].ToString() == "Hospitalized")
            {
                CovidRecord record = new CovidRecord
                {
                    CurrentStatus = "Hospitalized",
                    DateAnnounced = Convert.ToDateTime(row["date_announced"]),
                    State = row["detected_state"].ToString(),
                    District = (string.IsNullOrWhiteSpace(row["detected_district"].ToString()) ? "Unknown" : row["detected_district"].ToString()) + " [" + row["detected_state"].ToString() +"]",
                    NoCases = 1
                };
                records.Add(record);
            }
        }

        var growthData = ParseGrowthData();
        records.AddRange(growthData);

        records = records.OrderBy(x=>x.DateAnnounced).ToList();
        return records;
    }
}
