using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonoBehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new();

    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildedSuccesfully;

    protected override void Awake()
    {
        base.Awake();

        //Load the room node type list
        LoadRoomNodeTypeList();
    }

    private void OnEnable()
    {
        //Set dimmed material to zero
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        //Set dimmed material to fully visible
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    /// <summary>
    /// Load The Room Node Type List
    /// </summary>
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// Generates the random dungeon, returns true if succeded, false if failed to generate the dungeon
    /// </summary>
    /// <returns></returns>
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplatesList;

        //Load the room template SO into the dictionary
        LoadRoomTemplateIntoDictionary();
        dungeonBuildedSuccesfully = false;
        int dungeonBuildAttemps = 0;

        while (!dungeonBuildedSuccesfully && dungeonBuildAttemps < Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttemps++;

            //Select a random room node graph from the node graph list 
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphsList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildedSuccesfully = false;

            //Loop until dungeon succesfully built or the max attemps for rebuilding the node graph had been reached
            while (!dungeonBuildedSuccesfully && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForNodeGraph)
            {
                //Clears the dungeon room gameobjects and the room node dictionary
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                //Attempt to build a random dungeon with the selected room node graph
                dungeonBuildedSuccesfully = AttemptToBuildRandomDungeon(roomNodeGraph);

            }

            //Put Outside the while loop if it doesn't work
            if (dungeonBuildedSuccesfully)
            {
                //Instantiate room gameobjects
                InstantiateRoomGameobjects();
            }
        }

        return dungeonBuildedSuccesfully;
    }

    /// <summary>
    /// Load The Room Templates Into The Dictionary
    /// </summary>
    private void LoadRoomTemplateIntoDictionary()
    {
        //Clear room template dictionary
        roomTemplateDictionary.Clear();
        
        //Load room template list into dictionary
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicated Room Template Key In " + roomTemplateList);
            }
        }
    }

    /// <summary>
    /// Attemps To Randomly Generate The Dungeon For The Specified Room Node Graph, Returns True If Succeded,
    /// Returns False If Failed And Another Attempt Is Required.
    /// </summary>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        //Creates the Queue for the open room node graphs
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        //Add entrance node to the Queue, from the room node graph
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node, Cannot Procced");
            return false; // Dungeon Not Built
        }

        bool noRoomOverlaps = true;

        //Process the open room nodes in queue
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        //If all the room nodes have been processed without overlaps, then return true
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Process The Room Nodes In The Open Room Node Queue, Returning True If There Are No Overlaps 
    /// </summary>
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        //While theres rooms in the queue and no rooms overlaps detected
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            //Get next room node from the queue
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            //Add the child room nodes from the graph to the queue
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            //If the room is the entrance mark as positioned and add it to the dictionary
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                //Add Room to dictionary
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            //If the room node type isn't an entrance
            else
            {
                //Else get parent room for node
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                //See if the room can be placed without any overlaps
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    /// <summary>
    /// Attempt To Place The Room Node In The Dungeon - If The Node Can Be Placed, Returns The Node, 
    /// Else Returns Null
    /// </summary>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        //Initialise and assume there are overlaps, until prove otherwise
        bool roomOverlaps = true;

        //Do While Room Overlaps - try to place against all available doorways of the parent until
        //the room is succesfully placed without overlaps
        while (roomOverlaps)
        {
            //Select random unconnected available doorway for parent
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorwayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                //If no more doorways to try, then overlap failure
                return false; //Room overlaps
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            //Get a random room template for the room node that is consistent with the parent door orientation
            RoomTemplateSO roomTemplate = GetRandomRoomTemplateConsistenWithParent(roomNode, doorwayParent);

            //Create A Room
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            //Place the room - returns true if the room doesn't overlap
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                //If room doesn't overlap
                roomOverlaps = false;

                //Mark room as positioned
                room.isPositioned = true;

                //Add room to dictionary
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }
        return true; //No room overlaps
    }

    /// <summary>
    /// Gets A Random Child Room Template That Matches The Parent Orientation
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplateConsistenWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;
        
        //If room node is a corridor, then select random corridor with the correct orientation,
        //based on the parent doorway orientation
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.doorOrientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));  
                    break;
                
                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;
                
                case Orientation.none:
                    break;
                
                default:
                    break;
            }
        }
        //Else select random room template
        else
        {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomTemplate;
    }

    /// <summary>
    /// Place The Room - Returns True If The Room Doesn't Overlap, False If It Does
    /// </summary>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {
        //Get current room doorway position
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorwayList);

        //Return if there's no doorway in the room opposite to the parent doorway
        if (doorway == null)
        {
            //Just mark the parent doorway as unavailable so we don't try and connect it again
            doorwayParent.isUnavailable = true;

            return false;
        }

        //Calculate the 'world' grip position of the parent doorway position
        Vector2Int parentDoorwayPosition = parentRoom.worldLowerBounds + doorwayParent.doorPosition - parentRoom.tilemapLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        //Calculate adjustment position offset based on room doorway position that we are trying to connect (e.g. if
        //this doorway is west, then we need to add (1,0) to the east parent doorway)
        switch (doorway.doorOrientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
            
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;
            
            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;
            
            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;
            
            case Orientation.none:
                break;
            default:
                break;
        }

        //Calculate the room lower and upper bounds based on position to align with the parent doorway 
        room.worldLowerBounds = parentDoorwayPosition + adjustment + room.tilemapLowerBounds - doorway.doorPosition;
        room.worldUpperBounds = room.worldLowerBounds + room.tilemapUpperBounds - room.tilemapLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            //Mark the doorways as connected & unavailable
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            //Return true to show rooms have been connected with no overlapping
            return true;
        }
        else
        {
            //Just mark the parent doorway as unavailable so we don't try and connect it again
            doorwayParent.isUnavailable = true;

            return false;
        }
    }

    /// <summary>
    /// Get The Doorway From The Doorway List That Has The Opposite Orientation To The Doorway
    /// </summary>
    private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> doorwayList)
    {
        foreach (Doorway doorwayToCheck in doorwayList)
        {
            if (doorwayParent.doorOrientation == Orientation.east && doorwayToCheck.doorOrientation == Orientation.west)
            {
                return doorwayToCheck;
            }
            else if (doorwayParent.doorOrientation == Orientation.west && doorwayToCheck.doorOrientation == Orientation.east)
            {
                return doorwayToCheck;
            }
            else if (doorwayParent.doorOrientation == Orientation.north && doorwayToCheck.doorOrientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            else if (doorwayParent.doorOrientation == Orientation.south && doorwayToCheck.doorOrientation == Orientation.north)
            {
                return doorwayToCheck;
            }
        }

        return null;
    }

    /// <summary>
    /// Check For The Rooms That Overlap The Upper And Lower Bounds Parameters,
    /// And If There Are Overlapping Rooms, Then Return The Room,
    /// Otherwise Null.
    /// </summary>
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        //Iterate throug all rooms
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            //Skip if is the same room as roomToTest or the room hasn't been positiones
            if (room.id == roomToTest.id || !room.isPositioned)
                continue;

            //If room overlaps
            if (IsOverlappingRoom(roomToTest, room))
            {
                return room;
            }
        }

        return null;
    }

    /// <summary>
    /// Check If 2 Rooms Overlap With Each Other - Return True If They Overlap, False If Not.
    /// </summary>
    private bool IsOverlappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverlappingInterval(room1.worldLowerBounds.x, room1.worldUpperBounds.x, 
            room2.worldLowerBounds.x, room2.worldUpperBounds.x);

        bool isOverlappingY = IsOverlappingInterval(room1.worldLowerBounds.y, room1.worldUpperBounds.y,
            room2.worldLowerBounds.y, room2.worldUpperBounds.y);

        if (isOverlappingX && isOverlappingY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Check If Interval 1 Overlaps With Interval 2 - This Method Is Used By The IsOverlappingRoom Method, Just Above
    /// </summary>
    private bool IsOverlappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Get A Random Room Template From The roomTemplateList That Matches The Room Type And Returns it
    /// Returns Null If No Matching Room Template Is Found
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new();

        //Loop through room template list
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            //Add matching room templates
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        //Return null if list is empty
        if (matchingRoomTemplateList.Count == 0)
             return null;

        //Select random room template from the list and return
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }

    /// <summary>
    /// Get Unconnected Doorways
    /// </summary>
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        //Loop through the doorway list
        foreach (Doorway doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
            {
                yield return doorway;
            }
        }
    }

    /// <summary>
    /// Creates The Room Based On The RoomTemplate And LayoutNode, And Returns The Created Room
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        //Initialise room from template
        Room room = new Room
        {
            templateID = roomTemplate.guid,
            id = roomNode.id,
            roomTilemapPrefab = roomTemplate.prefab,
            roomNodeType = roomTemplate.roomNodeType,

            ambientMusic = roomTemplate.ambientMusic,
            battleMusic = roomTemplate.battleMusic,

            worldLowerBounds = roomTemplate.lowerBounds,
            worldUpperBounds = roomTemplate.upperBounds,
            spawnPositionArray = roomTemplate.spawnPositionArray,

            enemiesByLevelList = roomTemplate.enemiesByLevelList,
            roomLevelEnemySpawnParametersList = roomTemplate.roomEnemySpawnParemetersList,

            tilemapLowerBounds = roomTemplate.lowerBounds,
            tilemapUpperBounds = roomTemplate.upperBounds,

            childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList),
            doorwayList = CopyDoorwayList(roomTemplate.doorwayList)
        };

        //Set parent id for room
        if (roomNode.parentRoomNodeIDList.Count ==0) //This means is the Entrace
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            //Set the entrance as the current room in the game manager
            GameManager.Instance.SetCurrentRoom(room);
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        //If there are no enemies to spawn in this room, then default it to be clear of enemies
        if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
        {
            room.isClearOfEnemies = true;
        }

        return room;
    }

    /// <summary>
    /// Select a random room node graph from the list of room node graphs
    /// </summary>
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphsList)
    {
        if (roomNodeGraphsList.Count > 0)
        {
            return roomNodeGraphsList[UnityEngine.Random.Range(0, roomNodeGraphsList.Count)];
        }
        else
        {
            Debug.Log("No Room Node Graphs In List");
            return null;
        }
    }

    /// <summary>
    /// Creates A Deep Copy Of The Doorway List
    /// </summary>
    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway
            {
                doorPosition = doorway.doorPosition,
                doorOrientation = doorway.doorOrientation,
                doorPrefab = doorway.doorPrefab,
                isConnected = doorway.isConnected,
                isUnavailable = doorway.isUnavailable,
                doorwayStartCopyPosition = doorway.doorwayStartCopyPosition,
                doorwayCopyTileWidth = doorway.doorwayCopyTileWidth,
                doorwayCopyTileHeight = doorway.doorwayCopyTileHeight
            };

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }

    /// <summary>
    /// Creates A Deep Copy Of The String List
    /// </summary>
    /// <param name="childRoomNodeIDList"></param>
    /// <returns></returns>
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

    /// <summary>
    /// Instantiate The Dungeon Room Gameobjects From The Prefabs
    /// </summary>
    private void InstantiateRoomGameobjects()
    {
        foreach (KeyValuePair<string,Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            //Calculate room position (remember the room instantiation position needs to be adjusted by the room template (tilemap) lower bounds)
            Vector3 roomPosition = new Vector3(room.worldLowerBounds.x - room.tilemapLowerBounds.x, 
                room.worldLowerBounds.y - room.tilemapLowerBounds.y, 0f);

            //Instantiate room
            GameObject roomGameobject = Instantiate(room.roomTilemapPrefab, roomPosition, Quaternion.identity, transform);

            //Get instantiated room component from instantiated prefab
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            //Initialise the instantiated room
            instantiatedRoom.Initialise(roomGameobject);

            //Save gameobject reference
            room.instantiatedRoom = instantiatedRoom;
        }
    }

    /// <summary>
    /// Get A Room Template By Room TemplateID, Returns Null If ID Doesn't Exist
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get A Room By RoomID, Returns Null If ID Doesn't Exist
    /// </summary>
    public Room GetRoomByRoomID(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Clear The Dungeon Room Gameobjects And The Room Dictionary 
    /// </summary>
    private void ClearDungeon()
    {
        //Destroy instantiated dungeon gameobjects and clear dungeon manager room dictionary
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string,Room> keyValuePair in dungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;

                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }

            dungeonBuilderRoomDictionary.Clear();
        }
    }
}
