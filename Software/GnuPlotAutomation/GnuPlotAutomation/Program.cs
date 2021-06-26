using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace GnuPlotAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            FilterData filterData;
            using (StreamReader sr = new StreamReader("FilterData.json"))
            {
                string jsonString = sr.ReadToEnd();
                filterData = JsonConvert.DeserializeObject<FilterData>(jsonString);
            }
            
            List<string> FileList = Directory.GetFiles(@"C:\Users\Tobi\Desktop\TestDest", "*.data", SearchOption.AllDirectories).ToList();

            if (false)
            {
                List<List<string>> HistoPlotLists = new List<List<string>>();

                foreach (string serverTag in filterData.ServerTags)
                {
                    foreach (int c in filterData.Cs)
                    {
                        List<string> tempList = new List<string>();

                        tempList.AddRange(FileList.Where(x => x.Contains(serverTag) && x.Contains("c" + c + ".data")));

                        if (tempList.Any())
                        {
                            List<HistoDataBundleRow> histoDataBundleRows = new List<HistoDataBundleRow>();
                            foreach (string file in tempList)
                            {
                                string output = GetMeanFromGnuPlot(file.Replace('\\', '/'));
                                string[] array = output.Split("\r\n");
                                int ctime_mean = 0;
                                int dtime_mean = 0;
                                if (!array.Any(x => x.Contains("warning")))
                                {
                                    double test1 = Convert.ToDouble(array[0].Replace('.', ','));
                                    double test2 = Convert.ToDouble(array[1].Replace('.', ','));
                                    ctime_mean = Convert.ToInt32(test1);
                                    dtime_mean = Convert.ToInt32(test2);
                                }
                                string tempHostTag = "";
                                int tempn = Convert.ToInt32(file.Split("_").First(x => x.Contains("c" + c + ".data")).Replace("n", "").Split("c").First());

                                foreach (string HostTag in filterData.Tags)
                                {
                                    if (file.Contains(HostTag))
                                    {
                                        tempHostTag = HostTag;
                                    }
                                }
                                histoDataBundleRows.Add(new HistoDataBundleRow(tempHostTag, tempn, c, ctime_mean, dtime_mean));
                            }

                            histoDataBundleRows.OrderBy(x => x.HostTag).ThenBy(x => x.N);

                            List<string> DataFiles = new List<string>();

                            string dataFilePath = "C:/Users/Tobi/Desktop/TestDest/HistoData/MainDataFile" + serverTag + "_c" + c + ".data";
                            DataFiles.Add(dataFilePath);
                            using (StreamWriter sw = new StreamWriter(dataFilePath))
                            {
                                sw.WriteLine("Host n c meanctime meandtime");
                                foreach (HistoDataBundleRow row in histoDataBundleRows)
                                {
                                    sw.WriteLine(row.HostTag + " " + row.N + " " + row.C + " " + row.MeanCTime + " " + row.MeanDTime);
                                }
                            }

                            foreach (int n in filterData.Ns)
                            {
                                string groupDataFilePath = "C:/Users/Tobi/Desktop/TestDest/HistoData/SecondaryDataFile" + serverTag + "_n" + n + "_c" + c + ".data";
                                DataFiles.Add(groupDataFilePath);
                                using (StreamWriter sw = new StreamWriter(groupDataFilePath))
                                {
                                    sw.WriteLine("Host n c meanctime meandtime");
                                    foreach (HistoDataBundleRow histoDataBundleRow in histoDataBundleRows.Where(x => x.N.Equals(n)))
                                    {
                                        sw.WriteLine(histoDataBundleRow.HostTag + " " + histoDataBundleRow.N + " " + histoDataBundleRow.C + " " + histoDataBundleRow.MeanCTime + " " + histoDataBundleRow.MeanDTime);
                                    }
                                }
                            }

                            CreateHistogramScript(DataFiles, filterData.Ns, c, filterData.Tags, serverTag);

                            HistoPlotLists.Add(tempList);
                        }
                    }

                } 
            }


            //Linegraph //RPI
            if (true)
            {
                List<List<string>> LinePlotLists = new List<List<string>>();
                foreach (string serverTag in filterData.ServerTags)
                {
                    foreach (string hostTag in filterData.Tags)
                    {
                        foreach (int n in filterData.Ns)
                        {
                            List<string> tempList = new List<string>();
                            foreach (int c in filterData.Cs)
                            {
                                string element = FileList.FirstOrDefault(x => x.Contains(serverTag) && x.Contains(hostTag) && x.Contains("n" + n) && x.Contains("c" + c + ".data"));

                                if (element != null)
                                    tempList.Add(element);
                            }

                            if (tempList.Any())
                            {
                                CreateLineGraphscript(tempList, n, filterData.Cs, serverTag, hostTag);
                                LinePlotLists.Add(tempList);
                            }

                        }
                    }

                } 
            }
        }

        public static void CreateHistogramScript(List<string> Files, List<int> Ns, int c, List<string> HostTags, string ServerTag)
        {
            List<string> tempList = new List<string>();

            foreach (string path in Files)
            {
                tempList.Add(path.Replace('\\', '/'));
            }

            Files = tempList;

            if (Files.Count != 6)
                Console.WriteLine();

            string basestring = "#Set output\nset terminal png\nset output \"[OUTPUTSTRING]\"\n\n#Set Diagrammstyle\nset style fill solid border - 1\nset style data histogram\nset style histogram rowstack\nset boxwidth 0.8\nset style histogram rowstack title offset 0,1\nset grid nopolar\nset grid noxtics\n\n#GetStats\nstats \"[MAINDATAFILE]\" using 4 name \"A\" nooutput\nstats \"[MAINDATAFILE]\" using 5 name \"B\" nooutput\nset yrange[0:(A_max + B_max) + 30]\nset xtics rotate by -90\n\n#Layout\nset title font \",16\" \"Durchschnittliche Anfragedauer ([C] gleichzeitige Anfragen)\"\nset ylabel font \",16\" \"Zeit [ms]\"\nset xlabel font \",16\" \"Anzahl an Anfragen n\" offset 0,-2\nset key top left reverse\n\n\n#Plot\nplot [N1][N2][N3][N4][N5]";
            string nplotstring = "newhistogram \"n = [N]\", \\\n	\"[SPLITDATAFILE]\" using 4:xticlabels(1) lc rgbcolor \"#E69F00\" title \"[TITLE1]\",\\\n	\"[SPLITDATAFILE]\" using 5:xticlabels(1) lc rgbcolor \"#0072B2\" title \"[TITLE2]\",\\\n";

            string ScriptPath = "C:/Users/Tobi/Desktop/TestDest/HistoData/HistogramScript" + ServerTag + "_c" + c + ".p";

            basestring = basestring.Replace("[C]", c.ToString());
            basestring = basestring.Replace("[OUTPUTSTRING]", "C:/Users/Tobi/Desktop/TestDest/HistoData/Histogram" + ServerTag + "_c" + c + ".png");
            basestring = basestring.Replace("[MAINDATAFILE]", Files.Where(x => x.Contains("MainDataFile")).First());

            Ns.Sort();
            bool first = true;
            int i = 1;
            foreach (int n in Ns)
            {
                string tempstring = "";

                if (n < c)
                {
                    basestring = basestring.Replace("[N" + i + "]", "");
                    ++i;
                    continue;
                }
                else
                {
                    tempstring = nplotstring.Replace("[N]", n.ToString());
                }

                tempstring = tempstring.Replace("[SPLITDATAFILE]", Files.Where(x => x.Contains("_n" + n + "_c" + c + ".data")).First());


                if (first)
                {
                    tempstring = tempstring.Replace("[TITLE1]", "durchschnittlicher Verbindungsaufbau");
                    tempstring = tempstring.Replace("[TITLE2]", "durchschnittliche Antwort");
                    first = false;
                }
                else
                {
                    tempstring = tempstring.Replace("[TITLE1]", "");
                    tempstring = tempstring.Replace("[TITLE2]", "");
                }
                basestring = basestring.Replace("[N" + i +"]", tempstring);

                ++i;
            }

            basestring = basestring.Remove(basestring.Length - 2);

            using (StreamWriter sw = new StreamWriter(ScriptPath))
            {
                sw.Write(basestring);
            }

            CallGnuPlot(ScriptPath);
        }

        public static void CreateLineGraphscript(List<string> Files, int N, List<int> Cs, string ServerTag, string HostTag)
        {
            List<string> tempList = new List<string>();

            foreach(string path in Files)
            {
                tempList.Add(path.Replace('\\', '/'));
            }

            Files = tempList;
            Files.Sort();

            if (Files.Count != 5)
                Console.WriteLine();

            string directoryName = Path.GetDirectoryName(Files.First()) + "\\";
            directoryName = directoryName.Replace('\\', '/');
            Cs.Sort();

            string baseString = "set terminal png\nset output \"[OUTPUTFILE]\"\nset grid nopolar\n\nset yrange[0:*]\n\nset title font \",16\" \"[TAG] [SERVERTAG] Stress Test ([N] Anfragen)\"\nset ylabel font \",16\" \"Zeit [ms]\"\nset xlabel font \",16\" \"Anfrage\"\nset key top left reverse\nplot [FCS0][FCS1][FCS2][FCS3][FCS4]";

            string fileConfigurationString = "\"[FILEPATH]\" using 9 lc rgbcolor \"[COLOR]\" with lines title \"" + "[C] Gleichzeitig\",\\\n";

            string[] colors = { "black", "purple", "green", "orange", "red" };

            int i = 0;
            foreach(string file in Files)
            {
                string tempstring = fileConfigurationString.Replace("[FILEPATH]", file);
                tempstring = tempstring.Replace("[COLOR]", colors[i]);
                tempstring = tempstring.Replace("[C]", Cs[i].ToString());

                baseString = baseString.Replace("[FCS" + i + "]", tempstring);

                ++i;
            }

            for(; i <= 4; ++i)
            {
                baseString = baseString.Replace("[FCS" + i + "]", "");
            }

            baseString = baseString.Replace("[TAG]", HostTag);
            baseString = baseString.Replace("[SERVERTAG]", ServerTag);
            baseString = baseString.Replace("[N]", N.ToString());
            baseString = baseString.Replace("[OUTPUTFILE]", directoryName + HostTag + ServerTag + N + ".png");

            baseString = baseString.Remove(baseString.Length - 2);

            string scriptPath = directoryName + HostTag + ServerTag + N + ".p";

            using (StreamWriter sw = new StreamWriter(scriptPath))
            {
                sw.Write(baseString);
            }

            CallGnuPlot(scriptPath);
        }

        public static void CallGnuPlot(string ScriptPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Program Files\gnuplot\bin\gnuplot.exe";
            process.StartInfo.Arguments = ScriptPath;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.Start();
            process.WaitForExit();
        }

        public static string GetMeanFromGnuPlot(string SourceData)
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Program Files\gnuplot\bin\gnuplot.exe";
            process.StartInfo.Arguments = "-e \"SourceFile='" + SourceData + "'\"" + " ReturnMeanctimeanddtime.p";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            StreamReader sr = process.StandardOutput;
            string output = sr.ReadToEnd();
            process.WaitForExit();

            return output;
        }
    }
}
