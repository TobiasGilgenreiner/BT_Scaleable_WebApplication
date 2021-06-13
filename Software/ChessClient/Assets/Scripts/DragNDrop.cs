using ChessClassLib;
using System.Collections;   
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragNDrop : MonoBehaviour
{
    private RectTransform rectTransform;
    private bool isDragging;
    private Vector3 startingPosition;

    private void Awake()
    {
        gameObject.layer = 3;
        rectTransform = GetComponent<RectTransform>();
    }

    private List<Cell> TargetCells = null;
    private List<Move> possibleMoves = null;
    private void OnMouseDown()
    {
        if (!isDragging)
        {
            startingPosition = transform.position;
            Color color = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0.6f);
            GetComponent<SpriteRenderer>().sortingOrder = 2;

            Board board = transform.GetComponentInParent<Board>();
            foreach(Cell cell in board.BCells)
            {
                if(cell.Piece != 0)
                {
                    cell.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            transform.GetComponent<BoxCollider2D>().enabled = true;

            possibleMoves = board.PossibleMoves.Where(x => x.StartPosition.Equals(transform.parent.GetComponent<Cell>().Position)).ToList();
            TargetCells = new List<Cell>();
            foreach(Move move in possibleMoves)
            {
                TargetCells.Add(board.BCells[move.TargetPosition.x, move.TargetPosition.y]);
            }

            TargetCells = TargetCells.Distinct().ToList();

            foreach(Cell cell in TargetCells)
            {
                cell.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
            }
        }
        else
        {
            foreach (Cell cell in TargetCells)
            {
                cell.GetComponent<SpriteRenderer>().color = cell.BaseColor;
            }

            Color color = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1);
            GetComponent<SpriteRenderer>().sortingOrder = 1;

            Vector3 mousPosition = Input.mousePosition;
            Board board = transform.GetComponentInParent<Board>();
            Cell targetcell = null;
            foreach(Transform child in board.transform)
            {
                if ((child.localPosition.x <= mousPosition.x - 560) && (child.localPosition.x + 100 >= mousPosition.x - 560) && (child.localPosition.y <= mousPosition.y - 140) && (child.localPosition.y + 100 >= mousPosition.y - 140))
                {
                    targetcell = child.GetComponent<Cell>();
                    break;
                }
            }

            foreach (Cell cell in board.BCells)
            {
                if (cell.Piece != 0 && !cell.gameObject.Equals(transform.parent.GetComponent<Cell>()))
                {
                    cell.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
                }
            }

            if (TargetCells.Contains(targetcell))
            {
                targetcell.ClearCell();
                List<Move> resultingMoves = possibleMoves.Where(x => x.TargetPosition.x.Equals(targetcell.Position.x) && x.TargetPosition.y.Equals(targetcell.Position.y)).ToList();
                if (resultingMoves.Count == 1)
                {
                    targetcell.Piece = resultingMoves.First().EndPiece;
                }
                else
                {
                    //TBD Actual Selection
                    targetcell.Piece = resultingMoves.Last().EndPiece;
                }

                transform.parent.GetComponent<Cell>().ClearCell();
            }
            else
            {
                byte tempPiece = transform.parent.GetComponent<Cell>().Piece;
                Cell parentcell = transform.parent.GetComponent<Cell>();
                parentcell.SetOldPieceNone();
                parentcell.ClearCell();
                parentcell.Piece = tempPiece;
            }
        }

        isDragging = (isDragging) ? (false) : (true);
    }

    private void Update()
    {
        if(isDragging)
        {
            Vector2 mousPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousPosition);
        }
    }
}
