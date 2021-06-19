using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuPlotAutomation
{
    class FilterData
    {
        public List<string> ServerTags { get; set; }
        public List<string> Tags { get; set; }
        public List<int> Ns { get; set; }
        public List<int> Cs { get; set; }
    }
}
