using ChessClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessWebAppWorker
{
    public class ChessAIService
    {
        DbContextFactory _dbContextFactory;

        public ChessAIService(DbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void GetRandMoveWrapper(Object stateInfo)
        {
            WorkerInfo workerInfo = (stateInfo as WorkerInfo);
            using (var Context = _dbContextFactory.Create())
            {
                WorkerResult workerResult = Context.WorkerResult.Find(workerInfo.WorkerID);

                workerResult.Fen = Fen.PositionToFen(PieceData.MakeMove(workerInfo.GamePosition, ChessAI.GetRandMove(workerInfo.GamePosition, workerInfo.Color)));
                workerResult.Finished = true;

                Context.Update(workerResult);
                Context.SaveChanges();
            }
        }
    }
}
