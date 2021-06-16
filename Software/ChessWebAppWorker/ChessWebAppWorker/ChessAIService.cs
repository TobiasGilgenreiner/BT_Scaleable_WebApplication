using ChessClassLib;
using System;

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
                WorkerResult workerResult = Context.WorkerResults.Find(workerInfo.WorkerID);

                workerResult.Fen = Fen.PositionToFen(PieceData.MakeMove(workerInfo.GamePosition, ChessAI.GetRandMove(workerInfo.GamePosition, workerInfo.Color)));
                workerResult.Finished = true;

                Context.Update(workerResult);
                Context.SaveChanges();
            }
        }

        public void GetNegaMaxMoveWrapper(Object stateInfo)
        {
            WorkerInfo workerInfo = (stateInfo as WorkerInfo);
            using (var Context = _dbContextFactory.Create())
            {
                WorkerResult workerResult = Context.WorkerResults.Find(workerInfo.WorkerID);
                Move newMove;
                ChessAI.NegaMax(3, ChessAI.NegInfinity, ChessAI.Infinity, workerInfo.GamePosition, workerInfo.Color, out newMove);
                workerResult.Fen = Fen.PositionToFen(PieceData.MakeMove(workerInfo.GamePosition, newMove));
                workerResult.Finished = true;

                Context.Update(workerResult);
                Context.SaveChanges();
            }
        }
    }
}
