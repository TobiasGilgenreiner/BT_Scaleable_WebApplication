using System.ComponentModel.DataAnnotations;

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
