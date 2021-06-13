using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChessClassLib
{
    public static class ChessAI
    {
        public static int Infinity = 99999;
        public static int NegInfinity = -Infinity;

        public static List<Move> GetAllPossibleMoves(byte[] gamePosition, byte color, bool CheckForCheck = false)
        {
            List<Move> PossibleMoves = new List<Move>();

            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    if (!((gamePosition[x + y * 8] & color) != 0))
                        continue;

                    if ((gamePosition[x + y * 8] & PieceData.Pawn) != 0)
                        PossibleMoves.AddRange(PieceData.GetPawnMoves(gamePosition, new Vector2Int(x, y)));

                    if ((gamePosition[x + y * 8] & PieceData.Knight) != 0)
                        PossibleMoves.AddRange(PieceData.GetKnightMoves(gamePosition, new Vector2Int(x, y)));

                    if ((gamePosition[x + y * 8] & PieceData.Bishop) != 0)
                        PossibleMoves.AddRange(PieceData.GetBishopMoves(gamePosition, new Vector2Int(x, y)));

                    if ((gamePosition[x + y * 8] & PieceData.Rook) != 0)
                        PossibleMoves.AddRange(PieceData.GetRookMoves(gamePosition, new Vector2Int(x, y)));

                    if ((gamePosition[x + y * 8] & PieceData.Queen) != 0)
                        PossibleMoves.AddRange(PieceData.GetQueenMoves(gamePosition, new Vector2Int(x, y)));

                    if ((gamePosition[x + y * 8] & PieceData.King) != 0)
                        PossibleMoves.AddRange(PieceData.GetKingMoves(gamePosition, new Vector2Int(x, y)));
                }
            }

            foreach (Move move in PossibleMoves)
            {
                move.StartPiece += color;
                move.EndPiece += color;
            }

            if (CheckForCheck)
            {
                byte EnemyColor = (color.Equals(PieceData.White)) ? (PieceData.Black) : (PieceData.White);
                List<Move> CheckingMoves = PieceData.GetChecks(gamePosition, color, GetAllPossibleMoves(gamePosition, EnemyColor, false));

                List<Move> Remove = new List<Move>();
                //if so find move that removes all checking moves
                foreach (Move move in PossibleMoves)
                {
                    byte[] AfterMovePossition = new byte[64];
                    Array.Copy(gamePosition, AfterMovePossition, gamePosition.Length);
                    AfterMovePossition[move.TargetPosition.x + move.TargetPosition.y * 8] = (move.EndPiece);
                    AfterMovePossition[move.StartPosition.x + move.StartPosition.y * 8] = PieceData.None;

                    if (PieceData.GetChecks(AfterMovePossition, color, GetAllPossibleMoves(AfterMovePossition, EnemyColor, false)).Any())
                        Remove.Add(move);
                }

                foreach (Move move in Remove)
                {
                    PossibleMoves.Remove(move);
                }
            }

            if (!PossibleMoves.Any())
                Debug.Log("No more moves left: " + ((color.Equals(PieceData.White)) ? ("Black wins!") : ("White wins!")));

            return PossibleMoves;
        }

        public static int NegaMax(int depth, int alpha, int beta, byte[] gamePosition, byte color, out Move bestmove)
        {
            bestmove = null;
            if (depth == 0)
            {
                return EvaluatePosition(gamePosition, color);
            }

            byte EnemyColor = (color.Equals(PieceData.White)) ? (PieceData.Black) : (PieceData.White);
            List<Move> Moves = GetAllPossibleMoves(gamePosition, color, true);

            if (Moves.Count == 0)
            {
                if (PieceData.GetChecks(gamePosition, color, GetAllPossibleMoves(gamePosition, EnemyColor, true)).Any())
                {
                    return NegInfinity;
                }
                return 0;
            }

            int value = NegInfinity;
            Move bestMoveout = null;

            foreach (Move move in Moves)
            {
                byte[] tempGamePosition = new byte[64];
                Array.Copy(gamePosition, tempGamePosition, gamePosition.Length);
                tempGamePosition[move.StartPosition.x + move.StartPosition.y * 8] = PieceData.None;
                tempGamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] = move.EndPiece;
                int eval = -NegaMax(depth - 1, -beta, -alpha, tempGamePosition, EnemyColor, out bestMoveout);

                if (eval >= value)
                {
                    value = eval;
                    bestmove = move;
                }

                alpha = (alpha >= value) ? (alpha) : (value);

                if (alpha > beta)
                    break;
            }
            return value;
        }

        public static Move GetRandMove(byte[] gamePosition, byte color)
        {
            List<Move> Moves = GetAllPossibleMoves(gamePosition, color, true);
            System.Random random = new System.Random();

            return (Moves.Any()) ? (Moves[random.Next(0, Moves.Count)]) : (null);
        }

        public static int EvaluatePosition(byte[] gamePosition, byte color)
        {
            int result = 0;
            foreach (byte square in gamePosition)
            {
                if ((color & square) != 0)
                {
                    result += PieceData.PieceValue[(byte)(square & ~color)];
                }
                else
                {
                    result -= PieceData.PieceValue[(byte)(square & ~((color.Equals(PieceData.White)) ? (PieceData.Black) : (PieceData.White)))];
                }
            }
            //TBD Morecomplex with betterpositioned pieces beeing worth more etc

            return result;
        }
    }

    public class Move
    {
        public Vector2Int StartPosition { get; set; }
        public Vector2Int TargetPosition { get; set; }

        public byte StartPiece { get; set; }

        public byte EndPiece { get; set; }

        public Move(Vector2Int startPosition, Vector2Int targetPosition, byte startPiece, byte endPiece)
        {
            StartPosition = startPosition;
            TargetPosition = targetPosition;
            StartPiece = startPiece;
            EndPiece = endPiece;
        }
    }
}
