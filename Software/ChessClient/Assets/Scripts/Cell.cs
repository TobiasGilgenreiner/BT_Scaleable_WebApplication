using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class Cell : MonoBehaviour
{
    #region PieceSprites
    public Sprite WKingSprite;
    public Sprite BKingSprite;
    public Sprite WQueenSprite;
    public Sprite BQueenSprite;
    public Sprite WRookSprite;
    public Sprite BRookSprite;
    public Sprite WBishopSprite;
    public Sprite BBishopSprite;
    public Sprite WKnightSprite;
    public Sprite BKnightSprite;
    public Sprite WPawnSprite;
    public Sprite BPawnSprite;
    #endregion

    public byte Piece = PieceData.None;
    [HideInInspector]
    public RectTransform rectTransform;
    [HideInInspector]
    public Color BaseColor = new Color(255, 255, 255, 255);
    [HideInInspector]
    public Vector2Int Position;
    [HideInInspector]
    public Sprite Piece_Sprite;

    public void Setup(Board board, Vector3 position, Vector2Int newPosition)
    {
        Position = newPosition;
        //Setup Cell
        gameObject.layer = 2;
        transform.SetParent(board.transform);
        transform.position = position;
        transform.localScale = new Vector3(1, 1);

        //Setup BoxCollider2D
        GetComponent<BoxCollider2D>().size = new Vector2(100, 100);
        GetComponent<BoxCollider2D>().offset = new Vector2(50, 50);

        //Setup RectTransform of Cell
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition3D = new Vector3(position.x * 100, position.y * 100, -1);

        //Setup Cell Sprite
        GetComponent<SpriteRenderer>().size = new Vector2(100, 100);
    }

    public void SetupPieceDisplayGameObject()
    {
        //Setup Gameobject for piecedisplay
        pieceGameObject = new GameObject("PieceObject", typeof(SpriteRenderer), typeof(DragNDrop), typeof(BoxCollider2D));
        pieceGameObject.transform.SetParent(gameObject.transform);
        pieceGameObject.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        pieceGameObject.transform.localPosition = new Vector3(50, 50);
        pieceGameObject.GetComponent<BoxCollider2D>().size = new Vector2(100 / 0.35f, 100 / 0.35f);

        //Setup SpriteRenderer of pieceGameObject to display pieces correctly
        pieceGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        pieceGameObject.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
        pieceGameObject.GetComponent<SpriteRenderer>().size = new Vector2(111, 111);
    }

    public void ClearCell()
    {
        Piece = 0;

        if(pieceGameObject != null)
            DestroyImmediate(pieceGameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private GameObject pieceGameObject;
    private byte oldPiece = 0;

    public void SetOldPieceNone()
    {
        oldPiece = PieceData.None;
    }
    // Update is called once per frame
    void Update()
    {
        if (oldPiece != Piece)
        {
            if (Piece == 0)
            {
                if (pieceGameObject != null)
                    DestroyImmediate(pieceGameObject);
            }
            else if ((Piece & PieceData.Pawn) != 0)
            {
                if(pieceGameObject != null)
                    DestroyImmediate(pieceGameObject);

                SetupPieceDisplayGameObject();
                Piece_Sprite = ((Piece & PieceData.White) != 0) ? (WPawnSprite) : (BPawnSprite);
            }
            else if ((Piece & PieceData.Knight) != 0)
            {
                if (pieceGameObject != null)
                    DestroyImmediate(pieceGameObject);

                SetupPieceDisplayGameObject();
                Piece_Sprite = ((Piece & PieceData.White) != 0) ? (WKnightSprite) : (BKnightSprite);
            }
            else if ((Piece & PieceData.Bishop) != 0)
            {
                if (pieceGameObject != null)
                    DestroyImmediate(pieceGameObject);

                SetupPieceDisplayGameObject();
                Piece_Sprite = ((Piece & PieceData.White) != 0) ? (WBishopSprite) : (BBishopSprite);
            }
            else if ((Piece & PieceData.Queen) != 0)
            {
                if (pieceGameObject != null)
                    DestroyImmediate(pieceGameObject);

                SetupPieceDisplayGameObject();
                Piece_Sprite = ((Piece & PieceData.White) != 0) ? (WQueenSprite) : (BQueenSprite);
            }
            else if ((Piece & PieceData.Rook) != 0)
            {
                if (pieceGameObject != null)
                    DestroyImmediate(pieceGameObject);

                SetupPieceDisplayGameObject();
                Piece_Sprite = ((Piece & PieceData.White) != 0) ? (WRookSprite) : (BRookSprite);
            }
            else if ((Piece & PieceData.King) != 0)
            {
                if (pieceGameObject != null)
                    DestroyImmediate(pieceGameObject);

                SetupPieceDisplayGameObject();
                Piece_Sprite = ((Piece & PieceData.White) != 0) ? (WKingSprite) : (BKingSprite);
            }
            else
            {
                throw new System.Exception("Invalid Piece Value");
            }
            oldPiece = Piece;

            byte[] newGamePosition = new byte[64];
            Array.Copy(transform.parent.GetComponent<Board>().GamePosition, newGamePosition, transform.parent.GetComponent<Board>().GamePosition.Length);
            newGamePosition[Position.x + Position.y * 8] = Piece;
            Array.Copy(newGamePosition,transform.parent.GetComponent<Board>().GamePosition, newGamePosition.Length);

            if (pieceGameObject != null)
            {
                pieceGameObject.GetComponent<SpriteRenderer>().sprite = Piece_Sprite;
            }
        }
        
    }
}
