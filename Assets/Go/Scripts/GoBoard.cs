using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoBoard : MonoBehaviour
{
    // Variables: Pieces
    public GameObject prefabPieceBlack;
    public GameObject prefabPieceWhite;

    // Variables: Board
    public int gridSize;                        // Size of play field grid (always a square)
    public Vector2 textureDimensions;
    public Vector2 textureCoordsUL;             // Upper-left coordinates of texture (but BOTTOM-left when applied to quad)
    public Vector2 texturePlayfieldDimensions;  // Distance in pixels from UL corner of the board to the BR corner

    protected float txtGridSquareLengthSide;  // Length of each side of a grid-square (in pixels)
    protected float txtGridPosRadiusSq;       // Radius for click detection of each grid position (squared)

    protected GoPoint[,] gridPoints; 

    // Use this for initialization
    void Start ()
    {
        // Calculate the length of a grid square's size, in pixels :NOTE: assuming our board's grid is square
        txtGridSquareLengthSide = texturePlayfieldDimensions.x / (gridSize - 1);
        // Calculate grid pos radius (squared)
        txtGridPosRadiusSq = (txtGridSquareLengthSide) * 0.25f;
        txtGridPosRadiusSq *= txtGridPosRadiusSq;

        // Populate the grid points 2D array 
        gridPoints = new GoPoint[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2 point_board = new Vector2(x, y);
                Vector2 pos_txt     = textureCoordsUL + (txtGridSquareLengthSide * new Vector2(x, y));

                // Convert grid position to board's local position
                float pos_x = transform.localScale.x * (((point_board.x * txtGridSquareLengthSide) + textureCoordsUL.x) / textureDimensions.x);
                pos_x -= (0.5f * transform.localScale.x);
                float pos_z = transform.localScale.z * (((point_board.y * txtGridSquareLengthSide) + textureCoordsUL.y) / textureDimensions.y);
                pos_z -= (0.5f * transform.localScale.z);
                Vector3 pos_3D      = new Vector3(pos_x, 0.0f, pos_z);
                gridPoints[x, y]    = new GoPoint(point_board, pos_txt, pos_3D);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseOver()
    {
        // Click (Any)
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {

        }
        // Left: Black Piece
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseClick(true); 
        }
        // Right: White Piece
        else
        if (Input.GetMouseButtonDown(1))
        {
            OnMouseClick(false);
        }
        // Middle: Remove
        else
        if (Input.GetMouseButtonDown(2))
        {
            RemovePiece(GetClosestGridPoint());
        }
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseClick(bool bLeft)
    {
        // For clarity/readability
        bool b_black = bLeft;

        // Trying w/ Raycast
        // Cast ray from current mouse position to board
        Ray ray_click = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit_info;
        if (Physics.Raycast(ray_click, out hit_info))//Physics.Raycast(pos_mouse, Vector3.down, out hit_info, /*Maximum Distance */1000.0f))
        {
            // Determine closest grid space
            Vector2 pos_grid = GetClosestGridSpace(hit_info.point);

            // Create piece on that position :TODO: Verify if within radius
            CreatePiece(pos_grid, b_black);
        }
    }


    // @return  Closest point on board if mouse currently hovering over board; (-1,-1) otherwise
    private Vector2 GetClosestGridPoint()
    {
        Vector2 point_closest = new Vector2(-1, -1); 

        // Cast ray from current mouse position to board
        Ray ray_click = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit_info;
        if (Physics.Raycast(ray_click, out hit_info))//Physics.Raycast(pos_mouse, Vector3.down, out hit_info, /*Maximum Distance */1000.0f))
        {
            // Determine closest grid space
            point_closest = GetClosestGridSpace(hit_info.point);
        }

        return point_closest;
    }

    private Vector2 GetClosestGridSpace(Vector3 posWorld)
    {
        // Convert world position to local position
        Vector3 pos_local = posWorld - transform.position;

        // Next, determine our world scale
        // :NOTE: Currently, we are using a 1x1 scaled quad, so we'll simply use the transform's scale as dimensions.
        Vector3 dimensions_local = transform.localScale;

        // Finally, translate local pos so bottom-left is (0,0)
        pos_local = pos_local + (0.5f * dimensions_local);

        // Now we can map our local position from (0,0) to (10,10) onto the texture coordinates
        Vector2 pos_txt = new Vector2(pos_local.x, pos_local.z);
        // Convert local position to unit coordinates...
        pos_txt.x /= dimensions_local.x;
        pos_txt.y /= dimensions_local.z; // :NOTE: We mapped the 3D X & Z coords to our D X & Y coords
        // ... and then multiply by texture dimensions
        pos_txt = new Vector2(pos_txt.x * textureDimensions.x, pos_txt.y * textureDimensions.y);

        //print("Position Txt: " + pos_txt.ToString("F4"));

        // Next is to identify which grid square we are in, to identify our closest point
        Vector2 grid_square = (pos_txt - textureCoordsUL) / txtGridSquareLengthSide;
        //print("Position Grid: " + grid_square.ToString("F4"));

        // Round & Clamp results
        int grid_x = (int)(Mathf.Clamp(Mathf.Round(grid_square.x), 0, gridSize - 1));
        int grid_y = (int)(Mathf.Clamp(Mathf.Round(grid_square.y), 0, gridSize - 1));


        //textureCoordsUL
        return new Vector2(grid_x, grid_y);
    }

    private GameObject CreatePiece(Vector2 posGrid, bool bIsBlack)
    {
        //// Convert grid position to board's local position
        //float x = transform.localScale.x * (((posGrid.x * txtGridSquareLengthSide) + textureCoordsUL.x) / textureDimensions.x);
        //x -= (0.5f * transform.localScale.x);
        //float z = transform.localScale.z * (((posGrid.y * txtGridSquareLengthSide) + textureCoordsUL.y) / textureDimensions.y);
        //z -= (0.5f * transform.localScale.z);
        //Vector3 pos_local = new Vector3(x, transform.localPosition.y, z);

        int point_x = (int)posGrid.x;
        int point_y = (int)posGrid.y;

        // Check if a piece already exists on given spot. If so, we cannot create a new one
        if (!gridPoints[point_x, point_y].IsEmpty())
        {
            Debug.Log("[GB] Could not create @ ( " + point_x + ", " + point_y + "); already occupied");
            return null;
        }

        // Spawn
        // :TODO:Offset Y by half the piece's height, aka scale-Y
        Debug.Log("[GB] Piece created @ ( " + point_x + ", " + point_y + ")");
        GameObject piece_new = CreatePiece(gridPoints[point_x, point_y].PosLocal, bIsBlack);
        gridPoints[point_x, point_y].SetPiece(piece_new, bIsBlack);
        return piece_new;
    }

    private GameObject CreatePiece(Vector3 posSpawn, bool bIsBlack)
    {
        GameObject piece_new = Instantiate((bIsBlack ? prefabPieceBlack : prefabPieceWhite), posSpawn/*new Vector3(-0.5f, 0.0f, -1.0f)*/, Quaternion.identity, transform);
        return piece_new;
    }

    private bool RemovePiece(Vector3 posGrid)
    {
        int x = (int)posGrid.x;
        int y = (int)posGrid.y;
        return gridPoints[x, y].RemovePiece();
    }

    private bool RemovePiece(Vector2 posGrid)
    {
        int x = (int)posGrid.x;
        int y = (int)posGrid.y;
        return gridPoints[x, y].RemovePiece();
    }
}
