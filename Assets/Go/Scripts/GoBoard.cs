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

    protected Vector2[,] txtGridPositions; 

    // Use this for initialization
    void Start ()
    {
        // Calculate the length of a grid square's size, in pixels :NOTE: assuming our board's grid is square
        txtGridSquareLengthSide = texturePlayfieldDimensions.x / (gridSize - 1);
        // Calculate grid pos radius (squared)
        txtGridPosRadiusSq = (txtGridSquareLengthSide) * 0.25f;
        txtGridPosRadiusSq *= txtGridPosRadiusSq;

        // Populate the grid positions 2D array 
        // :NOTE: Later on this array will probably be more substantial and contain more information regarding the board state
        txtGridPositions = new Vector2[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                txtGridPositions[x, y] = textureCoordsUL + (txtGridSquareLengthSide * new Vector2(x,y));
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        // Trying w/ Raycast
        // Convert screen click space into world position
        //Vector3 pos_mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)); // <<< PROBLEM HERE
       // print("Click Pos (World): " + pos_mouse.ToString("F4"));
        Ray ray_click = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit_info;
        if (Physics.Raycast(ray_click, out hit_info))//Physics.Raycast(pos_mouse, Vector3.down, out hit_info, /*Maximum Distance */1000.0f))
        {
            // Determine closest grid space
            Vector2 pos_grid = GetClosestGridSpace(hit_info.point);

            // Create piece on that position :TODO: Verify if within radius
            CreatePiece(pos_grid, true);
            //CreatePiece(hit_info.point, true);
        }

        ///////////////////////
        /** 
            //TRYING W/ BOUNDS
        // Retrieve plane associated w/ Go Board
        Collider plane = gameObject.GetComponent<Collider>();
        if (plane != null)
        {
            // Convert screen click space into world position
            Vector3 pos_mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

            // Retrieve position on the board
            Vector3 pos_board = plane.ClosestPointOnBounds(pos_mouse);

            Vector2 pos_grid = GetClosestGridSpace(pos_board);
            

        
            


            // DEBUG: Output
            //print("Click Pos (Screen): " + Input.mousePosition.ToString("F4"));
            //print("Click Pos (World): " + pos_mouse.ToString("F4"));
            print("Click Pos (Board) : " +pos_board.ToString("F4"));

            CreatePiece(pos_board, true);
        }
        **/
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

        print("Position Txt: " + pos_txt.ToString("F4"));

        // Next is to identify which grid square we are in, to identify our closest point
        Vector2 grid_square = (pos_txt - textureCoordsUL) / txtGridSquareLengthSide;
        print("Position Grid: " + grid_square.ToString("F4"));

        // Round & Clamp results
        int grid_x = (int)(Mathf.Clamp(Mathf.Round(grid_square.x), 0, gridSize - 1));
        int grid_y = (int)(Mathf.Clamp(Mathf.Round(grid_square.y), 0, gridSize - 1));


        //textureCoordsUL
        return new Vector2(grid_x, grid_y);
    }

    private void CreatePiece(Vector2 posGrid, bool bIsBlack)
    {
        // Convert grid position to board's local position
        float x = transform.localScale.x * (((posGrid.x * txtGridSquareLengthSide) + textureCoordsUL.x) / textureDimensions.x);
        x -= (0.5f * transform.localScale.x);
        float z = transform.localScale.z * (((posGrid.y * txtGridSquareLengthSide) + textureCoordsUL.y) / textureDimensions.y);
        z -= (0.5f * transform.localScale.z);
        Vector3 pos_local = new Vector3(x, transform.localPosition.y, z); // Offset Y by half the piece's height, aka scale-Y
        // Spawn
        CreatePiece(pos_local, bIsBlack);
    }

    private void CreatePiece(Vector3 posSpawn, bool bIsBlack)
    {
        GameObject piece_new = Instantiate((bIsBlack ? prefabPieceBlack : prefabPieceWhite), posSpawn/*new Vector3(-0.5f, 0.0f, -1.0f)*/, Quaternion.identity, transform);

    }
}
