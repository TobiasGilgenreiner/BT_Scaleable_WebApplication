using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Board : MonoBehaviour
{
    private string StartUpFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    public Color32 EvenColor = new Color(255, 255, 255, 255);
    public Color32 OddColor = new Color(0, 0, 0, 255);
    public GameObject CellPrefab;

    [HideInInspector]
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
                newCell.GetComponent<Cell>().Setup(this, new Vector3(x, y, -1));
                BCells[x,y] = newCell.GetComponent<Cell>();
            }
        }

        //Color
        for (int y = 0; y < 8; ++y)
        {
            for (int x = 0; x < 8; x++)
            {
                BCells[x, y].GetComponent<SpriteRenderer>().color = ((x + y) % 2 == 0) ? (EvenColor) : (OddColor);
            }
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        GamePosition = Fen.LoadPositionFromFen(StartUpFen);
        Debug.Log(Fen.PositionToFen(GamePosition));
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
            oldGamePosition = GamePosition;
        }
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
}