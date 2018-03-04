﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoBoard : MonoBehaviour
{
    // Variables: Pieces
    public GoStone prefabPieceBlack;
    public GoStone prefabPieceWhite;

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
                // Calculate vectors
                Vector2 point_board = new Vector2(x, y);
                Vector2 pos_txt     = textureCoordsUL + (txtGridSquareLengthSide * new Vector2(x, y));

                // Convert grid position to board's local position
                float pos_x = transform.localScale.x * (((point_board.x * txtGridSquareLengthSide) + textureCoordsUL.x) / textureDimensions.x);
                pos_x -= (0.5f * transform.localScale.x);
                float pos_z = transform.localScale.z * (((point_board.y * txtGridSquareLengthSide) + textureCoordsUL.y) / textureDimensions.y);
                pos_z -= (0.5f * transform.localScale.z);
                Vector3 pos_3D      = new Vector3(pos_x, 0.0f, pos_z);

                // Set up adjacency flags for current point (Up/Right/Down/Left)
                int flags_adjacent = 0x0000;

                flags_adjacent |= ((y == (gridSize - 1) ? GoPoint.FLAG_NONE : GoPoint.FLAG_EMPTY) << GoPoint.OFFSET_U);
                flags_adjacent |= ((y == 0 ? GoPoint.FLAG_NONE : GoPoint.FLAG_EMPTY) << GoPoint.OFFSET_D);
                flags_adjacent |= ((x == 0 ? GoPoint.FLAG_NONE : GoPoint.FLAG_EMPTY) << GoPoint.OFFSET_L);
                flags_adjacent |= ((x == (gridSize - 1) ? GoPoint.FLAG_NONE : GoPoint.FLAG_EMPTY) << GoPoint.OFFSET_R);

                // Create Point
                gridPoints[x, y]    = new GoPoint(point_board, pos_txt, pos_3D, flags_adjacent);
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

    // Place piece on the board and handle board state management
    private void PlacePiece(GoStone piece, Vector2 pointGrid)
    {
        int point_x = (int)pointGrid.x;
        int point_y = (int)pointGrid.y;
        
        // Place GoPiece GameObject in the GoPoint
        gridPoints[point_x, point_y].SetPiece(piece);

        // Decrement neighboring point's empty count
        UpdateAdjacentEmptySpaces(pointGrid, (piece.Color == GoColor.GC_Black));
    }

    // Creates piece and places it onto the board
    private GoStone CreatePiece(Vector2 posGrid, bool bIsBlack)
    {
        //// Convert grid position to board's local position
        //float x = transform.localScale.x * (((posGrid.x * txtGridSquareLengthSide) + textureCoordsUL.x) / textureDimensions.x);
        //x -= (0.5f * transform.localScale.x);
        //float z = transform.localScale.z * (((posGrid.y * txtGridSquareLengthSide) + textureCoordsUL.y) / textureDimensions.y);
        //z -= (0.5f * transform.localScale.z);
        //Vector3 pos_local = new Vector3(x, transform.localPosition.y, z);

        int point_x = (int)posGrid.x;
        int point_y = (int)posGrid.y;

        // :DEBUG:
        PrintAdjacencyData(point_x, point_y);

        // Check if a piece already exists on given spot. If so, we cannot create a new one
        if (!gridPoints[point_x, point_y].IsEmpty())
        {
            Debug.Log("[GB] Could not create @ ( " + point_x + ", " + point_y + "); already occupied");
            return null;
        }

        // Spawn
        // :TODO:Offset Y by half the piece's height, aka scale-Y
        Debug.Log("[GB] Piece created @ ( " + point_x + ", " + point_y + ")");

        // Create the Unity GameObject
        GoStone piece_new = CreatePiece(gridPoints[point_x, point_y].PosLocal, bIsBlack);
        // Place newly-created piece on the board
        PlacePiece(piece_new, new Vector2(point_x, point_y));
        
        return piece_new;
    }

    

    // Creates GameObject  in the 3D world
    private GoStone CreatePiece(Vector3 posSpawn, bool bIsBlack)
    {
        GoStone piece_new = Instantiate((bIsBlack ? prefabPieceBlack : prefabPieceWhite), posSpawn/*new Vector3(-0.5f, 0.0f, -1.0f)*/, Quaternion.identity, transform);
        return piece_new;
    }

    //private bool RemovePiece(Vector3 posGrid)
    //{
    //    int x = (int)posGrid.x;
    //    int y = (int)posGrid.y;
    //    return gridPoints[x, y].RemovePiece();
    //}

    private bool RemovePiece(Vector2 pointGrid)
    {
        int x = (int)pointGrid.x;
        int y = (int)pointGrid.y;

        bool b_removed = gridPoints[x, y].RemovePiece();
        if (b_removed)
        {
            UpdateAdjacentEmptySpaces(pointGrid);
        }
        return b_removed;
    }

    // Remove: NEW
    private void UpdateAdjacentEmptySpaces(Vector2 pointCenter)
    {
        UpdateAdjacencyState(pointCenter + new Vector2(-1, 0), GoPoint.FLAG_EMPTY, GoPoint.OFFSET_R);
        UpdateAdjacencyState(pointCenter + new Vector2( 1, 0), GoPoint.FLAG_EMPTY, GoPoint.OFFSET_L);
        UpdateAdjacencyState(pointCenter + new Vector2(0, -1), GoPoint.FLAG_EMPTY, GoPoint.OFFSET_U);
        UpdateAdjacencyState(pointCenter + new Vector2(0,  1), GoPoint.FLAG_EMPTY, GoPoint.OFFSET_D);
    }

    // Add: NEW
    private void UpdateAdjacentEmptySpaces(Vector2 pointCenter, bool bIsBlack)
    {
        UpdateAdjacencyState(pointCenter + new Vector2(-1, 0), (bIsBlack ? GoPoint.FLAG_BLACK : GoPoint.FLAG_WHITE), GoPoint.OFFSET_R);
        UpdateAdjacencyState(pointCenter + new Vector2( 1, 0), (bIsBlack ? GoPoint.FLAG_BLACK : GoPoint.FLAG_WHITE), GoPoint.OFFSET_L);
        UpdateAdjacencyState(pointCenter + new Vector2(0, -1), (bIsBlack ? GoPoint.FLAG_BLACK : GoPoint.FLAG_WHITE), GoPoint.OFFSET_U);
        UpdateAdjacencyState(pointCenter + new Vector2(0,  1), (bIsBlack ? GoPoint.FLAG_BLACK : GoPoint.FLAG_WHITE), GoPoint.OFFSET_D);
    }

    private void UpdateAdjacencyState(Vector2 point, int state, int directionOffset)
    {
        // Return -1 if invalid point is requested
        if (!IsValidPoint(point))
        {
            return;
        }

        int x = (int)point.x;
        int y = (int)point.y;

        // Clear previous state
        int state_new = gridPoints[x, y].AdjSpacesState & ~(0x000F << directionOffset); // :NOTE: Offset before invert or you'll shift in zeroes!
        // Apply new state
        state_new |= (state << directionOffset);
        // Store
        gridPoints[x, y].AdjSpacesState = state_new;
    }

    //// Add/Remove: OLD
    //private void OLDUpdateAdjacentEmptySpaces(Vector2 pointCenter, int offset)
    //{
    //    OLDUpdateNumEmptySpaces(pointCenter + new Vector2(-1, 0), offset);
    //    OLDUpdateNumEmptySpaces(pointCenter + new Vector2(1, 0), offset);
    //    OLDUpdateNumEmptySpaces(pointCenter + new Vector2(0, -1), offset);
    //    OLDUpdateNumEmptySpaces(pointCenter + new Vector2(0, 1), offset);
    //}

    //// OLD
    //private int OLDUpdateNumEmptySpaces(Vector2 point, int offset)
    //{
    //    // Return -1 if invalid point is requested
    //    if (!IsValidPoint(point))
    //    {
    //        return -1;
    //    }

    //    int x = (int)point.x;
    //    int y = (int)point.y;

    //    // Calc & store new empty space count
    //    int count_updated = gridPoints[x, y].AdjSpacesEmptyCount + offset;
    //    gridPoints[x, y].AdjSpacesEmptyCount = count_updated;

    //    return count_updated;
    //}

    // Check if a given point is a valid board position
    private bool IsValidPoint(Vector2 point)
    {
        int x = (int)point.x;
        int y = (int)point.y;

        return (x >= 0 && x < gridSize && y >= 0 && y < gridSize);
    }

    private void PrintAdjacencyData(int x, int y)
    {
        gridPoints[x, y].PrintAdjacencyData();
    }
}
