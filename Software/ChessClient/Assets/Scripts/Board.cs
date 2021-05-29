using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public Color32 EvenColor = new Color(255, 255, 255, 255);
    public Color32 OddColor = new Color(0, 0, 0, 255);
    public GameObject CellPrefab;

    [HideInInspector]
    public Cell [,] BCells = null;

    public void Setup()
    {
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
