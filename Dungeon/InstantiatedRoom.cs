using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundtilemap;
    [HideInInspector] public Tilemap decorations1Tilemap;
    [HideInInspector] public Tilemap decorations2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public int[,] aStarMovementPenalty; // This is used to store the movement penalties for the AStar Pathfinding
    [HideInInspector] public int[,] aStarItemObstacles; // Store the position of moveable items which acts as an obstacle
  
    [HideInInspector] public Bounds roomColliderBounds;
    [HideInInspector] public List<MoveableItem> moveableItemsList = new List<MoveableItem>();

    private BoxCollider2D boxCollider2D;
    private const string chestRoomName = "Chest Room";

    #region Header References
    [Header("REFERENCES")]
    [Tooltip("Populate with the environment child placeholder gameobject")]
    #endregion
    [SerializeField] private GameObject environmentGameObject;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        roomColliderBounds = boxCollider2D.bounds;
    }

    private void Start()
    {
        UpdateMoveableObstacles();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Settings.playerTag) && room != GameManager.Instance.GetCurrentRoom())
        {
            //Set room as visited
            this.room.isPreviouslyVisited = true;

            //Call the room change event
            StaticEventHandler.CallRoomChangedEvent(room);

            PlayChestRoomSoundOnEntry();
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < (room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1); i++)
        {
            for (int j = 0; j < (room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1); j++)
            {
                if (aStarItemObstacles[i, j] == 0)
                {
                    Vector3 worldCellPos = grid.CellToWorld(new Vector3Int(i + room.tilemapLowerBounds.x,
                        j + room.tilemapLowerBounds.y, 0));

                    Gizmos.DrawWireCube(new Vector3(worldCellPos.x + 0.5f, worldCellPos.y + 0.5f, 0f), Vector3.one);
                }
            }
        }
    }

    /// <summary>
    /// Initialise The Instantiated Rooms.
    /// </summary>
    public void Initialise(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        BlockOffUnusedDoorways();

        AddObstaclesAndPreferredPaths();

        CreateItemObstaclesArray();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
    }

    /// <summary>
    /// Populate The Grid And Tilemap Variables Specified Above.
    /// </summary>
    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        //Get the grid component
        grid = roomGameobject.GetComponentInChildren<Grid>();

        //Get the tilemap component in the children
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.CompareTag("groundTilemap"))
            {
                groundtilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("decoration1Tilemap"))
            {
                decorations1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("decoration2Tilemap"))
            {
                decorations2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("frontTilemap"))
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("collisionTilemap"))
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("minimapTilemap"))
            {
                minimapTilemap = tilemap;
            }
        }
    }

    /// <summary>
    /// Blocks Off Unused Doorways In The Room
    /// </summary>
    private void BlockOffUnusedDoorways()
    {
        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.isConnected)
                continue;
            //Block unconnected doorways using tiles on tilemaps
            if (collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }
            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }
            if (groundtilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundtilemap, doorway);
            }
            if (decorations1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decorations1Tilemap, doorway);
            }
            if (decorations2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decorations2Tilemap, doorway);
            }
            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }
        }
    }

    /// <summary>
    /// Blocks A Doorway On A Tilemap Layer
    /// </summary>
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.doorOrientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;
            
            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;
            
            case Orientation.none:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Blocks The Doorway Horizontally
    /// </summary>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                //Get the rotation of the tiles that are being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos,
                    startPosition.y - yPos, 0));

                //Copy Tile
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                //Set rotation of the tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos,0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Blocks The Doorway Vertically
    /// </summary>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                //Get the rotation of the tiles that are being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos,
                    startPosition.y - yPos, 0));

                //Copy Tile
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                //Set rotation of the tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Update Obstacles Used In The AStar Pathfinding
    /// </summary>
    private void AddObstaclesAndPreferredPaths()
    {
        //this array will be populated with wall obstacles
        aStarMovementPenalty = new int[room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1,
            room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1];

        //Loop through all grid squares
        for (int x = 0; x < (room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1; y++)
            {
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.tilemapLowerBounds.x, y + room.tilemapLowerBounds.y, 0));

                foreach (TileBase collTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if (tile == collTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                //Add preferred path for enemies
                if (tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                }
            }
        }
    }

    /// <summary>
    /// Add The Doors To The Dungeon Rooms, No Doors Are Added If It's A Corridor
    /// </summary>
    private void AddDoorsToRooms()
    {
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return;

        //Instantiate the door prefab at the doorway positions
        foreach (Doorway doorway in room.doorwayList)
        {
            //If the door prefab isn't null and the door is connected
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;

                if (doorway.doorOrientation == Orientation.north)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.doorPosition.x + tileDistance / 2, doorway.doorPosition.y + tileDistance, 0f);
                }
                else if (doorway.doorOrientation == Orientation.south)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.doorPosition.x + tileDistance / 2, doorway.doorPosition.y, 0f);
                }
                else if (doorway.doorOrientation == Orientation.east)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.doorPosition.x + tileDistance, doorway.doorPosition.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.doorOrientation == Orientation.west)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.doorPosition.x, doorway.doorPosition.y + tileDistance * 1.25f, 0f);
                }

                //Gets the door component
                Door doorComponent = door.GetComponent<Door>();

                //Check if the door is part of a Boss Room
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;
                    //Lock the door to prevent access to the room
                    doorComponent.LockDoor();

                    //Instantiante the BossSkull prefab to let the player know that the room is a boss room
                    GameObject skullGameObject = Instantiate(GameResources.Instance.skullMinimapPrefab, gameObject.transform);
                    skullGameObject.transform.localPosition = door.transform.localPosition;
                }
            }
        }
    }

    /// <summary>
    /// Disable The Collision Tilemap Renderer
    /// </summary>
    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    /// <summary>
    /// Locks All The Doors In A Given Room
    /// </summary>
    public void LockDoorsAfterEntry()
    {
        Door[] doorsArray = GetComponentsInChildren<Door>();

        //Close all the doors in the room
        foreach (Door door in doorsArray)
        {
            door.LockDoor();
        }

        //Disables the room trigger collider
        DisableRoomCollider();
    }

    /// <summary>
    /// Unlock the doors
    /// </summary>
    public void UnlockTheDoors(float doorUnlockingDelay)
    {
        StartCoroutine(UnlockDoorsRoutine(doorUnlockingDelay));
    }

    /// <summary>
    /// Unlocks the doors with a coroutine, yielding for the amount of seconds specified in the Settings class
    /// </summary>
    private IEnumerator UnlockDoorsRoutine(float doorUnlockingDelay)
    {
        if (doorUnlockingDelay > 0)
            yield return new WaitForSeconds(doorUnlockingDelay);

        Door[] doorsArray = GetComponentsInChildren<Door>();

        //Open the doors
        foreach (Door door in doorsArray)
        {
            door.UnlockDoor();
        }

        EnableRoomCollider();
    }

    /// <summary>
    /// Creates the item obstacles array
    /// </summary>
    private void CreateItemObstaclesArray()
    {
        //This array will be populated during gameplay with any moveable obstacle
        aStarItemObstacles = new int[room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1,
            room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1];
    }

    /// <summary>
    /// Initialize the array above this method
    /// </summary>
    private void InitializeItemObstaclesArray()
    {
        for (int x = 0; x < (room.tilemapUpperBounds.x - room.tilemapLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.tilemapUpperBounds.y - room.tilemapLowerBounds.y + 1); y++)
            {
                //Set the default penalty for the grid squares, with higher penalty the enemies will avoid this grid squares
                aStarItemObstacles[x, y] = Settings.defaultAStarMovementPenalty;
            }
        }
    }

    /// <summary>
    /// Updates the array of the moveable obstacles
    /// </summary>
    public void UpdateMoveableObstacles()
    {
        InitializeItemObstaclesArray();

        foreach (MoveableItem moveable in moveableItemsList)
        {
            Vector3Int minColliderBounds = grid.WorldToCell(moveable.boxCollider2D.bounds.min);
            Vector3Int maxColliderBounds = grid.WorldToCell(moveable.boxCollider2D.bounds.max);

            //Loop through and add moveable item colliders bounds to the obstacle array
            for (int i = minColliderBounds.x; i <= maxColliderBounds.x; i++)
            {
                for (int j = minColliderBounds.y; j <= maxColliderBounds.y; j++)
                {
                    aStarItemObstacles[i - room.tilemapLowerBounds.x, j - room.tilemapLowerBounds.y] = 0;
                }
            }
        }
    }

    public void DeactivateEnvironmentObjects()
    {
        if (environmentGameObject != null)
        {
            environmentGameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Disables the trigger collider, which is used to know when the player enters a room
    /// </summary>
    private void DisableRoomCollider()
    {
        boxCollider2D.enabled = false;   
    }

    public void ActivateEnvironmentObjects()
    {
        if (environmentGameObject != null)
        {
            environmentGameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Enables the trigger collider, which is used to know when the player enters a room
    /// </summary>
    private void EnableRoomCollider()
    {
        boxCollider2D.enabled = true;
    }

    private void PlayChestRoomSoundOnEntry()
    {
        if (room.roomNodeType.roomNodeTypeName != chestRoomName)
            return;

        int RI = Random.Range(0, GameResources.Instance.chestRoomSoundEffects.Length);
        
        SoundManager.Instance.PlaySoundEffect(GameResources.Instance.chestRoomSoundEffects[RI]);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(environmentGameObject), environmentGameObject);
    }
#endif
    #endregion
}
