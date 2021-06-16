using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using ChessClassLib;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;

public class Board : MonoBehaviour
{
    public Color32 EvenColor = new Color(255, 255, 255, 255);
    public Color32 OddColor = new Color(0, 0, 0, 255);
    public GameObject CellPrefab;

    [HideInInspector]
    public byte[] GamePosition = new byte[64];
    [HideInInspector]
    public Cell [,] BCells = null;
    [HideInInspector]
    public List<Move> PossibleMoves = new List<Move>();

    private string StartUpFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    private byte[] oldGamePosition = new byte[64];

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
                //Move move;
                //int eval = ChessAI.NegaMax(3, ChessAI.NegInfinity, ChessAI.Infinity, GamePosition, transform.GetComponentInParent<GameManager>().NextToMove, out move);

                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    //Get WorkerNode
                    client.BaseAddress = new Uri("http://localhost:5000");
                    HttpResponseMessage response = client.GetAsync("/ChessAI").Result;

                    //GetNewJobinWorker
                    client.BaseAddress = new Uri(response.Content.ReadAsStringAsync().Result);
                    response = client.GetAsync("?fenPositionString=" + Fen.PositionToFen(GamePosition) + "&color=" + transform.GetComponentInParent<GameManager>().NextToMove + "&oponent=1").Result;
                    int WorkerID = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
                    client.BaseAddress = new Uri(Regex.Match(client.BaseAddress.AbsoluteUri, "https:\\/\\/[^/]+").Value);
                    response = client.GetAsync("/Worker/Result?id=" + WorkerID).Result;
                    while (!response.IsSuccessStatusCode)
                    {
                        response = client.GetAsync("/Worker/Result?id=" + WorkerID).Result;
                    }

                    byte[] newPosition = Fen.LoadPositionFromFen(response.Content.ReadAsStringAsync().Result);
                    Array.Copy(newPosition, GamePosition, newPosition.Length);
                }
                    stopwatch.Stop();
                Debug.Log(stopwatch.ElapsedMilliseconds);
                //if (move != null)
                //{
                //    Debug.Log("Move: (" + move.StartPosition.x + "," + move.StartPosition.x + ") -> (" + move.TargetPosition.x + "," + move.TargetPosition.y + ")\nEvaluation: " + eval + " Time: " + stopwatch.ElapsedMilliseconds + "ms");
                //    byte[] tempGamePosition = new byte[64];
                //    Array.Copy(GamePosition, tempGamePosition, GamePosition.Length);
                //    tempGamePosition[move.StartPosition.x + move.StartPosition.y * 8] = PieceData.None;
                //    tempGamePosition[move.TargetPosition.x + move.TargetPosition.y * 8] = move.EndPiece;
                //    Array.Copy(tempGamePosition, GamePosition, tempGamePosition.Length);
                //}
            }
            transform.GetComponentInParent<GameManager>().NextToMove = (transform.GetComponentInParent<GameManager>().NextToMove.Equals(PieceData.White)) ? (PieceData.Black) : (PieceData.White);
        }
    }
}