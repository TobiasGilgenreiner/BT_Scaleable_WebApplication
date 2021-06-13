using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessClassLib
{
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
                for (int y = 7; y >= 0; --y)
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

        public static bool ISStringFen(string FenString)
        {
            Regex FenRegex = new Regex("\\s*([rnbqkpRNBQKP1-8]+\\/){7}([rnbqkpRNBQKP1-8]+)");

            return FenRegex.Match(FenString).Success;
        }
    }

}
