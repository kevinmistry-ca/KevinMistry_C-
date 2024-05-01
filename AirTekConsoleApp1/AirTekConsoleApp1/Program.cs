using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using static Program;

public class Program
{
    public static void Main(string[] args)
    {        
        using (StreamReader r = new StreamReader("C:\\Users\\Kevin.Mistry\\OneDrive - GEP\\Desktop\\Kevin\\Personal\\Air-Tek\\AirTekConsoleApp1\\AirTekConsoleApp1\\coding-assigment-orders.json"))
        {
            string json = r.ReadToEnd();
            JObject orders = JsonConvert.DeserializeObject<JObject>(json);
            //Console.WriteLine(orders);            
            var schedule = printFlightSchedule();
            printFlightItineraries(orders, schedule);
        }

    }

    public static JArray printFlightSchedule()
    {
        JArray schedule = new JArray();
        JArray daySchedule = new JArray();               

        schedule.Add(new JObject()
        {
            { "day", 1 },
            { "flightId","1"},
            { "departure", "YUL" },
            { "arrival", "YYZ" }

        });
        schedule.Add(new JObject()
        {
            { "day", 1 },
            { "flightId","2"},
            { "departure", "YUL" },
            { "arrival", "YYC" }

        });
        schedule.Add(new JObject()
        {
            { "day", 1 },
            { "flightId","3"},
            { "departure", "YUL" },
            { "arrival", "YVR" }

        });
        schedule.Add(new JObject()
        {
            { "day", 2 },
            { "flightId","4"},
            { "departure", "YUL" },
            { "arrival", "YYZ" }

        });
        schedule.Add(new JObject()
        {
            { "day", 2 },
            { "flightId","5"},
            { "departure", "YUL" },
            { "arrival", "YYC" }

        });
        schedule.Add(new JObject()
        {
            { "day", 2 },
            { "flightId","6"},
            { "departure", "YUL" },
            { "arrival", "YVR" }

        });

        Console.WriteLine(schedule);
        return schedule;
    }

    public static void printFlightItineraries(JObject orders, JArray schedule)
    {
        int yyzCount = 20; int yycCount = 20;  int yvrCount = 20;
        JArray flightItinerary = new JArray();        

        foreach (var item in orders)
        {
            string key = item.Key;
            var dest = (string)orders[key]["destination"];            
            var flight = schedule.FirstOrDefault(x => (string)x["arrival"] == dest);
            var arrival = checkArrival(dest);
            if(flight != null && arrival != "")
            {
                flightItinerary.Add(new JObject()
                {
                    {"order",  key},
                    {"flightNumber", (string)flight["flightId"] },
                    {"departure", (string)flight["departure"] },
                    {"arrival", arrival },
                    {"day", (string)flight["day"] }
                });
            }
            else
            {
                flightItinerary.Add(new JObject()
                {
                    {"order",  key},
                    {"flightNumber", "not scheduled" }                    
                });
            }                        
        }

        Console.WriteLine(flightItinerary) ;
    }

    static string checkArrival(string dest)
    {
        string arrival = "";
        int yyzCount = 20; int yycCount = 20; int yvrCount = 20;
        if (dest == "YYZ" && yyzCount > 0)
        {
            arrival = dest;
            yyzCount--;
        } 
        else if (dest == "YYC" && yycCount > 0)
        {
            arrival = dest;
            yycCount--;
        }
        else if (dest == "YVR" && yvrCount > 0)
        {
            arrival = dest;
            yvrCount--;
        }
        return arrival;
    }
}