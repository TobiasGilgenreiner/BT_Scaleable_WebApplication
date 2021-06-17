using ChessWebAppManager.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace ChessWebAppManager.Controllers
{
    public class WorkerNodesController : Controller
    {
        private readonly WorkerNodeContext _context;

        public WorkerNodesController(WorkerNodeContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ChessAI")]
        public IActionResult GetAIChessWorker()
        {
            List<WorkerNode> workerNodes= _context.WorkerNodes.ToList();

            WorkerNode freeworkerNode = null;
            int minActiveTasks = int.MaxValue;

            
                foreach (WorkerNode element in workerNodes)
                {
                    using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                    {
                        client.BaseAddress = new Uri(element.Uri);
                        HttpResponseMessage response = client.GetAsync("/Worker/ActiveThreads").Result;
                        int activethreads = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);

                        if (response.IsSuccessStatusCode && minActiveTasks > activethreads)
                        {
                            freeworkerNode = element;
                            minActiveTasks = activethreads;
                        }
                    }
                }
            

            return Ok(freeworkerNode.Uri + "/Worker/Start");
        }

        private bool WorkerNodeExists(int id)
        {
            return _context.WorkerNodes.Any(e => e.ID == id);
        }
    }
}
