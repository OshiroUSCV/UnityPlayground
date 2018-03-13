using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GoColor
{
    GC_Black = 0,
    GC_White = 1,
}

public class GoStateManager : MonoBehaviour
{
    // The Game Board that is being played upon
    public GoBoard gameBoard;

    // UI
    public Text textScore;
    public Text textTurn;
    public Button btnPass;

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
    void Start()
    {
        // Tell our Board about our existence	
        gameBoard.SetGameManager(this);

        // Initialize score
        ResetScore();
    }


    public void PassTurn()
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

        // Update HUD
        UpdateHud();
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
    void Update() {

    }

    public void OnBoardPointClicked(Vector2 point)
    {
        // Do nothing if game has ended
        if (!mb_gameActive)
        {
            return;
        }

        // If given Point is not a valid play, do nothing
        if (!gameBoard.IsValidPlay(point, CurrentColor ))
        {
            return;

        }

        // Otherwise, place the Stone
        gameBoard.CreatePiece(point, mb_blacksTurn);
        OnStonePlayed();
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
        UpdateHud();
    }

    public void OnStonesCaptured(GoColor colorAttacker, int countCaptured)
    {
        m_scoreCaptures[(int)colorAttacker] += countCaptured;
        UpdateHud();
    }

    protected void UpdateHud()
    {
        textScore.text = string.Format("<color=purple>SCORE:</color> <color=black>{0}</color> - <color=white>{1}</color>", m_scoreCaptures[0], m_scoreCaptures[1]);
        textTurn.text = string.Format("<color=purple>ACTIVE PLAYER</color>\n<color={0}>{0}</color>", mb_blacksTurn ? "BLACK" : "WHITE");
    }
}
