using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClassLib
{
    public static class PieceData
    {
        public const byte None = 0;
        public const byte King = 1;
        public const byte Pawn = 2;
        public const byte Knight = 4;
        public const byte Bishop = 8;
        public const byte Rook = 16;
        public const byte Queen = 32;

        public const byte White = 64;
        public const byte Black = 128;

        public static Dictionary<byte, int> PieceValue = new Dictionary<byte, int>()
        {
            [PieceData.King] = 10000,
            [PieceData.Queen] = 900,
            [PieceData.Rook] = 500,
            [PieceData.Bishop] = 300,
            [PieceData.Knight] = 300,
            [PieceData.Pawn] = 100,
            [PieceData.None] = 0,
        };
        
        public static byte[] MakeMove(byte[] gamePosition, Move move)
        {
            byte[] resultGamePosition = new byte[64];
            Array.Copy(gamePosition, resultGamePosition, gamePosition.Length);
            resultGamePosition[move.StartPosition.x + move.StartPosition.y * 8] = PieceData.None;
            resultGamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] = move.EndPiece;

            return resultGamePosition;
        }

        public static byte[] UnMakeMove(byte[] gamePosition, Move move)
        {
            byte[] resultGamePosition = new byte[64];
            Array.Copy(gamePosition, resultGamePosition, gamePosition.Length);
            resultGamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] = PieceData.None;
            resultGamePosition[move.StartPosition.x + move.StartPosition.y * 8] = move.StartPiece;

            return resultGamePosition;
        }

        public static HelperVector2Int GetKingPosition(byte[] gamePosition, byte color)
        {
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    if ((gamePosition[x + 8 * y] & (PieceData.King)) != 0 && (gamePosition[x + 8 * y] & (color)) != 0)
                    {
                        return new HelperVector2Int(x, y);
                    }
                }
            }

            return null;
        }

        public static List<Move> GetChecks(byte[] gamePosition, byte kingColor, List<Move> possibleEnemyMoves)
        {
            HelperVector2Int tempKing = PieceData.GetKingPosition(gamePosition, kingColor);
            HelperVector2Int KingPosition;

            if (tempKing != null)
                KingPosition = tempKing;
            else
                return new List<Move>();

            return possibleEnemyMoves.Where(x => x.TargetPosition.x.Equals(KingPosition.x) && x.TargetPosition.y.Equals(KingPosition.y)).ToList();
        }

        /// <summary>
        /// Returns Moves that are not inside of the chessboard coordiantes (0-7)
        /// </summary>
        private static List<Move> GetOutOfBoundsMoves(byte[] gamePosition, List<Move> possibleMoves)
        {
            List<Move> OutOfBoundsMoves = new List<Move>();

            foreach (Move move in possibleMoves)
            {
                if (move.TargetPosition.x > 7 || move.TargetPosition.x < 0 || move.TargetPosition.y > 7 || move.TargetPosition.y < 0)
                {
                    OutOfBoundsMoves.Add(move);
                    continue;
                }
            }

            return OutOfBoundsMoves;
        }

        /// <summary>
        /// Moves blocked by own pieces
        /// </summary>
        private static List<Move> GetBlockedMoves(byte[] gamePosition, List<Move> possibleMoves)
        {
            List<Move> BlockedMoves = new List<Move>();

            foreach (Move move in possibleMoves)
            {
                byte color = ((gamePosition[move.StartPosition.x + move.StartPosition.y * 8] & PieceData.White) != 0) ? (PieceData.White) : (PieceData.Black);

                if ((gamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] & color) != 0)
                {
                    BlockedMoves.Add(move);
                }
            }

            return BlockedMoves;
        }

        /// <summary>
        /// Returns moves intercepted by own or enemy pieces
        /// </summary>
        private static List<Move> GetInterceptedMoves(byte[] gamePosition, List<Move> possibleMoves, HelperVector2Int directionVector)
        {
            IOrderedEnumerable<Move> tempCollection;
            if (directionVector.x >= 0)
            {
                tempCollection = possibleMoves.OrderBy(x => x.TargetPosition.x);
            }
            else
            {
                tempCollection = possibleMoves.OrderByDescending(x => x.TargetPosition.x);
            }

            if (directionVector.y >= 0)
            {
                possibleMoves = tempCollection.ThenBy(x => x.TargetPosition.y).ToList();
            }
            else
            {
                possibleMoves = tempCollection.ThenByDescending(x => x.TargetPosition.x).ToList();
            }

            HelperVector2Int startPosition = possibleMoves.First().StartPosition;
            byte color = ((gamePosition[startPosition.x + startPosition.y * 8] & PieceData.White) != 0) ? (PieceData.White) : (PieceData.Black);

            List<Move> InvalidMoves = new List<Move>();
            List<Move> ToBeTestedMoves = new List<Move>();

            List<Move> DirectionMoves = new List<Move>();

            int x = startPosition.x;
            int y = startPosition.y;
            while (x < 8 && x >= 0 && y < 8 && y >= 0)
            {
                DirectionMoves.Add(new Move(startPosition, new HelperVector2Int(x, y), PieceData.None, PieceData.None));
                x += directionVector.x;
                y += directionVector.y;
            }

            ToBeTestedMoves = possibleMoves.Where(x => DirectionMoves.Any(y => y.TargetPosition.x.Equals(x.TargetPosition.x) && y.TargetPosition.y.Equals(x.TargetPosition.y))).ToList();

            bool blocked = false;
            foreach (Move move in ToBeTestedMoves)
            {
                if (blocked)
                {
                    InvalidMoves.Add(move);
                }
                else
                {
                    if (gamePosition[move.TargetPosition.x + move.TargetPosition.y * 8].Equals(PieceData.None))
                    {
                        continue;
                    }
                    else if ((gamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] & color) != 0)
                    {
                        InvalidMoves.Add(move);
                        blocked = true;
                    }
                    else if (!((gamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] & color) != 0))
                    {
                        blocked = true;
                    }
                }
            }

            return InvalidMoves;
        }

        public static List<Move> GetPawnMoves(byte[] gamePosition, HelperVector2Int startPosition)
        {
            List<Move> PossibleMoves = new List<Move>();

            if ((gamePosition[startPosition.x + startPosition.y * 8] & PieceData.White) != 0)
            {
                if (startPosition.y < 6)
                {
                    //move one forward
                    if (gamePosition[startPosition.x + (startPosition.y + 1) * 8].Equals(PieceData.None))
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y + 1), PieceData.Pawn, PieceData.Pawn));
                    }

                    //capture to the right
                    if (startPosition.x + 1 <= 7 && (gamePosition[startPosition.x + 1 + (startPosition.y + 1) * 8] & PieceData.Black) != 0)
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.Pawn, PieceData.Pawn));
                    }

                    //capture to the left
                    if (startPosition.x - 1 >= 0 && (gamePosition[startPosition.x - 1 + (startPosition.y + 1) * 8] & PieceData.Black) != 0)
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.Pawn, PieceData.Pawn));
                    }

                    //move two forward
                    if (startPosition.y == 1 && gamePosition[startPosition.x + (startPosition.y + 2) * 8].Equals(PieceData.None) && gamePosition[startPosition.x + (startPosition.y + 1) * 8].Equals(PieceData.None))
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y + 2), PieceData.Pawn, PieceData.Pawn));
                    }
                }
                else if (startPosition.y == 6)
                {
                    //move one forward
                    if (gamePosition[startPosition.x + (startPosition.y + 1) * 8].Equals(PieceData.None))
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y + 1), PieceData.Pawn, PieceData.Knight));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y + 1), PieceData.Pawn, PieceData.Bishop));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y + 1), PieceData.Pawn, PieceData.Rook));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y + 1), PieceData.Pawn, PieceData.Queen));
                    }

                    //capture to the right
                    if (startPosition.x + 1 <= 7 && (gamePosition[startPosition.x + 1 + (startPosition.y + 1) * 8] & PieceData.Black) != 0)
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.Pawn, PieceData.Knight));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.Pawn, PieceData.Bishop));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.Pawn, PieceData.Rook));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.Pawn, PieceData.Queen));
                    }

                    //capture to the left
                    if (startPosition.x - 1 >= 0 && (gamePosition[startPosition.x - 1 + (startPosition.y + 1) * 8] & PieceData.Black) != 0)
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.Pawn, PieceData.Knight));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.Pawn, PieceData.Bishop));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.Pawn, PieceData.Rook));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.Pawn, PieceData.Queen));
                    }
                }
            }
            else if ((gamePosition[startPosition.x + startPosition.y * 8] & PieceData.Black) != 0)
            {
                if (startPosition.y > 1)
                {
                    //move one forward
                    if (gamePosition[startPosition.x + (startPosition.y - 1) * 8].Equals(PieceData.None))
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y - 1), PieceData.Pawn, PieceData.Pawn));
                    }

                    //capture to the rigth
                    if (startPosition.x + 1 <= 7 && (gamePosition[startPosition.x + 1 + (startPosition.y - 1) * 8] & PieceData.White) != 0)
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.Pawn, PieceData.Pawn));
                    }

                    //capture to the left
                    if (startPosition.x - 1 >= 0 && (gamePosition[startPosition.x - 1 + (startPosition.y - 1) * 8] & PieceData.White) != 0)
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.Pawn, PieceData.Pawn));
                    }

                    //move two forward
                    if (startPosition.y == 6 && gamePosition[startPosition.x + (startPosition.y - 2) * 8].Equals(PieceData.None) && gamePosition[startPosition.x + (startPosition.y - 1) * 8].Equals(PieceData.None))
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y - 2), PieceData.Pawn, PieceData.Pawn));
                    }
                }
                else if (startPosition.y == 1)
                {
                    //move one forward
                    if (gamePosition[startPosition.x + (startPosition.y - 1) * 8].Equals(PieceData.None))
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y - 1), PieceData.Pawn, PieceData.Knight));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y - 1), PieceData.Pawn, PieceData.Bishop));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y - 1), PieceData.Pawn, PieceData.Rook));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y - 1), PieceData.Pawn, PieceData.Queen));
                    }

                    //capture to the rigth
                    if (startPosition.x + 1 <= 7 && (gamePosition[startPosition.x + 1 + (startPosition.y - 1) * 8] & PieceData.White) != 0)
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.Pawn, PieceData.Knight));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.Pawn, PieceData.Bishop));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.Pawn, PieceData.Rook));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.Pawn, PieceData.Queen));
                    }

                    //capture to the left
                    if (startPosition.x - 1 >= 0 && (gamePosition[startPosition.x - 1 + (startPosition.y - 1) * 8] & PieceData.White) != 0)
                    {
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.Pawn, PieceData.Knight));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.Pawn, PieceData.Bishop));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.Pawn, PieceData.Rook));
                        PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.Pawn, PieceData.Queen));
                    }
                }
            }

            return PossibleMoves;
        }
        public static List<Move> GetKnightMoves(byte[] gamePosition, HelperVector2Int startPosition)
        {
            List<Move> PossibleMoves = new List<Move>();

            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 2, startPosition.y + 1), PieceData.Knight, PieceData.Knight));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 2, startPosition.y - 1), PieceData.Knight, PieceData.Knight));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 2, startPosition.y + 1), PieceData.Knight, PieceData.Knight));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 2, startPosition.y - 1), PieceData.Knight, PieceData.Knight));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y + 2), PieceData.Knight, PieceData.Knight));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y + 2), PieceData.Knight, PieceData.Knight));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y - 2), PieceData.Knight, PieceData.Knight));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y - 2), PieceData.Knight, PieceData.Knight));

            List<Move> ToBeRemovedMoves = GetOutOfBoundsMoves(gamePosition, PossibleMoves);

            foreach (Move move in ToBeRemovedMoves)
            {
                PossibleMoves.Remove(move);
            }

            ToBeRemovedMoves = GetBlockedMoves(gamePosition, PossibleMoves);

            foreach (Move move in ToBeRemovedMoves)
            {
                PossibleMoves.Remove(move);
            }

            return PossibleMoves;
        }
        public static List<Move> GetBishopMoves(byte[] gamePosition, HelperVector2Int startPosition)
        {
            byte color = ((gamePosition[startPosition.x + startPosition.y * 8] & PieceData.White) != 0) ? (PieceData.White) : (PieceData.Black);
            List<Move> PossibleMoves = new List<Move>();

            for (int i = 1; i < 8; ++i)
            {
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + i, startPosition.y + i), PieceData.Bishop, PieceData.Bishop));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + i, startPosition.y - i), PieceData.Bishop, PieceData.Bishop));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - i, startPosition.y + i), PieceData.Bishop, PieceData.Bishop));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - i, startPosition.y - i), PieceData.Bishop, PieceData.Bishop));
            }

            List<Move> ToBeRemovedMoves = GetOutOfBoundsMoves(gamePosition, PossibleMoves);

            foreach (Move move in ToBeRemovedMoves)
            {
                PossibleMoves.Remove(move);
            }

            ToBeRemovedMoves = GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(1, 1));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(1, -1)));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(-1, 1)));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(-1, -1)));

            foreach (Move move in ToBeRemovedMoves)
            {
                PossibleMoves.Remove(move);
            }

            return PossibleMoves;
        }
        public static List<Move> GetRookMoves(byte[] gamePosition, HelperVector2Int startPosition)
        {
            List<Move> PossibleMoves = new List<Move>();

            for (int i = 1; i < 8; ++i)
            {
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + i, startPosition.y), PieceData.Rook, PieceData.Rook));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - i, startPosition.y), PieceData.Rook, PieceData.Rook));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y + i), PieceData.Rook, PieceData.Rook));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y - i), PieceData.Rook, PieceData.Rook));
            }

            List<Move> ToBeRemovedMoves = GetOutOfBoundsMoves(gamePosition, PossibleMoves);

            foreach (Move move in ToBeRemovedMoves)
            {
                PossibleMoves.Remove(move);
            }

            ToBeRemovedMoves = GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(1, 0));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(-1, 0)));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(0, 1)));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(0, -1)));

            foreach (Move move in ToBeRemovedMoves)
            {
                PossibleMoves.Remove(move);
            }

            return PossibleMoves;
        }
        public static List<Move> GetQueenMoves(byte[] gamePosition, HelperVector2Int startPosition)
        {
            List<Move> PossibleMoves = new List<Move>();

            for (int i = 1; i < 8; ++i)
            {
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + i, startPosition.y + i), PieceData.Queen, PieceData.Queen));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + i, startPosition.y - i), PieceData.Queen, PieceData.Queen));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - i, startPosition.y + i), PieceData.Queen, PieceData.Queen));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - i, startPosition.y - i), PieceData.Queen, PieceData.Queen));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + i, startPosition.y), PieceData.Queen, PieceData.Queen));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - i, startPosition.y), PieceData.Queen, PieceData.Queen));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y + i), PieceData.Queen, PieceData.Queen));
                PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y - i), PieceData.Queen, PieceData.Queen));
            }

            List<Move> ToBeRemovedMoves = GetOutOfBoundsMoves(gamePosition, PossibleMoves);

            foreach (Move move in ToBeRemovedMoves)
            {
                PossibleMoves.Remove(move);
            }

            ToBeRemovedMoves = GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(1, 0));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(-1, 0)));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(0, 1)));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(0, -1)));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(1, 1)));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(1, -1)));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(-1, 1)));
            ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new HelperVector2Int(-1, -1)));

            foreach (Move move in ToBeRemovedMoves)
            {
                PossibleMoves.Remove(move);
            }

            return PossibleMoves;
        }
        public static List<Move> GetKingMoves(byte[] gamePosition, HelperVector2Int startPosition)
        {
            List<Move> PossibleMoves = new List<Move>();

            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.King, PieceData.King));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.King, PieceData.King));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.King, PieceData.King));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.King, PieceData.King));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x + 1, startPosition.y), PieceData.King, PieceData.King));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x - 1, startPosition.y), PieceData.King, PieceData.King));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y + 1), PieceData.King, PieceData.King));
            PossibleMoves.Add(new Move(startPosition, new HelperVector2Int(startPosition.x, startPosition.y - 1), PieceData.King, PieceData.King));

            List<Move> ToBeRemovedMoves = GetOutOfBoundsMoves(gamePosition, PossibleMoves);

            foreach (Move move in ToBeRemovedMoves)
            {
                PossibleMoves.Remove(move);
            }

            ToBeRemovedMoves = GetBlockedMoves(gamePosition, PossibleMoves);

            foreach (Move move in ToBeRemovedMoves)
            {
                PossibleMoves.Remove(move);
            }

            return PossibleMoves;
        }
    }

    public class HelperVector2Int
    {
        public int x { get; set; }
        public int y { get; set; }

        public HelperVector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }


}
