using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChessWebAppWorker;
using ChessWebAppWorker.Models;
using ChessClassLib;
using System.Threading;

namespace ChessWebAppWorker.Controllers
{
    [ApiController]
    [Route("Worker")]
    public class WorkerResultsController : Controller
    {
        private readonly WorkerResultContext _context;

        public WorkerResultsController(WorkerResultContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Result")]
        public async Task<IActionResult> GetWorkerResult(int? id)
        {
            if(id == null || !WorkerResultExists((int)id))
            {
                return BadRequest("No workerresult with this id");
            }
            int tempid = (int)id;
            WorkerResult workerResult = await _context.WorkerResult.FirstAsync(x => x.WorkerID.Equals(tempid));
            
            if(workerResult.Finished)
            {
                string FenResult = workerResult.Fen;
                _context.Remove(workerResult);
                _context.SaveChanges();
                return Ok(FenResult);
            }
            else
            {
                return StatusCode(666, HttpContext.Request.Path);
            }
        }

        // POST: WorkerResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Start")]
        public async Task<IActionResult> StartNewWorker(string fenPositionString, byte color)
        {
            if (fenPositionString == null || !Fen.ISStringFen(fenPositionString) || (color != PieceData.White && color != PieceData.Black))
            {
                return BadRequest("Bad Parameters");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    WorkerResult newWorkerResult = new WorkerResult();
                    newWorkerResult.Finished = false;
                    int i = 0;
                    while (WorkerResultExists(i))
                        ++i;

                    newWorkerResult.WorkerID = i;
                    _context.Add(newWorkerResult);

                    //Start new Thread
                    WorkerInfo newWorkerInfo = new WorkerInfo(Fen.LoadPositionFromFen(fenPositionString), color, newWorkerResult.WorkerID);
                    if (ThreadPool.QueueUserWorkItem(new ChessAIService(new DbContextFactory()).GetRandMoveWrapper, newWorkerInfo))
                    {
                        await _context.SaveChangesAsync();
                        return Accepted(HttpContext.Request.PathBase + "/Worker/Result/", newWorkerInfo.WorkerID);
                    }
                    else
                    {
                        return BadRequest("Couldn't queue Task");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                
            }
            return BadRequest("Invalid ModelState");
        }

        private bool WorkerResultExists(int id)
        {
            return _context.WorkerResult.Any(x => x.WorkerID == id);
        }
    }
}
