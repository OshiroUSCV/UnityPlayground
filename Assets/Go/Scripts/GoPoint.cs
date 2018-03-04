using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Relevant data for a single point on a Go Board
public class GoPoint
{
    //////////////////////////////////////////
    // Constants

    // Flags for Point States
    public const int FLAG_EMPTY = 0x01;    // No piece
    public const int FLAG_WHITE = 0x02;    // White piece
    public const int FLAG_BLACK = 0x04;    // Black piece
    public const int FLAG_NONE  = 0x08;    // Edge of board; cannot place points

    // Masks for adjacency data (clockwise?)
    public const int MASK_U = 0xF000;
    public const int MASK_R = 0x0F00;
    public const int MASK_D = 0x00F0;
    public const int MASK_L = 0x000F;

    // Offsets for the Up, Right, Down, and Left point states
    public const int OFFSET_U = 12;
    public const int OFFSET_R = 8;
    public const int OFFSET_D = 4;
    public const int OFFSET_L = 0;

    

    //////////////////////////////////////////
    // Member Variables
    public Vector2 PointBoard = new Vector2(-1, -1);    // (X,Y) point on the Go Board (Columns,Rows)
    public Vector2 PosTexture = new Vector2(-1, -1);  // 2D texture position, used to detect closest clicks
    public Vector3 PosLocal = new Vector3();       // 3D position of the point in the Go Board's local space
    public int AdjSpacesEmptyCount = -1;
    public int AdjSpacesState = 0x0000;

    protected GoStone GoPiece = null;                // GameObject representing the board piece, or null if empty

    //public GoPoint() { }


    //////////////////////////////////////////
    // Functions
    public GoPoint(Vector2 pointBoard, Vector2 posTxt, Vector3 pos3D, int flagsAdjacent)
    {
        PointBoard  = pointBoard;
        PosTexture  = posTxt;
        PosLocal    = pos3D;

        AdjSpacesState = flagsAdjacent;

        // Count empty spaces
        AdjSpacesEmptyCount = 0;
        for (int i = 0; i < 4; i++)
        {
            if (((flagsAdjacent >> (i * 4)) & FLAG_EMPTY) != 0) // Check empty flag for each of the 4 cardinal directions
            {
                AdjSpacesEmptyCount++;
            }
        }
    }

    public bool IsEmpty()
    {
        return (GoPiece == null);
    }

    // @return true if space was previously empty; false otherwise (should always be true in normal circumstances)
    public bool SetPiece(GoStone piece)
    {
        GoStone piece_prev = GoPiece;
        GoPiece = piece;

        // :TODO: Delete piece_prev?

        return (piece_prev == null);
    }

    public bool RemovePiece()
    {
        bool b_removed = false;
        if (!IsEmpty())
        {
            Object.Destroy(GoPiece.gameObject);
            b_removed = true;
        }
        GoPiece = null;

        return b_removed;
    }

    //public GoColor GetColor()
    //{
    //    return GoColor.GC_White; 
    //}
    public void PrintAdjacencyData()
    {
        Debug.Log(string.Format("({0}-{1}): {2:X4}", (char)('A' + PointBoard.x), PointBoard.y, AdjSpacesState));
    }
}

/**
public class GoPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
*/