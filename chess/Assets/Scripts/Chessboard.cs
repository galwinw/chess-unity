using System;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialMove {
        None = 0,
        EnPassant = 1,
        Castling = 2,
        Promotion = 3

    }

public class Chessboard : MonoBehaviour {
    [Header("Art Stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 0.7f;
    [SerializeField] private float deathSpacing = 0.5f;
    [SerializeField] private float deathOffset = 0.1f;
    [SerializeField] private float dragHeight = 1.0f;
    [SerializeField] private GameObject victoryScreen;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;


    // Logic
    private ChessPiece[,] chessPieces; // 2D array of chess pieces
    private ChessPiece currentlyDragging; // The piece that is currently being dragged
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();

    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    private const int TILE_COUNT_X = 8;  
    private const int TILE_COUNT_Y = 8; 
    private GameObject[,] tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private bool isWhiteTurn = true;

    private SpecialMove specialMove;


    private void Awake() { // Awake is called before Start

        isWhiteTurn = true;
        
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositioningAllPieces();
    }

    private void Update() { // Update is called once per frame
        if (!currentCamera) {
            currentCamera = Camera.main;
            return;
        }
        RaycastHit info; 
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition); // Cast a ray from the camera to the mouse position
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight"))) { // If we hit something
            // Get the tile index of the hit tile
            Vector2Int hitPositoin = LookupTileIndex(info.transform.gameObject); // LookupTileIndex returns the index of the tile that was hit

            // If the hit tile is different from the current hover tile
            //If we are hovering over a tile for the first time
            if (currentHover == -Vector2Int.one) {
                currentHover = hitPositoin;
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Hover"); 
            } 
            // if we are hovering over a different tile
            if (currentHover != hitPositoin) {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPositoin;
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we press down on the mouse
            if (Input.GetMouseButtonDown(0)) {
                if (chessPieces[hitPositoin.x, hitPositoin.y] != null) {
                    currentlyDragging = null;
                    // is it our turn?
                    if (isWhiteTurn == (chessPieces[hitPositoin.x, hitPositoin.y].team == 0)) {
                        
                        currentlyDragging = chessPieces[hitPositoin.x, hitPositoin.y];

                        // Get a list of where I can go, highlight those tiles
                        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        
                        // Get a list of special Moves
                        specialMove = currentlyDragging.GetSpecialMove(ref chessPieces, ref moveList, ref availableMoves);

                        PreventCheck();
                        HighlightTiles();
                    } 

                }
            }

            // If we release the mouse
            if (currentlyDragging != null && Input.GetMouseButtonUp(0)) {
                Vector2Int previosPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                bool validMove = MoveTo(currentlyDragging, hitPositoin.x, hitPositoin.y);
                
                if(!validMove) {
                    currentlyDragging.SetPosition(GetTileCenter(previosPosition.x, previosPosition.y));
                }
                currentlyDragging = null;
                RemoveHighlightTiles();
            } else {
                // If we are dragging a piece
                if (currentlyDragging) {
                    Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
                    float distance = 0.0f;

                    if (horizontalPlane.Raycast(ray, out distance)) {
                        currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragHeight, false);
                    }
                }
            }

        } else {
            // If we are not hovering over a tile after a frame
            if (currentHover != -Vector2Int.one) {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonDown(0)) {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
                RemoveHighlightTiles();
            }

        }

        // If we are dragging a piece
        if (currentlyDragging) {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;

            if (horizontalPlane.Raycast(ray, out distance)) {
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragHeight, false);
            }
        } 


    }

    // Generate all tiles and board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY) { // Generate all tiles
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountY / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++) {
            for (int y = 0; y < tileCountY; y++) {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
        
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y) { // Generate a single tile
        GameObject tileObject = new GameObject(string.Format("Tile X:{0} Y:{1}", x, y));
        tileObject.transform.parent = transform;// Make the tile a child of the chessboard

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];// 4 vertices to form a square
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize,yOffset, (y + 1) * tileSize) - bounds;
        
        int[] triangles = new int[6] { 0, 1, 2, 1, 3, 2}; // 2 triangles to form a square
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>(); 


        return tileObject;
    }

    // Generate all pieces
    private void SpawnAllPieces() {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0;
        int blackTeam = 1;

        //White Team
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for (int i = 0; i < TILE_COUNT_X; i++) {
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        }


        //Black Team
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for (int i = 0; i < TILE_COUNT_X; i++) {
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        }
    }

    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team) {

        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>(); // Instantiate the prefab
        cp.type = type;
        cp.team = team;
        Material[] materials = cp.GetComponent<MeshRenderer>().materials;
        materials[1] = teamMaterials[team];
        cp.GetComponent<MeshRenderer>().materials = materials; 

        return cp;
    }

    //Positioning
    private void PositioningAllPieces() {
        for (int x = 0; x < TILE_COUNT_X; x++) {
            for (int y = 0; y < TILE_COUNT_Y; y++) {
                if (chessPieces[x, y] != null) {
                    PositioningSinglePiece(x, y, true);
                }
            }
        }
    }

    private void PositioningSinglePiece(int x, int y, bool force = false) {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x,y].SetPosition(GetTileCenter(x, y), force);
    }

    private Vector3 GetTileCenter(int x, int y) {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    // Highlighting
    private void HighlightTiles() {
        for (int i = 0; i < availableMoves.Count; i++) {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }

    private void RemoveHighlightTiles() {
        for (int i = 0; i < availableMoves.Count; i++) {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }

        availableMoves.Clear();
    }

    // Checkmate
    private void CheckMate(int team) {
        DisplayVictory(team);
    }

    private void DisplayVictory(int winningteam) {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningteam).gameObject.SetActive(true);
        Debug.Log("Victory");
    }

    public void OnResetButton() {
        //UI
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.SetActive(false);

        // Fields Resets
        currentlyDragging = null;
        availableMoves.Clear();
        moveList.Clear();


        // Clean up
        for (int x = 0; x < TILE_COUNT_X; x++) {
            for (int y = 0; y < TILE_COUNT_Y; y++) {
                if (chessPieces[x, y] != null) {
                    Destroy(chessPieces[x, y].gameObject);
                }
            }
        }

        for (int i = 0; i < deadWhites.Count; i++) {
            Destroy(deadWhites[i].gameObject);
        }
        for (int i = 0; i < deadBlacks.Count; i++) {
            Destroy(deadBlacks[i].gameObject);
        }

        deadWhites.Clear();
        deadBlacks.Clear();

        SpawnAllPieces();
        PositioningAllPieces();
        isWhiteTurn = true;
    }

    public void OnExitButton() {
        Application.Quit();
    }

    // Special Moves
    private void ProcessSpecialMove() {

        switch (specialMove) {
            case SpecialMove.EnPassant:
                var newMove = moveList[moveList.Count - 1];
                ChessPiece myPawn = chessPieces[newMove[1].x, newMove[1].y];
                var targetPawnPosition = moveList[moveList.Count - 2];
                ChessPiece enemyPawn = chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];

                if (myPawn.currentX == enemyPawn.currentX) {
                    if (myPawn.currentY == enemyPawn.currentY - 1 || myPawn.currentY == enemyPawn.currentY + 1) {
                        if (enemyPawn.team == 0) {
                            deadWhites.Add(enemyPawn);
                            enemyPawn.SetScale(Vector3.one * deathSize);
                            enemyPawn.SetPosition(new Vector3(8 * tileSize - deathOffset, yOffset, -1 * tileSize)
                                - bounds
                                + new Vector3(tileSize / 2, 0, tileSize / 2)
                                + (Vector3.forward * deathSpacing) * deadWhites.Count);
                        } 
                        else {
                            deadBlacks.Add(enemyPawn);
                            enemyPawn.SetScale(Vector3.one * deathSize);
                            enemyPawn.SetPosition(new Vector3(-1 * tileSize + deathOffset, yOffset, 8 * tileSize)
                                - bounds
                                + new Vector3(tileSize / 2, 0, tileSize / 2)
                                + (Vector3.back * deathSpacing) * deadBlacks.Count);
                        }
                        chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y] = null;
                    }
                }

                break;
            case SpecialMove.Castling:
                //Castling();
                Vector2Int[] lastMove = moveList[moveList.Count - 1];

                // Left Rook
                if (lastMove[1].x == 2) {
                    if (lastMove[1].y == 0) { // White side
                        chessPieces[3, 0] = chessPieces[0,0];
                        PositioningSinglePiece(3,0);
                        chessPieces[0, 0] = null;
                    } else if (lastMove[1].y == 7) { // Black side
                        chessPieces[3, 7] = chessPieces[0, 7];
                        PositioningSinglePiece(3, 7);
                        chessPieces[0, 7] = null;
                    }                    
                } else if (lastMove[1].x == 6) { // Right Rook
                    if (lastMove[1].y == 0) { // White side
                        chessPieces[5, 0] = chessPieces[7, 0];
                        PositioningSinglePiece(5, 0);
                        chessPieces[7, 0] = null;
                    } else if (lastMove[1].y == 7) { // Black side
                        chessPieces[5, 7] = chessPieces[7, 7];
                        PositioningSinglePiece(5, 7);
                        chessPieces[7, 7] = null;
                    }
                }

                break;
            case SpecialMove.Promotion:
                //Promotion();
                Vector2Int[] lastMovePos = moveList[moveList.Count - 1];
                ChessPiece targetPawn = chessPieces[lastMovePos[1].x, lastMovePos[1].y];

                if (targetPawn.type == ChessPieceType.Pawn) {
                    if (targetPawn.team == 0 && targetPawn.currentY == 7) {
                        ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen, 0);
                        Destroy(chessPieces[lastMovePos[1].x, lastMovePos[1].y].gameObject);
                        newQueen.transform.position =  chessPieces[lastMovePos[1].x, lastMovePos[1].y].transform.position;
                        chessPieces[lastMovePos[1].x, lastMovePos[1].y] = newQueen;
                        PositioningSinglePiece(lastMovePos[1].x, lastMovePos[1].y);

                    } 
                    if (targetPawn.team == 1 && targetPawn.currentY == 0) {
                        ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen, 1);
                        Destroy(chessPieces[lastMovePos[1].x, lastMovePos[1].y].gameObject);
                        newQueen.transform.position =  chessPieces[lastMovePos[1].x, lastMovePos[1].y].transform.position;
                        chessPieces[lastMovePos[1].x, lastMovePos[1].y] = newQueen;
                        PositioningSinglePiece(lastMovePos[1].x, lastMovePos[1].y);
                    }
                }


                break;
            default:
                break;
        }
    }

    private void PreventCheck() {
        ChessPiece targetKing = null;
        for (int x = 0; x < TILE_COUNT_X; x++) {
            for (int y = 0; y < TILE_COUNT_Y; y++) {
                if (chessPieces[x, y] != null) {
                    if (chessPieces[x, y].type == ChessPieceType.King && chessPieces[x, y].team == currentlyDragging.team) {
                        targetKing = chessPieces[x, y];
                    }
                }
            }
        }

        //Since we are sending in ref availableMoves, we will be deleting moves that are putting us in check
        SimulateMoveForSinglePiece(currentlyDragging, ref availableMoves, targetKing);
    }

    private void SimulateMoveForSinglePiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetKing) { 
        //Save the current values, to reset after the function call
        int actualX = cp.currentX;
        int actualY = cp.currentY;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();

        // Going through all the moves, simulate them and check if we're in check
        for (int i = 0; i < moves.Count; i++) {
            int simX = moves[i].x;
            int simY = moves[i].y;

            Vector2Int kingPositionThisSim = new Vector2Int(targetKing.currentX, targetKing.currentY); // Save the king position before the simulation
            // Did we simulate the king move
            if (cp.type == ChessPieceType.King) {
                kingPositionThisSim = new Vector2Int(simX, simY);
            }

            // Copy the board and not a reference (hard copy)
            ChessPiece[,] simulatedBoard = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
            List<ChessPiece> simAttackingPieces = new List<ChessPiece>();

            for (int x = 0; x < TILE_COUNT_X; x++) {
                for (int y = 0; y < TILE_COUNT_Y; y++) {
                    if (chessPieces[x, y] != null) {
                        simulatedBoard[x, y] = chessPieces[x, y];
                        if (simulatedBoard[x, y].team != cp.team) {
                            simAttackingPieces.Add(simulatedBoard[x, y]);
                        }
                    }
                }
            }

            // Simulate the move
            simulatedBoard[actualX, actualY] = null;
            cp.currentX = simX;
            cp.currentY = simY;
            simulatedBoard[simX, simY] = cp;

            // Did one of the Piece get taken down during our simulation
            var deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY);
            if (deadPiece) {
                simAttackingPieces.Remove(deadPiece);
            }

            // Get all the simulated attacking pieces moves
            List<Vector2Int> simMoves = new List<Vector2Int>();
            for (int j = 0; j < simAttackingPieces.Count; j++) {
                var pieceMoves = simAttackingPieces[j].GetAvailableMoves(ref simulatedBoard, TILE_COUNT_X, TILE_COUNT_Y);
                for (int k = 0; k < pieceMoves.Count; k++) {
                    simMoves.Add(pieceMoves[k]);
                }
            }

            // Is the king in trouble? if so, remove the move
            if (ContainsValidMove(ref simMoves, kingPositionThisSim)) { // this means one of the simulated moves contains the kings position
                movesToRemove.Add(moves[i]);
            }

            // Restore the actual CP data
            cp.currentX = actualX;
            cp.currentY = actualY;

        }

        // Remove from the current avvailable move list
        for ( int i = 0; i < movesToRemove.Count; i++) {
            moves.Remove(movesToRemove[i]);
        }
    }
    
    private bool CheckForMate() {
        var lastMove = moveList[moveList.Count - 1];
        int targetTeam = (chessPieces[lastMove[1].x, lastMove[1].y].team == 0) ? 1 : 0;

        List<ChessPiece> attackingPieces = new List<ChessPiece>();
        List<ChessPiece> defendingPieces = new List<ChessPiece>();


        ChessPiece targetKing = null;
        for (int x = 0; x < TILE_COUNT_X; x++) {
            for (int y = 0; y < TILE_COUNT_Y; y++) {
                if (chessPieces[x, y] != null) {
                    if (chessPieces[x,y].team == targetTeam) {
                        defendingPieces.Add(chessPieces[x, y]);
                        if (chessPieces[x, y].type == ChessPieceType.King) {
                            targetKing = chessPieces[x, y];
                        }
                    } else {
                        attackingPieces.Add(chessPieces[x, y]);
                    }
                }
            }
        }

        // Is the king attacked rn?
        List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();
        for (int i = 0; i < attackingPieces.Count; i++) {
            var pieceMoves = attackingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
            for (int j = 0; j < pieceMoves.Count; j++) {
                currentAvailableMoves.Add(pieceMoves[j]);
            }
        }

        if (ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.currentX, targetKing.currentY))) { // if we enter this it means we are currently in check
            // king is under attack, can we move something to help him?
            for (int i = 0; i < defendingPieces.Count; i++) {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                // Since we're sending ref availableMoves, we will be deleting moves that are putting us in check
                SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, targetKing);

                if (defendingMoves.Count != 0) {
                    return false;
                }  
            }

            return true;//CheckMate exit
        }

        return false;
    }

    //Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo) {
        for (int x = 0; x < TILE_COUNT_X; x++) {
            for (int y = 0; y < TILE_COUNT_Y; y++) {
                if (tiles[x, y] == hitInfo) {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);// Not found, this should never happen
    }
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2Int pos) { 
        for (int i = 0; i < moves.Count; i++) {
            if (moves[i].x == pos.x && moves[i].y == pos.y) return true;
        }
        return false;
    }
    private bool MoveTo(ChessPiece cp, int x, int y) { // Move a piece to a new position
        if (!ContainsValidMove(ref availableMoves, new Vector2Int(x, y))) return false;

        Vector2Int previosPosition = new Vector2Int(cp.currentX,cp.currentY);

        if (chessPieces[x,y] != null) {
            ChessPiece ocp = chessPieces[x,y]; // Other chess piece

            // own team
            if (cp.team == ocp.team) {
                return false;
            } 

            // different team
            if (ocp.team == 0) {

                // king dies
                if (ocp.type == ChessPieceType.King) {
                    CheckMate(1);
                }


                deadWhites.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3(8 * tileSize - deathOffset, yOffset, -1 * tileSize)
                                - bounds
                                + new Vector3(tileSize / 2, 0, tileSize / 2)
                                + (Vector3.forward * deathSpacing) * deadWhites.Count);
            } else {

                // king dies
                if (ocp.type == ChessPieceType.King) {
                    CheckMate(0);
                }

                deadBlacks.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3(-1 * tileSize + deathOffset, yOffset, 8 * tileSize)
                                - bounds
                                + new Vector3(tileSize / 2, 0, tileSize / 2)
                                + (Vector3.back * deathSpacing) * deadBlacks.Count);
            }

        }

        chessPieces[x, y] = cp;
        chessPieces[previosPosition.x, previosPosition.y] = null;

        PositioningSinglePiece(x,y);

        isWhiteTurn = !isWhiteTurn;
        moveList.Add(new Vector2Int[] { previosPosition, new Vector2Int(x, y) });
        
        ProcessSpecialMove();
        if (CheckForMate()) {
            CheckMate(cp.team);
        }
        return true;        
    }
    

}
