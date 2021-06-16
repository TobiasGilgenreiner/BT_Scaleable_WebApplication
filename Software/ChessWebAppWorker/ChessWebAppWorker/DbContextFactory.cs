﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessWebAppWorker.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessWebAppWorker
{
    public class DbContextFactory
    {
        public WorkerResultContext Create()
        {
            var options = new DbContextOptionsBuilder<WorkerResultContext>()
                .UseInMemoryDatabase("WorkerResults")
                .Options;

            return new WorkerResultContext(options);
        }
    }
}
