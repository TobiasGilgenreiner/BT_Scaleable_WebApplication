using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [HideInInspector]
    public RectTransform rectTransform;
    [HideInInspector]
    public Vector2Int Position;
    [HideInInspector]
    public Image Piece;

    public void Setup(Board board, Vector3 position)
    {
        rectTransform = GetComponent<RectTransform>();
        transform.SetParent(board.transform);
        transform.position = position;
        transform.localScale = new Vector3(1, 1);
        GetComponent<SpriteRenderer>().size = new Vector2(100, 100);
        rectTransform.anchoredPosition3D = new Vector3(position.x * 100, position.y * 100, -1);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
