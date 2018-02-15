using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoColor
{
    GC_Black = 0,
    GC_White = 1,
}

public class GoStateManager : MonoBehaviour
{
    private bool mb_blacksTurn      = true;    // Black Starts
    private bool mb_passed          = false;  // True if the previous player passed his turn. If another pass is received, the game ends
    private int[] m_scoreCaptures   = { 0, 0 };

	// Use this for initialization
	void Start () {
		
	}


    void PassTurn()
    {
        if (mb_passed)
        {
            EndGame();
        }

        mb_passed = true;
        EndTurn();
    }
    // 
    void EndTurn()
    {
        // Change Turn
        mb_blacksTurn = !mb_blacksTurn;
    }

    void EndGame()
    {


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
