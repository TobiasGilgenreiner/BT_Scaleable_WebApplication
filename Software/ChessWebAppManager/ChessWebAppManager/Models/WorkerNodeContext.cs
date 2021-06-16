using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ChessWebAppManager.Models
{
    public class WorkerNodeContext : DbContext
    {
        private readonly DbContextOptions<WorkerNodeContext> _options;
        public DbContextOptions<WorkerNodeContext> Options
        {
            get
            {
                return _options;
            }
        }

        public WorkerNodeContext(DbContextOptions<WorkerNodeContext> options) : base(options)
        {
            _options = options;
        }

        public DbSet<WorkerNode> WorkerNodes{ get; set; }
    }
}
