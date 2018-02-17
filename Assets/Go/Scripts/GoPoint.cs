using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Relevant data for a single point on a Go Board
public class GoPoint
{
    public Vector2 PointBoard = new Vector2(-1, -1);    // (X,Y) point on the Go Board
    public Vector2 PosTexture = new Vector2(-1, -1);  // 2D texture position, used to detect closest clicks
    public Vector3 PosLocal = new Vector3();       // 3D position of the point in the Go Board's local space

    protected GameObject GoPiece = null;                // GameObject representing the board piece, or null if empty

    //public GoPoint() { }

    public GoPoint(Vector2 pointBoard, Vector2 posTxt, Vector3 pos3D)
    {
        PointBoard  = pointBoard;
        PosTexture  = posTxt;
        PosLocal    = pos3D;
    }

    public bool IsEmpty()
    {
        return (GoPiece == null);
    }

    public void SetPiece(GameObject piece, bool bIsBlack)
    {
        GoPiece = piece;
    }

    public bool RemovePiece()
    {
        bool b_removed = false;
        if (!IsEmpty())
        {
            Object.Destroy(GoPiece);
            b_removed = true;
        }
        GoPiece = null;

        return b_removed;
    }

    //public GoColor GetColor()
    //{
    //    return GoColor.GC_White; 
    //}

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