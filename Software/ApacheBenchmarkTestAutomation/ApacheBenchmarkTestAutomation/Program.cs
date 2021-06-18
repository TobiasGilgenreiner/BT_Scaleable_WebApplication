using Newtonsoft.Json;
using System;
using System.IO;
using System.Diagnostics;

namespace ApacheBenchmarkTestAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonRoot myDeserializedClass;
            using (StreamReader sr = new StreamReader("ServerList.json"))
            {
                string jsonString = sr.ReadToEnd();
                myDeserializedClass = JsonConvert.DeserializeObject<JsonRoot>(jsonString);
            }

            RunApacheBenchmark("http://192.168.0.70:5000/ChessAI", "C:/Users/Tobi/Desktop/TestDest/Test.csv", "C:/Users/Tobi/Desktop/TestDest/Test.data", 1000, 1);
            //Run ApacheBenchmark for every instance
            //n 100 300 600 1000 5000 10000
            //c 1 10 20 40 70 100

            //use list of files to create gnuplot script for
            // histogram mean ctime and mean dtime stacked, worker0 from each server grouped together, display all testsize groups
            // histogram worker1
            // histogram manager
            // all tests with same size into linegraph pick interesting ones ttime -> Request

            //run script
        }

        public static void RunApacheBenchmark(string RequestAdress, string CSVOutputFile, string dataOutputFile, int Instances, int Concurrent)
        {
            Process process = new Process();
            process.StartInfo.FileName = "C:/xampp/apache/bin/ab.exe";
            process.StartInfo.Arguments = "-n " + Instances + " -c " + Concurrent + " -g " + dataOutputFile + " -e " + CSVOutputFile + " " + RequestAdress;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.Start();
            process.WaitForExit();
        }

        public static void CreateGNUPlotScript()
        {

        }
    }
}
