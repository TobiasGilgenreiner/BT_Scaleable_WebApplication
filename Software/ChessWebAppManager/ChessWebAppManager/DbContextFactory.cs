using ChessWebAppManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessWebAppManager
{
    public class DbContextFactory
    {
        public WorkerNodeContext Create()
        {
            var options = new DbContextOptionsBuilder<WorkerNodeContext>()
                .UseInMemoryDatabase("WorkerNodes")
                .Options;

            return new WorkerNodeContext(options);
        }
    }
}
