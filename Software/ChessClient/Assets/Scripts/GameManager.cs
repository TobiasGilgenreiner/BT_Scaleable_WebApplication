using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board Board;
    [HideInInspector]
    private byte nexttomove = PieceData.White;
    public byte NextToMove
    {
        get
        {
            return nexttomove;
        }
        set
        {
            if(nexttomove != value)
            {
                nexttomove = value;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Board.Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
