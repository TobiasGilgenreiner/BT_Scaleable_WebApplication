using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuPlotAutomation
{
    class HistoDataBundleRow
    {
        public string HostTag { get; set; }
        public int N { get; set; }
        public int C { get; set; }
        public double MeanCTime { get; set; }
        public double MeanDTime { get; set; }

        public HistoDataBundleRow(string hostTag, int n, int c, double meanCTime, double meanDTime)
        {
            HostTag = hostTag;
            N = n;
            C = c;
            MeanCTime = meanCTime;
            MeanDTime = meanDTime;
        }
    }
}
