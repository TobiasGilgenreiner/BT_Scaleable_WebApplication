using Microsoft.EntityFrameworkCore;

namespace ChessWebAppWorker.Models
{
    public class WorkerResultContext : DbContext
    {
        private readonly DbContextOptions<WorkerResultContext> _options;
        public DbContextOptions<WorkerResultContext> Options
        {
            get
            {
                return _options;
            }
        }

        public WorkerResultContext(DbContextOptions<WorkerResultContext> options) : base(options)
        {
            _options = options;
        }

        public DbSet<ChessWebAppWorker.WorkerResult> WorkerResults { get; set; }
    }
}
