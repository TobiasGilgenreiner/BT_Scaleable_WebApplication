using Newtonsoft.Json;
using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading;

namespace ApacheBenchmarkTestAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonRoot jsonRoot;
            using (StreamReader sr = new StreamReader("ServerList.json"))
            {
                string jsonString = sr.ReadToEnd();
                jsonRoot = JsonConvert.DeserializeObject<JsonRoot>(jsonString);
            }

            TestConfiguration testConfiguration;
            using (StreamReader sr = new StreamReader("TestConfiguration.json"))
            {
                string jsonString = sr.ReadToEnd();
                testConfiguration = JsonConvert.DeserializeObject<TestConfiguration>(jsonString);
            }

            List<string> generatedFiles = new List<string>();

            foreach(Platform platform in jsonRoot.Platforms)
            {
                if (!platform.PlatformName.Equals("AWS"))
                    continue;

                foreach(Server server in platform.Servers)
                {
                    if (!server.Tag.Equals("Worker"))
                        continue;

                    foreach(string endpoint in jsonRoot.Tags.First(x => x.Name.Equals(server.Tag)).Endpoints)
                    {
                        if (!jsonRoot.Tags.First(x => x.Name.Equals(server.Tag)).Endpoints.IndexOf(endpoint).Equals(0))
                            continue;

                        foreach(int requests in testConfiguration.Requests)
                        {
                            foreach (int concurrent in testConfiguration.Concurrent)
                            {
                                if (requests < concurrent)
                                    continue;

                                string csvfile = "C:/Users/Tobi/Desktop/TestDest/" + platform.PlatformName + server.Tag + server.ID + "endpoint" + jsonRoot.Tags.First(x => x.Name.Equals(server.Tag)).Endpoints.IndexOf(endpoint) + "_n" + requests + "c" + concurrent + ".csv";
                                string datafile = "C:/Users/Tobi/Desktop/TestDest/" + platform.PlatformName + server.Tag + server.ID + "endpoint" + jsonRoot.Tags.First(x => x.Name.Equals(server.Tag)).Endpoints.IndexOf(endpoint) + "_n" + requests + "c" + concurrent + ".data";
                                generatedFiles.Add(csvfile);
                                generatedFiles.Add(datafile);
                                RunApacheBenchmark("http://" + server.Url + endpoint, csvfile, datafile, requests, concurrent);
                                
                                if(server.Tag.Equals("Worker"))
                                {
                                    Console.WriteLine("------------------------------------------------");
                                    using ( var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                                    {
                                        client.BaseAddress = new Uri("http://" + server.Url);

                                        int running = -1;
                                        while (running != 0)
                                        {
                                            HttpResponseMessage response = client.GetAsync("/Worker/ActiveThreads").Result;
                                            running = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
                                            Console.Write(running + ",");
                                            Thread.Sleep(1000);
                                        }
                                    }
                                    Console.WriteLine("------------------------------------------------");
                                }
                            }
                        }
                    }
                }
            }

            //RunApacheBenchmark("http://192.168.0.70:5000/ChessAI", "C:/Users/Tobi/Desktop/TestDest/Test.csv", "C:/Users/Tobi/Desktop/TestDest/Test.data", 1000, 1);
            //Run ApacheBenchmark for every instance
            //n 100 300 600 1000 5000 10000
            //c 1 10 20 40 70 100

            //use list of files to create gnuplot script for
            // histogram mean ctime and mean dtime stacked, worker0 from each server grouped together, display all testsize groups
            // histogram worker1
            // histogram manager
            // File with meanctime and mean ttime

            // all tests with same size into linegraph pick interesting ones ttime -> Request

            //run script
        }

        public static void RunApacheBenchmark(string RequestAdress, string CSVOutputFile, string dataOutputFile, int Instances, int Concurrent)
        {
            Console.WriteLine("C:/xampp/apache/bin/ab.exe" + "-s 120 -n " + Instances + " -c " + Concurrent + " -g " + dataOutputFile + " -e " + CSVOutputFile + " " + RequestAdress);
            Process process = new Process();
            process.StartInfo.FileName = "C:/xampp/apache/bin/ab.exe";
            process.StartInfo.Arguments = "-s 120 -n " + Instances + " -c " + Concurrent + " -g " + dataOutputFile + " -e " + CSVOutputFile + " " + RequestAdress;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.Start();
            process.WaitForExit();
        }

        public static void CreateGNUPlotScript()
        {

        }
    }
}
