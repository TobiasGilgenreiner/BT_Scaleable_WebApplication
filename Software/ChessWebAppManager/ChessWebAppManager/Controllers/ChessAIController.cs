using ChessClassLib;
using Microsoft.AspNetCore.Mvc;
using RESTfulWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RESTfulWebApplication.Controllers
{
    [ApiController]
    [Route("Chess")]
    public class ChessAIController : Controller
    {
        [HttpGet]
        [Route("ChessAI")]
        public IActionResult GetAIMove(string fenPositionString)
        {
            if(fenPositionString == null || !Fen.ISStringFen(fenPositionString))
            {
                return BadRequest();
            }

            int ID = 0;

            //TBD give work to the workernodes
            //TBD return worker Uri
            return Accepted(new Uri("https://localhost:44396/Chess/Worker/" + ID));
        }
    }
}
