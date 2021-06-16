using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChessWebAppWorker;

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

        public DbSet<ChessWebAppWorker.WorkerResult> WorkerResult { get; set; }
    }
}
