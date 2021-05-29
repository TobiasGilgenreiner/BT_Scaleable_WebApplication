using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board Board;
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
