using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnuPlotAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> Filelist = Directory.GetFiles(@"C:\Users\Tobi\Desktop\TestDest\ManagerEndpoint0", "*.data", SearchOption.AllDirectories).ToList();

            //Manager Done
            List<string> ManagerEndpoint0c1 =  Filelist.Where(x => x.Contains("Manager") && x.Contains("c1.data")).ToList();
            List<string> ManagerEndpoint0c10 = Filelist.Where(x => x.Contains("Manager") && x.Contains("c10.data")).ToList();
            List<string> ManagerEndpoint0c20 = Filelist.Where(x => x.Contains("Manager") && x.Contains("c20.data")).ToList();
            List<string> ManagerEndpoint0c30 = Filelist.Where(x => x.Contains("Manager") && x.Contains("c30.data")).ToList();
            List<string> ManagerEndpoint0c50 = Filelist.Where(x => x.Contains("Manager") && x.Contains("c50.data")).ToList();

            //Worker0Endpoint0
            List<string> Worker0Endpoint0c1 =  Filelist.Where(x => x.Contains("Worker0endpoint0") && x.Contains("c1.data")).ToList();
            List<string> Worker0Endpoint0c10 = Filelist.Where(x => x.Contains("Worker0endpoint0") && x.Contains("c10.data")).ToList();
            List<string> Worker0Endpoint0c20 = Filelist.Where(x => x.Contains("Worker0endpoint0") && x.Contains("c20.data")).ToList();

            //Worker0Endpoint1
            List<string> Worker0Endpoint1c1 =  Filelist.Where(x => x.Contains("Worker0endpoint1") && x.Contains("c1.data")).ToList();
            List<string> Worker0Endpoint1c10 = Filelist.Where(x => x.Contains("Worker0endpoint1") && x.Contains("c10.data")).ToList();
            List<string> Worker0Endpoint1c20 = Filelist.Where(x => x.Contains("Worker0endpoint1") && x.Contains("c20.data")).ToList();
            List<string> Worker0Endpoint1c30 = Filelist.Where(x => x.Contains("Worker0endpoint1") && x.Contains("c30.data")).ToList();
            List<string> Worker0Endpoint1c50 = Filelist.Where(x => x.Contains("Worker0endpoint1") && x.Contains("c50.data")).ToList();

            //Worker1Endpoint0
            List<string> Worker1Endpoint0c1 =  Filelist.Where(x => x.Contains("Worker1endpoint0") && x.Contains("c1.data")).ToList();
            List<string> Worker1Endpoint0c10 = Filelist.Where(x => x.Contains("Worker1endpoint0") && x.Contains("c10.data")).ToList();
            List<string> Worker1Endpoint0c20 = Filelist.Where(x => x.Contains("Worker1endpoint0") && x.Contains("c20.data")).ToList();

            //Worker1Endpoint1
            List<string> Worker1Endpoint1c1 =  Filelist.Where(x => x.Contains("Worker1endpoint1") && x.Contains("c1.data")).ToList();
            List<string> Worker1Endpoint1c10 = Filelist.Where(x => x.Contains("Worker1endpoint1") && x.Contains("c10.data")).ToList();
            List<string> Worker1Endpoint1c20 = Filelist.Where(x => x.Contains("Worker1endpoint1") && x.Contains("c20.data")).ToList();
            List<string> Worker1Endpoint1c30 = Filelist.Where(x => x.Contains("Worker1endpoint1") && x.Contains("c30.data")).ToList();
            List<string> Worker1Endpoint1c50 = Filelist.Where(x => x.Contains("Worker1endpoint1") && x.Contains("c50.data")).ToList();
        }

        public static void CallGnuPlot(string ScriptPath)
        {

        }
    }
}
