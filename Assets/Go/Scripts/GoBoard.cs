using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GoBoard : MonoBehaviour
{
    protected GoStateManager gameStateManager;

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

    // Called during StateManager's Start()
    public void SetGameManager(GoStateManager manager)
    {
        gameStateManager = manager;
    }
	
	void Update () {
		
	}

    private void OnMouseOver()
    {
        // Input Handling
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseClick();
        }

        //// Left: Black Piece
        //if (Input.GetMouseButtonDown(0))
        //{
        //    OnMouseClickDEBUG(true); 
        //}
        //// Right: White Piece
        //else
        //if (Input.GetMouseButtonDown(1))
        //{
        //    OnMouseClickDEBUG(false);
        //}
        //// Middle: Remove
        //else
        //if (Input.GetMouseButtonDown(2))
        //{
        //    // RemovePiece(GetClosestGridPoint());
        //    Vector2 point_clicked   = GetClosestGridPoint();
        //    GoPoint point_curr      = gridPoints[(int)point_clicked.x, (int)point_clicked.y];
        //    if (!point_curr.IsEmpty())
        //    {
        //        int count_captured = TryCaptureGroup(point_clicked, point_curr.GetStone().Color);
        //        Debug.Log("Stones Captured: " + count_captured);
        //    }
        //}
        // DEBUG: Stone Placing (Q = Black, W = White)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnMouseClickDEBUG(true);
        }
        else
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnMouseClickDEBUG(false);
        }

        // DEBUG: Check Point info
        if (Input.GetKeyDown(KeyCode.C))
        {
            Vector2 point_curr = GetClosestGridPoint();
            //PrintAdjacencyData((int)point_curr.x, (int)point_curr.y);
            Debug.Log(string.Format("Surrounded by Black: {0}", IsGroupCaptured(point_curr, GoColor.GC_White)));
            Debug.Log(string.Format("Surrounded by White: {0}", IsGroupCaptured(point_curr, GoColor.GC_Black)));
            GroupInfo g = GroupInfo.CreateGroup(this, point_curr);
            Debug.Log(g);
        }
    }

    private void OnMouseClick()
    {
        // Cast ray from current mouse position to board
        Ray ray_click = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit_info;
        if (Physics.Raycast(ray_click, out hit_info))//Physics.Raycast(pos_mouse, Vector3.down, out hit_info, /*Maximum Distance */1000.0f))
        {
            // Determine closest grid space
            Vector2 pos_grid = GetClosestGridSpace(hit_info.point);

            // :TODO: Check if close enough to that point to count as a click

            // Report the clicked point
            gameStateManager.OnBoardPointClicked(pos_grid);
        }
    }

    private void OnMouseClickDEBUG(bool bLeft)
    {
        // For clarity/readability
        bool b_black = bLeft;

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


    // Check if a given point is a valid board position
    public bool IsValidPoint(int x, int y)
    {
        return (x >= 0 && x < gridSize && y >= 0 && y < gridSize);
    }

    public bool IsValidPoint(Vector2 point)
    {
        int x = (int)point.x;
        int y = (int)point.y;

        return IsValidPoint(x, y);
    }

    // @return  True if point is valid AND empty
    public bool IsPointEmpty(Vector2 point)
    {
        return (IsValidPoint(point) && gridPoints[(int)point.x, (int)point.y].IsEmpty());
    }

    public GoPoint GetPoint(Vector2 point)
    {
        if (!IsValidPoint(point))
        {
            return null;
        }

        return gridPoints[(int)point.x, (int)point.y];
    }

    // @return  True if the specified color can play in the given Point
    public bool IsValidPlay(Vector2 point, GoColor colorStone)
    {
        int x = (int)point.x;
        int y = (int)point.y;

        // Check: If Point is empty. If not, we cannot play there
        if (!IsPointEmpty(point))
        {
            Debug.Log("[GB] Could not create @ ( " + x + ", " + y + "); already occupied");
            return false;
        }

        // Check: If new Stone would be immediately be captured
        if (IsGroupCaptured(point, colorStone))
        {
            // If a Stone would immediately be captured, we can play it
            // IFF that move would capture enemy Stone(s)
            // AND we aren't violating the rule of Ko
            List<Vector2> list_blocked = new List<Vector2>();
            list_blocked.Add(point);

            // Check adjacent spots to see if we can capture enemy Stone(s)
            List<Vector2> list_adjacent = GetAdjacentPoints(point);
            bool b_capture_detected = false;
            foreach(Vector2 point_adj in list_adjacent)
            {
                GoPoint gp_adj = gridPoints[(int)point_adj.x, (int)point_adj.y];
                // We only care about checking against enemy stones

                GoStone stone_adj = gp_adj.GetStone();
                if (stone_adj.Color != colorStone)
                {
                    b_capture_detected |= IsGroupCaptured(point_adj, stone_adj.Color, list_blocked);
                }
            }

            // If no captured were found, play is illegal
            if (!b_capture_detected)
            {
                Debug.Log("[GB] Could not create @ ( " + x + ", " + y + "); illegal move (surrounded)");
                return false;
            }
            // Check for Ko
            else
            {
                GoTurn turn_prev = gameStateManager.GetTurnPrev();
                if (turn_prev.piecesCaptured == 1 && IsGroupCaptured(turn_prev.pointPlay, turn_prev.turnColor, list_blocked))
                {
                    Debug.Log("[GB] Could not create @ ( " + x + ", " + y + "); illegal move (Ko)");
                    return false;
                }
            }
        }

        return true;
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
        GoPoint point_center = gridPoints[point_x, point_y];
        GoColor color_attacker = piece.Color;
        GoColor color_enemy = (color_attacker == GoColor.GC_Black ? GoColor.GC_White : GoColor.GC_Black);

        // Place GoPiece GameObject in the GoPoint
        point_center.SetPiece(piece);

        //// Decrement neighboring point's empty count
        //UpdateAdjacentEmptySpaces(pointGrid, (color_attacker == GoColor.GC_Black));

        // Check if we have captured any enemy pieces (Up/Down/Left/Right
        Queue<Vector2> queue_adjacents = new Queue<Vector2>();
        List<Vector2> list_adj = GetAdjacentPoints(pointGrid);
        foreach (Vector2 point_adj in list_adj)
        {
            queue_adjacents.Enqueue(point_adj);
        }

        int count_captured = 0;
        while (queue_adjacents.Count > 0)
        {
            // Retrieve Vector2
            Vector2 vec_curr = queue_adjacents.Dequeue();

            // Check if valid
            if (IsValidPoint(vec_curr))
            {
                // Retrieve GoPoint
                GoPoint point_curr = gridPoints[(int)vec_curr.x, (int)vec_curr.y];
                // Check if valid, if enemy color
                if (!point_curr.IsEmpty() && point_curr.GetStone().Color == color_enemy)
                {
                    // If so, check if surrounded. 
                    if (IsGroupCaptured(vec_curr, color_enemy))
                    {
                        // If so, remove group and report score to GoStateManager
                        count_captured += TryCaptureGroup(vec_curr, color_enemy);
                    }
                }
            }
        }
        
        // Report captured stones
        if (count_captured > 0)
        {
            gameStateManager.OnStonesCaptured(color_attacker, count_captured);
        }
    }

    // Creates GameObject  in the 3D world
    private GoStone CreatePiece(Vector3 posSpawn, bool bIsBlack)
    {
        GoStone piece_new = Instantiate((bIsBlack ? prefabPieceBlack : prefabPieceWhite), posSpawn/*new Vector3(-0.5f, 0.0f, -1.0f)*/, Quaternion.identity, transform);
        return piece_new;
    }

    // Creates piece and places it onto the board
    public GoStone CreatePiece(Vector2 posGrid, bool bIsBlack)
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

        // Create the Unity GameObject
        GoStone piece_new = CreatePiece(gridPoints[point_x, point_y].PosLocal, bIsBlack);
        // Place newly-created piece on the board
        PlacePiece(piece_new, new Vector2(point_x, point_y));
        
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
        //if (b_removed)
        //{
        //    UpdateAdjacentEmptySpaces(pointGrid);
        //}
        return b_removed;
    }

    // @param   point       Point to check if surrounded
    // @param   groupColor  Color of the group. Will check if surrounded by opposite color
    public bool IsGroupCaptured(Vector2 point, GoColor groupColor)
    {
        List<Vector2> list_empty = new List<Vector2>();
        return IsGroupCaptured(point, groupColor, list_empty);
    }

    // @param   listExcludes   List of board Points that will not be checkable. Used to simulate empty spaces actually having enemy Stones within
    public bool IsGroupCaptured(Vector2 point, GoColor groupColor, List<Vector2> listExcludes )
    {
        // Check: Bounds
        if (!IsValidPoint(point))
        {
            return false;
        }

        int x_start = (int)point.x;
        int y_start = (int)point.y;

        // Create hash to store all points that have been checked
        HashSet<GoPoint> hash_group = new HashSet<GoPoint>();
        // Create queue to store list of points that need to be checked
        Queue<GoPoint> queue_points = new Queue<GoPoint>();

        // Insert excluded points into our HashSet
        foreach (Vector2 point_exclude in listExcludes)
        {
            if (IsValidPoint(point_exclude))
            {
                hash_group.Add(gridPoints[(int)point_exclude.x, (int)point_exclude.y]);
            }
        }

        int x_curr = x_start;
        int y_curr = y_start;
        GoPoint point_curr = gridPoints[x_curr, y_curr];

        bool b_first = true;    // We don't want to check the Point/Stone at the initial position; so we can check even if no Stone is placed yet
        while (b_first || queue_points.Count > 0)
        {

            bool b_allied_piece = (b_first || false);

            // Check current point's Stone (or lack thereof)
            if (!b_first)
            {
                // Retrieve next point
                point_curr = queue_points.Dequeue();
                x_curr = (int)point_curr.PointBoard.x;
                y_curr = (int)point_curr.PointBoard.y;

                // Only proceed if we haven't seen this point yet
                if (!hash_group.Contains(point_curr))
                {
                    // If we don't find a stone, then there is at least one open space and the group is not captured
                    if (point_curr.IsEmpty())
                    {
                        return false;
                    }
                    // Next, check if we have found an enemy Stone. If so, we don't want to check adjacent spaces
                    if (point_curr.GetStone().Color == groupColor)
                    {
                        b_allied_piece = true;
                    }
                }
            }

            // We only want to skip the checks for our initial point
            b_first = false;

            // Retrieve adjacent points if allied
            if (b_allied_piece)
            {
                List<Vector2> list_adj = GetAdjacentPoints(point_curr.PointBoard);

                foreach (Vector2 point_adj in list_adj)
                {
                    queue_points.Enqueue(gridPoints[(int)point_adj.x, (int)point_adj.y]);
                }
            }

            // Mark GoPoint as checked
            hash_group.Add(point_curr);
        }

        // If we run out of points to check without having found an empty spot, this group is captured
        return true;
    }

    // @param   point       Point to check if surrounded
    // @param   groupColor  Color of the group. Will check if surrounded by opposite color
    // @return  The # of Stones captured
    public int TryCaptureGroup(Vector2 point, GoColor groupColor)
    {
        // Check: Bounds
        if (!IsValidPoint(point))
        {
            return 0;
        }

        // Create hash to store all points that have been checked
        HashSet<GoPoint> hash_group = new HashSet<GoPoint>();
        // Create queue to store list of points that need to be checked
        Queue<GoPoint> queue_points = new Queue<GoPoint>();

        GoPoint point_curr;
        int count_captured = 0;
        int x_curr = (int)point.x;
        int y_curr = (int)point.y;
 
        // Retrieve initial point
        queue_points.Enqueue(gridPoints[x_curr, y_curr]);
        while (queue_points.Count > 0)
        {
            // Retrieve next point
            point_curr = queue_points.Dequeue();
            x_curr = (int)point_curr.PointBoard.x;
            y_curr = (int)point_curr.PointBoard.y;

            bool b_allied_piece = false;

            // Only proceed if we haven't seen this point yet
            if (!hash_group.Contains(point_curr))
            {
                // Check current point for an allied stone
                if (!point_curr.IsEmpty() && point_curr.GetStone().Color == groupColor)
                {
                    b_allied_piece = true;
                }
            }

            if (b_allied_piece)
            {
                // Remove piece
                RemovePiece(point_curr.PointBoard);
                count_captured++;

                // Retrieve adjacent points if allied
                List<Vector2> list_adj = GetAdjacentPoints(point_curr.PointBoard);
                foreach(Vector2 point_adj in list_adj)
                {
                    queue_points.Enqueue(gridPoints[(int)point_adj.x, (int)point_adj.y]);
                }
            }

            // Mark GoPoint as checked
            hash_group.Add(point_curr);
        }

        // Finally, return the # of Stones removed
        return count_captured;
    }

    // @return  A list of valid adjacent points to the given point
    private List<Vector2> GetAdjacentPoints(Vector2 pointCenter)
    {
        List<Vector2> list_udlr = new List<Vector2>();
        Vector2 point_curr;

        // UP
        point_curr = pointCenter + new Vector2(0, 1);
        if (IsValidPoint(point_curr))
        {
            list_udlr.Add(point_curr);
        }
        // DOWN
        point_curr = pointCenter + new Vector2(0, -1);
        if (IsValidPoint(point_curr))
        {
            list_udlr.Add(point_curr);
        }
        // LEFT
        point_curr = pointCenter + new Vector2(-1, 0);
        if (IsValidPoint(point_curr))
        {
            list_udlr.Add(point_curr);
        }
        // RIGHT
        point_curr = pointCenter + new Vector2(1, 0);
        if (IsValidPoint(point_curr))
        {
            list_udlr.Add(point_curr);
        }

        return list_udlr;
    }

    //  Retrieve immediately adjacent liberties for a given point. 
    //  Allied adjacent stones do NOT count as liberties.
    //  @param point    Point on the board to check for liberties
    //  @return         # of empy adjacent spaces
    public int GetLibertiesAdjacent(Vector2 point)
    {
        // Return -1 if invalid point
        if (!IsValidPoint(point))
        {
            return -1;
        }

        // Otherwise, count # of empty adjacent points
        int count_liberties     = 0;
        List<Vector2> list_adj  = GetAdjacentPoints(point);
        foreach (Vector2 point_adj in list_adj)
        {
            GoPoint gp_adj = gridPoints[(int)point_adj.x, (int)point_adj.y];
            if (gp_adj.IsEmpty())
            {
                count_liberties++;
            }
        }

        return count_liberties;
    }

    //////////////////////////////////////////////////////////////////////
    //// :NOTE: Adjacency Status Tracking. MAY BE OBSOLETE/UNNECESSARY
    //// Remove: NEW
    //private void UpdateAdjacentEmptySpaces(Vector2 pointCenter)
    //{
    //    UpdateAdjacencyState(pointCenter + new Vector2(-1, 0), GoPoint.FLAG_EMPTY, GoPoint.OFFSET_R);
    //    UpdateAdjacencyState(pointCenter + new Vector2(1, 0), GoPoint.FLAG_EMPTY, GoPoint.OFFSET_L);
    //    UpdateAdjacencyState(pointCenter + new Vector2(0, -1), GoPoint.FLAG_EMPTY, GoPoint.OFFSET_U);
    //    UpdateAdjacencyState(pointCenter + new Vector2(0, 1), GoPoint.FLAG_EMPTY, GoPoint.OFFSET_D);
    //}

    //// Add: NEW
    //private void UpdateAdjacentEmptySpaces(Vector2 pointCenter, bool bIsBlack)
    //{
    //    UpdateAdjacencyState(pointCenter + new Vector2(-1, 0), (bIsBlack ? GoPoint.FLAG_BLACK : GoPoint.FLAG_WHITE), GoPoint.OFFSET_R);
    //    UpdateAdjacencyState(pointCenter + new Vector2(1, 0), (bIsBlack ? GoPoint.FLAG_BLACK : GoPoint.FLAG_WHITE), GoPoint.OFFSET_L);
    //    UpdateAdjacencyState(pointCenter + new Vector2(0, -1), (bIsBlack ? GoPoint.FLAG_BLACK : GoPoint.FLAG_WHITE), GoPoint.OFFSET_U);
    //    UpdateAdjacencyState(pointCenter + new Vector2(0, 1), (bIsBlack ? GoPoint.FLAG_BLACK : GoPoint.FLAG_WHITE), GoPoint.OFFSET_D);
    //}

    //private void UpdateAdjacencyState(Vector2 point, int state, int directionOffset)
    //{
    //    // Return -1 if invalid point is requested
    //    if (!IsValidPoint(point))
    //    {
    //        return;
    //    }

    //    int x = (int)point.x;
    //    int y = (int)point.y;

    //    // Clear previous state
    //    int state_new = gridPoints[x, y].AdjSpacesState & ~(0x000F << directionOffset); // :NOTE: Offset before invert or you'll shift in zeroes!
    //    // Apply new state
    //    state_new |= (state << directionOffset);
    //    // Store
    //    gridPoints[x, y].AdjSpacesState = state_new;
    //}

    //private void PrintAdjacencyData(int x, int y)
    //{
    //    gridPoints[x, y].PrintAdjacencyData();
    //}
    //////////////////////////////////////////////////////////////////////

    /**
 * Class:   GroupInfo
 * Descr:   Stores information about a group of connected points.
 *          Can be a set of empty points (Region), or of one of the two colored stones (Block).
**/
    public class GroupInfo
    {
        /////////////////////////////////////////////
        // Member Variables
        protected GoColor groupType;

        protected List<Vector2> groupPoints;
        protected HashSet<Vector2> groupLiberties;

        public GoColor GroupType
        {
            get
            {
                return groupType;
            }
        }

        public int GroupSize
        {
            get
            {
                return groupPoints.Count;
            }
        }

        public List<Vector2> GroupPoints
        {
            get
            {
                return groupPoints;
            }
        }

        public int NumLiberties
        {
            get
            {
                return groupLiberties.Count;
            }
        }

        // A block is a group of connected stones of a single color
        public bool IsBlock
        {
            get
            {
                return (groupType >= 0);
            }
        }

        // A region is an group of connected empty points
        public bool IsRegion
        {
            get
            {
                return (groupType == GoColor.GC_Empty);
            }
        }

        /////////////////////////////////////////////
        // Functions
        // Constructor
        protected GroupInfo()
        {
            groupType = GoColor.GC_Empty;
            groupPoints = new List<Vector2>();
            groupLiberties = new HashSet<Vector2>();
        }

        // Factory
        // Create new Group for a given point on the board
        public static GroupInfo CreateGroup(GoBoard board, Vector2 point)
        {
            // Sanity Check: Cannot create group if point is not valid
            if (!board.IsValidPoint(point))
            {
                return null;
            }

            GroupInfo group = new GroupInfo();

            // Create hash to store all points that have been checked
            HashSet<GoPoint> hash_group = new HashSet<GoPoint>();
            // Create queue to store list of points that need to be checked
            Queue<GoPoint> queue_points = new Queue<GoPoint>();

            int x_curr = (int)point.x;
            int y_curr = (int)point.y;

            // Use initial point to determine the group's State (Empty/Black/White)
            GoPoint point_curr  = board.gridPoints[x_curr, y_curr];
            group.groupType     = point_curr.PointState;

            // Queue initial point
            queue_points.Enqueue(board.gridPoints[x_curr, y_curr]);
            while (queue_points.Count > 0)
            {
                // Retrieve next point
                point_curr = queue_points.Dequeue();
                x_curr = (int)point_curr.PointBoard.x;
                y_curr = (int)point_curr.PointBoard.y;

                bool b_state_same = false;

                // Only proceed if we haven't seen this point yet
                if (!hash_group.Contains(point_curr))
                {
                    // Check current point is the same state as our Group
                    if (point_curr.PointState == group.GroupType)
                    {
                        b_state_same = true;
                    }
                }

                // If same, then we want to add it to our Group and check adjacent points
                if (b_state_same)
                {
                    // Add Point to Group
                    group.GroupPoints.Add(point_curr.PointBoard);

                    // Retrieve adjacent points if allied
                    List<Vector2> list_adj = board.GetAdjacentPoints(point_curr.PointBoard);
                    foreach (Vector2 point_adj in list_adj)
                    {
                        queue_points.Enqueue(board.gridPoints[(int)point_adj.x, (int)point_adj.y]);
                    }
                }

                // Mark GoPoint as checked
                hash_group.Add(point_curr);
            }

            // If our Group's type is not territory, check for liberties
            if (group.GroupType != GoColor.GC_Empty)
            {
                foreach (Vector2 gp_curr in group.GroupPoints)
                {
                    List<Vector2> points_adj = board.GetAdjacentPoints(gp_curr);
                    foreach (Vector2 point_adj in points_adj)
                    {
                        if (board.GetPoint(point_adj).IsEmpty())
                        {
                            group.groupLiberties.Add(point_adj);
                        }
                    }
                }
            }

            return group;
        }

        public override string ToString()
        {
            string state = "";
            switch (GroupType)
            {
                case GoColor.GC_Empty:
                    state = "Empty";
                    break;
                case GoColor.GC_Black:
                    state = "Black";
                    break;
                case GoColor.GC_White:
                    state = "White";
                    break;
            }

            string s = "== GROUP INFO ==\n";
            s += "Type: " + state + "\n";
            s += "Size: " + GroupSize + "\n";
            if (GroupType != GoColor.GC_Empty)
            {
                s += "Liberties: " + groupLiberties.Count + "\n";
            }
            foreach (Vector2 point_curr in groupPoints)
            {
                s += "\t" + point_curr + "\n";
            }

            return s;
        }
    }
}
