using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class Board : MonoBehaviour
{
    private string StartUpFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    public Color32 EvenColor = new Color(255, 255, 255, 255);
    public Color32 OddColor = new Color(0, 0, 0, 255);
    public GameObject CellPrefab;

    
    public byte[] GamePosition = new byte[64];

    [HideInInspector]
    public Cell [,] BCells = null;

    public void Setup()
    {
        gameObject.layer = 1;

        BCells = new Cell[8, 8];
        for(int y = 0; y < 8; ++y)
        {
            for(int x = 0; x < 8; x++)
            {
                GameObject newCell = Instantiate(CellPrefab);
                newCell.GetComponent<Cell>().Setup(this, new Vector3(x, y, -1), new Vector2Int(x,y));
                BCells[x,y] = newCell.GetComponent<Cell>();
            }
        }

        //Color
        for (int y = 0; y < 8; ++y)
        {
            for (int x = 0; x < 8; x++)
            {
                BCells[x, y].GetComponent<SpriteRenderer>().color = ((x + y) % 2 == 0) ? (EvenColor) : (OddColor);
                BCells[x, y].BaseColor = ((x + y) % 2 == 0) ? (EvenColor) : (OddColor);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GamePosition = Fen.LoadPositionFromFen(StartUpFen);
        Debug.Log(Fen.PositionToFen(GamePosition));
        //TBD make dependent on gamemanager
    }

    private byte[] oldGamePosition = new byte[64];
    // Update is called once per frame
    void Update()
    {
        if (!Enumerable.SequenceEqual(oldGamePosition, GamePosition))
        {
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; x++)
                {
                    BCells[x, y].Piece = GamePosition[y * 8 + x];
                }
            }
            Array.Copy(GamePosition, oldGamePosition, GamePosition.Length);

            if (transform.GetComponentInParent<GameManager>().NextToMove.Equals(PieceData.White))
            {
                PossibleMoves = ChessAI.GetAllPossibleMoves(GamePosition, transform.GetComponentInParent<GameManager>().NextToMove, true);
            }
            else
            {
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                Move move;
                int eval = ChessAI.NegaMax(3, ChessAI.NegInfinity, ChessAI.Infinity, GamePosition, transform.GetComponentInParent<GameManager>().NextToMove, out move);
                stopwatch.Stop();

                if (move != null)
                {
                    Debug.Log("Move: (" + move.StartPosition.x + "," + move.StartPosition.x + ") -> (" + move.TargetPosition.x + "," + move.TargetPosition.y + ")\nEvaluation: " + eval + " Time: " + stopwatch.ElapsedMilliseconds + "ms");
                    byte[] tempGamePosition = new byte[64];
                    Array.Copy(GamePosition, tempGamePosition, GamePosition.Length);
                    tempGamePosition[move.StartPosition.x + move.StartPosition.y * 8] = PieceData.None;
                    tempGamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] = move.EndPiece;
                    Array.Copy(tempGamePosition, GamePosition, tempGamePosition.Length);
                }
            }
            transform.GetComponentInParent<GameManager>().NextToMove = (transform.GetComponentInParent<GameManager>().NextToMove.Equals(PieceData.White)) ? (PieceData.Black) : (PieceData.White);
        }
    }

    [HideInInspector]
    public List<Move> PossibleMoves = new List<Move>();

    public int GetNextMove(byte[] gamePosition, byte color)
    {
        throw new System.NotImplementedException("GetNextMove");
    }
}

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
    public static Vector2Int? GetKingPosition(byte[] gamePosition, byte color)
    {
        for (int y = 0; y < 8; ++y)
        {
            for (int x = 0; x < 8; ++x)
            {
                if ((gamePosition[x + 8 * y] & (PieceData.King)) != 0 && (gamePosition[x + 8 * y] & (color)) != 0)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }

    public static List<Move> GetChecks(byte[] gamePosition, byte kingColor, List<Move> possibleEnemyMoves)
    {
        Vector2Int? tempKing = PieceData.GetKingPosition(gamePosition, kingColor);
        Vector2Int KingPosition;

        if (tempKing != null)
            KingPosition = (Vector2Int)tempKing;
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

            if((gamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] & color) != 0)
            {
                BlockedMoves.Add(move);
            }
        }

        return BlockedMoves;
    }

    /// <summary>
    /// Returns moves intercepted by own or enemy pieces
    /// </summary>
    private static List<Move> GetInterceptedMoves(byte[] gamePosition, List<Move> possibleMoves, Vector2Int directionVector)
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

        if(directionVector.y >= 0)
        {
            possibleMoves = tempCollection.ThenBy(x => x.TargetPosition.y).ToList();
        }
        else
        {
            possibleMoves = tempCollection.ThenByDescending(x => x.TargetPosition.x).ToList();
        }

        Vector2Int startPosition = possibleMoves.First().StartPosition;
        byte color = ((gamePosition[startPosition.x + startPosition.y * 8] & PieceData.White) != 0) ? (PieceData.White) : (PieceData.Black);

        List<Move> InvalidMoves = new List<Move>();
        List<Move> ToBeTestedMoves = new List<Move>();

        List<Move> DirectionMoves = new List<Move>();

        int x = startPosition.x;
        int y = startPosition.y;
        while(x < 8 && x >= 0 && y < 8 && y >= 0)
        {
            DirectionMoves.Add(new Move(startPosition, new Vector2Int(x, y), PieceData.None, PieceData.None));
            x += directionVector.x;
            y += directionVector.y;
        }

        ToBeTestedMoves = possibleMoves.Where(x => DirectionMoves.Any(y => y.TargetPosition.x.Equals(x.TargetPosition.x) && y.TargetPosition.y.Equals(x.TargetPosition.y))).ToList();

        bool blocked = false;
        foreach(Move move in ToBeTestedMoves)
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
                else if((gamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] & color) != 0)
                {
                    InvalidMoves.Add(move);
                    blocked = true;
                }
                else if(!((gamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] & color) != 0))
                {
                    blocked = true;
                }
            }
        }

        return InvalidMoves;
    }

    public static List<Move> GetPawnMoves(byte[] gamePosition, Vector2Int startPosition)
    {
        List<Move> PossibleMoves = new List<Move>();

        if((gamePosition[startPosition.x + startPosition.y * 8] & PieceData.White) != 0)
        {
            if(startPosition.y < 6)
            {
                //move one forward
                if (gamePosition[startPosition.x + (startPosition.y + 1) * 8].Equals(PieceData.None))
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y + 1), PieceData.Pawn, PieceData.Pawn));
                }

                //capture to the right
                if (startPosition.x + 1 <= 7 && (gamePosition[startPosition.x + 1 + (startPosition.y + 1) * 8] & PieceData.Black) != 0)
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.Pawn, PieceData.Pawn));
                }

                //capture to the left
                if (startPosition.x - 1 >= 0 && (gamePosition[startPosition.x - 1 + (startPosition.y + 1) * 8] & PieceData.Black) != 0)
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.Pawn, PieceData.Pawn));
                }

                //move two forward
                if (startPosition.y == 1 && gamePosition[startPosition.x + (startPosition.y + 2) * 8].Equals(PieceData.None) && gamePosition[startPosition.x + (startPosition.y + 1) * 8].Equals(PieceData.None))
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y + 2), PieceData.Pawn, PieceData.Pawn));
                }
            }
            else if(startPosition.y == 6)
            {
                //move one forward
                if (gamePosition[startPosition.x + (startPosition.y + 1) * 8].Equals(PieceData.None))
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y + 1), PieceData.Pawn, PieceData.Knight));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y + 1), PieceData.Pawn, PieceData.Bishop));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y + 1), PieceData.Pawn, PieceData.Rook));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y + 1), PieceData.Pawn, PieceData.Queen));
                }

                //capture to the right
                if (startPosition.x + 1 <= 7 && (gamePosition[startPosition.x + 1 + (startPosition.y + 1) * 8] & PieceData.Black) != 0)
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.Pawn, PieceData.Knight));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.Pawn, PieceData.Bishop));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.Pawn, PieceData.Rook));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.Pawn, PieceData.Queen));
                }

                //capture to the left
                if (startPosition.x - 1 >= 0 && (gamePosition[startPosition.x - 1 + (startPosition.y + 1) * 8] & PieceData.Black) != 0)
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.Pawn, PieceData.Knight));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.Pawn, PieceData.Bishop));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.Pawn, PieceData.Rook));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.Pawn, PieceData.Queen));
                }
            }
        }
        else if ((gamePosition[startPosition.x + startPosition.y * 8] & PieceData.Black) != 0)
        {
            if(startPosition.y > 1)
            {
                //move one forward
                if (gamePosition[startPosition.x + (startPosition.y - 1) * 8].Equals(PieceData.None))
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y - 1), PieceData.Pawn, PieceData.Pawn));
                }

                //capture to the rigth
                if (startPosition.x + 1 <= 7 && (gamePosition[startPosition.x + 1 + (startPosition.y - 1) * 8] & PieceData.White) != 0)
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.Pawn, PieceData.Pawn));
                }

                //capture to the left
                if (startPosition.x - 1 >= 0 && (gamePosition[startPosition.x - 1 + (startPosition.y - 1) * 8] & PieceData.White) != 0)
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.Pawn, PieceData.Pawn));
                }

                //move two forward
                if (startPosition.y == 6 && gamePosition[startPosition.x + (startPosition.y - 2) * 8].Equals(PieceData.None) && gamePosition[startPosition.x + (startPosition.y - 1) * 8].Equals(PieceData.None))
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y - 2), PieceData.Pawn, PieceData.Pawn));
                }
            }
            else if(startPosition.y == 1)
            {
                //move one forward
                if (gamePosition[startPosition.x + (startPosition.y - 1) * 8].Equals(PieceData.None))
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y - 1), PieceData.Pawn, PieceData.Knight));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y - 1), PieceData.Pawn, PieceData.Bishop));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y - 1), PieceData.Pawn, PieceData.Rook));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y - 1), PieceData.Pawn, PieceData.Queen));
                }

                //capture to the rigth
                if (startPosition.x + 1 <= 7 && (gamePosition[startPosition.x + 1 + (startPosition.y - 1) * 8] & PieceData.White) != 0)
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.Pawn, PieceData.Knight));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.Pawn, PieceData.Bishop));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.Pawn, PieceData.Rook));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.Pawn, PieceData.Queen));
                }

                //capture to the left
                if (startPosition.x - 1 >= 0 && (gamePosition[startPosition.x - 1 + (startPosition.y - 1) * 8] & PieceData.White) != 0)
                {
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.Pawn, PieceData.Knight));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.Pawn, PieceData.Bishop));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.Pawn, PieceData.Rook));
                    PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.Pawn, PieceData.Queen));
                }
            }
        }

        return PossibleMoves;
    }
    public static List<Move> GetKnightMoves(byte[] gamePosition, Vector2Int startPosition)
    {
        List<Move> PossibleMoves = new List<Move>();

        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 2, startPosition.y + 1), PieceData.Knight, PieceData.Knight));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 2, startPosition.y - 1), PieceData.Knight, PieceData.Knight));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 2, startPosition.y + 1), PieceData.Knight, PieceData.Knight));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 2, startPosition.y - 1), PieceData.Knight, PieceData.Knight));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y + 2), PieceData.Knight, PieceData.Knight));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y + 2), PieceData.Knight, PieceData.Knight));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y - 2), PieceData.Knight, PieceData.Knight));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y - 2), PieceData.Knight, PieceData.Knight));

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
    public static List<Move> GetBishopMoves(byte[] gamePosition, Vector2Int startPosition)
    {
        byte color = ((gamePosition[startPosition.x + startPosition.y * 8] & PieceData.White) != 0) ? (PieceData.White) : (PieceData.Black);
        List<Move> PossibleMoves = new List<Move>();

        for (int i = 1; i < 8; ++i)
        {
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + i, startPosition.y + i), PieceData.Bishop, PieceData.Bishop));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + i, startPosition.y - i), PieceData.Bishop, PieceData.Bishop));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - i, startPosition.y + i), PieceData.Bishop, PieceData.Bishop));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - i, startPosition.y - i), PieceData.Bishop, PieceData.Bishop));
        }

        List<Move> ToBeRemovedMoves = GetOutOfBoundsMoves(gamePosition, PossibleMoves);

        foreach (Move move in ToBeRemovedMoves)
        {
            PossibleMoves.Remove(move);
        }

        ToBeRemovedMoves = GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(1, 1));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(1, -1)));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(-1, 1)));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(-1, -1)));

        foreach (Move move in ToBeRemovedMoves)
        {
            PossibleMoves.Remove(move);
        }

        return PossibleMoves;
    }
    public static List<Move> GetRookMoves(byte[] gamePosition, Vector2Int startPosition)
    {
        List<Move> PossibleMoves = new List<Move>();

        for(int i = 1; i < 8; ++i)
        {
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + i, startPosition.y), PieceData.Rook, PieceData.Rook));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - i, startPosition.y), PieceData.Rook, PieceData.Rook));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y + i), PieceData.Rook, PieceData.Rook));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y - i), PieceData.Rook, PieceData.Rook));
        }

        List<Move> ToBeRemovedMoves = GetOutOfBoundsMoves(gamePosition, PossibleMoves);

        foreach (Move move in ToBeRemovedMoves)
        {
            PossibleMoves.Remove(move);
        }

        ToBeRemovedMoves = GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(1, 0));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(-1, 0)));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(0, 1)));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(0, -1)));

        foreach (Move move in ToBeRemovedMoves)
        {
            PossibleMoves.Remove(move);
        }

        return PossibleMoves;
    }
    public static List<Move> GetQueenMoves(byte[] gamePosition, Vector2Int startPosition)
    {
        List<Move> PossibleMoves = new List<Move>();

        for (int i = 1; i < 8; ++i)
        {
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + i, startPosition.y + i), PieceData.Queen, PieceData.Queen));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + i, startPosition.y - i), PieceData.Queen, PieceData.Queen));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - i, startPosition.y + i), PieceData.Queen, PieceData.Queen));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - i, startPosition.y - i), PieceData.Queen, PieceData.Queen));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + i, startPosition.y), PieceData.Queen, PieceData.Queen));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - i, startPosition.y), PieceData.Queen, PieceData.Queen));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y + i), PieceData.Queen, PieceData.Queen));
            PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y - i), PieceData.Queen, PieceData.Queen));
        }

        List<Move> ToBeRemovedMoves = GetOutOfBoundsMoves(gamePosition, PossibleMoves);

        foreach (Move move in ToBeRemovedMoves)
        {
            PossibleMoves.Remove(move);
        }

        ToBeRemovedMoves = GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(1, 0));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(-1, 0)));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(0, 1)));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(0, -1)));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(1, 1)));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(1, -1)));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(-1, 1)));
        ToBeRemovedMoves.AddRange(GetInterceptedMoves(gamePosition, PossibleMoves, new Vector2Int(-1, -1)));

        foreach (Move move in ToBeRemovedMoves)
        {
            PossibleMoves.Remove(move);
        }

        return PossibleMoves;
    }
    public static List<Move> GetKingMoves(byte[] gamePosition, Vector2Int startPosition)
    {
        List<Move> PossibleMoves = new List<Move>();

        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y + 1), PieceData.King, PieceData.King));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y - 1), PieceData.King, PieceData.King));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y + 1), PieceData.King, PieceData.King));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y - 1), PieceData.King, PieceData.King));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x + 1, startPosition.y), PieceData.King, PieceData.King));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x - 1, startPosition.y), PieceData.King, PieceData.King));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y + 1), PieceData.King, PieceData.King));
        PossibleMoves.Add(new Move(startPosition, new Vector2Int(startPosition.x, startPosition.y - 1), PieceData.King, PieceData.King));

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

public static class Fen
{
    public static Dictionary<char, byte> FenToPieceDictionary = new Dictionary<char, byte>()
    {
        ['k'] = PieceData.King + PieceData.Black,
        ['K'] = PieceData.King + PieceData.White,
        ['p'] = PieceData.Pawn + PieceData.Black,
        ['P'] = PieceData.Pawn + PieceData.White,
        ['n'] = PieceData.Knight + PieceData.Black,
        ['N'] = PieceData.Knight + PieceData.White,
        ['b'] = PieceData.Bishop + PieceData.Black,
        ['B'] = PieceData.Bishop + PieceData.White,
        ['r'] = PieceData.Rook + PieceData.Black,
        ['R'] = PieceData.Rook + PieceData.White,
        ['q'] = PieceData.Queen + PieceData.Black,
        ['Q'] = PieceData.Queen + PieceData.White,
    };

    public static Dictionary<byte, char> PieceToFenDictionary = new Dictionary<byte, char>()
    {
        [PieceData.King + PieceData.Black] = 'k',
        [PieceData.King + PieceData.White] = 'K',
        [PieceData.Pawn + PieceData.Black] = 'p',
        [PieceData.Pawn + PieceData.White] = 'P',
        [PieceData.Knight + PieceData.Black] = 'n',
        [PieceData.Knight + PieceData.White] = 'N',
        [PieceData.Bishop + PieceData.Black] = 'b',
        [PieceData.Bishop + PieceData.White] = 'B',
        [PieceData.Rook + PieceData.Black] = 'r',
        [PieceData.Rook + PieceData.White] = 'R',
        [PieceData.Queen + PieceData.Black] = 'q',
        [PieceData.Queen + PieceData.White] = 'Q',
    };

    public static byte[] LoadPositionFromFen(string Fen)
    {
        byte[] result = new byte[64];

        int file = 0, rank = 7;
        foreach (char symbol in Fen)
        {
            if (symbol == '/')
            {
                file = 0;
                --rank;
            }
            else if (char.IsDigit(symbol))
            {
                file += (int)char.GetNumericValue(symbol);
            }
            else
            {
                result[rank * 8 + file] = FenToPieceDictionary[symbol];
                ++file;
            }
        }

        return result;
    }

    public static string PositionToFen(byte[] PositionArray)
    {
        if (PositionArray.Length != 64)
            throw new System.Exception("Invalid PositionArray size");
        string result = "";
        try
        {
            for(int y = 7; y >= 0; --y)
            {
                int emptycells = 0;
                for (int x = 0; x < 8; ++x)
                {
                    if (PositionArray[y * 8 + x].Equals(PieceData.None))
                    {
                        ++emptycells;
                    }
                    else
                    {
                        if (emptycells != 0)
                        {
                            result += emptycells.ToString();
                            emptycells = 0;
                        }

                        result += Fen.PieceToFenDictionary[PositionArray[y * 8 + x]].ToString();
                    }

                }
                if (emptycells != 0)
                {
                    result += emptycells.ToString();
                }
                result += '/';
            }

            return result;
        }
        catch
        {
            throw new System.Exception("Invalid PositionArray");
        }
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