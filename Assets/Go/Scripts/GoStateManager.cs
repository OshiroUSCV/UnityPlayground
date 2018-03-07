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
    // The Game Board that is being played upon
    public GoBoard gameBoard;

    private bool mb_gameActive = true;
    private bool mb_blacksTurn = true;    // Black Starts
    private bool mb_passed = false;  // True if the previous player passed his turn. If another pass is received, the game ends
    private float[] m_scoreCaptures = { 0.0f, 0.0f };

    private static float fKomi = 6.5f;

    public GoColor CurrentColor
    {
        get
        {
            return (mb_blacksTurn ? GoColor.GC_Black : GoColor.GC_White);
        }

        // Probably shouldn't have this later
        set
        {
            mb_blacksTurn = (value == GoColor.GC_Black);
        }

    }

	// Use this for initialization
	void Start ()
    {
        // Tell our Board about our existence	
        gameBoard.SetGameManager(this);

        // Initialize score
        ResetScore();
	}


    void PassTurn()
    {
        if (mb_passed)
        {
            EndGame();
        }
        else
        {

            mb_passed = true;
            EndTurn();
        }
    }
    // 
    void EndTurn()
    {
        // Change Turn
        mb_blacksTurn = !mb_blacksTurn;
    }

    void EndGame()
    {
        if (mb_gameActive)
        {
            Debug.Log("Game Over!");
            mb_gameActive = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnBoardPointClicked(Vector2 point)
    {
        int x = (int)point.x;
        int y = (int)point.y;

        // Check if point is empty
        if (!gameBoard.IsPointEmpty(point))
        {
            // If not, we can't play here
            Debug.Log("[GB] Could not create @ ( " + x + ", " + y + "); already occupied");
            return;
        }

        // Check if clicked point is a valid point for current Color to play at
        // If not, do nothing
        if (gameBoard.IsGroupCaptured(point, CurrentColor))
        {
            Debug.Log("[GB] Could not create @ ( " + x + ", " + y + "); illegal move");
        }
        // Otherwise, place the Stone
        else
        {
            gameBoard.CreatePiece(point, mb_blacksTurn);
            OnStonePlayed();
        }
    }

    private void OnStonePlayed()
    {
        // Turn was not passed
        mb_passed = false;

        // Turn taken, so end turn
        EndTurn();
    }

    private void ResetScore()
    {
        m_scoreCaptures[(int)GoColor.GC_Black] = 0;
        m_scoreCaptures[(int)GoColor.GC_White] = fKomi; // Apply Komi value to white score
    }

    public void OnStonesCaptured(GoColor colorAttacker, int countCaptured)
    {
        m_scoreCaptures[(int)colorAttacker] += countCaptured;
    }
}
