using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChessWebAppWorker
{
    public class WorkerResult
    {
        [Key]
        public int WorkerID { get; set; }

        public bool Finished { get; set; }
        public string Fen { get; set; }
    }
}
