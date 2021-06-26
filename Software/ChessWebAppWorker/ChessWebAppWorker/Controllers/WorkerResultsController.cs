using ChessClassLib;
using ChessWebAppWorker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        [Route("ActiveThreads")]
        public ActionResult GetNumberofActiveTasks()
        {
            return Ok(_context.WorkerResults.Where(x => x.Finished == false).Count());
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
            WorkerResult workerResult = await _context.WorkerResults.FirstAsync(x => x.WorkerID.Equals(tempid));
            
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
        [HttpGet]
        [Route("Start")]
        public async Task<IActionResult> StartNewWorker(string fenPositionString, byte color, ChessAI.OponentAlgorith oponent)
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
                    _context.SaveChanges();

                    bool WorkerQueued = false; 
                    if(oponent.Equals(ChessAI.OponentAlgorith.Rand))
                    {
                        WorkerQueued = ThreadPool.QueueUserWorkItem(new ChessAIService(new DbContextFactory()).GetRandMoveWrapper, newWorkerInfo);
                    }
                    else if(oponent.Equals(ChessAI.OponentAlgorith.NegaMax))
                    {
                        WorkerQueued = ThreadPool.QueueUserWorkItem(new ChessAIService(new DbContextFactory()).GetNegaMaxMoveWrapper, newWorkerInfo);
                    }

                    if (WorkerQueued)
                    {
                        return Accepted(HttpContext.Request.PathBase + "/Worker/Result/", newWorkerInfo.WorkerID);
                    }
                    else
                    {
                        _context.Remove(newWorkerInfo);
                        _context.SaveChanges();
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
            return _context.WorkerResults.Any(x => x.WorkerID == id);
        }
    }
}
