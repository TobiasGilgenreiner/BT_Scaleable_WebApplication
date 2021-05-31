using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        }
        else
        {
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

            if(!GameObject.ReferenceEquals( targetcell, transform.parent.GetComponent<Cell>()))
            {
                byte tempPiece = transform.parent.GetComponent<Cell>().Piece;
                targetcell.ClearCell();
                targetcell.Piece = tempPiece;
                transform.parent.GetComponent<Cell>().ClearCell();
            }
            else
            {
                byte tempPiece = transform.parent.GetComponent<Cell>().Piece;
                targetcell.ClearCell();
                targetcell.Piece = tempPiece;
            }
        }

        isDragging = (isDragging) ? (false) : (true);
    }

    private void OnMouseUp()
    {

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
