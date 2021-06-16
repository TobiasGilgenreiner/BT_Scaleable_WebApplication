using ChessClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulWebApplication.Models
{
    public class AIMove
    {
        public UInt32 CalculationTime { get; set; }
        public Int32 Evaluation { get; set; } 
        public Move Move { get; set; }
    }
}
