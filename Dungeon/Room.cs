using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string id;
    public string templateID;
    public GameObject roomTilemapPrefab;
    public RoomNodeTypeSO roomNodeType;

    public MusicTrackSO ambientMusic;
    public MusicTrackSO battleMusic;
    
    public Vector2Int worldLowerBounds;
    public Vector2Int worldUpperBounds;
    public Vector2Int tilemapLowerBounds;
    public Vector2Int tilemapUpperBounds;
    public Vector2Int[] spawnPositionArray;

    public List<SpawnableObjectByLevel<EnemyDetailsSO>> enemiesByLevelList;
    public List<RoomEnemySpawnParemeters> roomLevelEnemySpawnParametersList;

    public List<string> childRoomIDList;
    public string parentRoomID;
    public List<Doorway> doorwayList;
    public bool isPositioned = false;
    public InstantiatedRoom instantiatedRoom;
    public bool isLit;
    public bool isClearOfEnemies = false;
    public bool isPreviouslyVisited = false;

    public Room()
    {
        childRoomIDList = new List<string>();
        doorwayList = new List<Doorway>();
    }

    /// <summary>
    /// Randomly gets the number of enemies that will spawn in this room for a given level
    /// </summary>
    /// <returns></returns>
    public int GetNumberOfEnemiesToSpawn(DungeonLevelSO dungeonLevel)
    {
        foreach (RoomEnemySpawnParemeters enemySpawnParemeters in roomLevelEnemySpawnParametersList)
        {
            if (enemySpawnParemeters.dungeonLevel == dungeonLevel)
            {
                return Random.Range(enemySpawnParemeters.minTotalEnemiesToSpawn, enemySpawnParemeters.maxTotalEnemiesToSpawn);
            }         
        }

        return 0;
    }

    /// <summary>
    /// Gets the enemy spawn parameters for this room for a given level - returns null if none is found
    /// </summary>
    public RoomEnemySpawnParemeters GetEnemySpawnParemeters(DungeonLevelSO dungeonLevel)
    {
        foreach (RoomEnemySpawnParemeters enemySpawnParemeters in roomLevelEnemySpawnParametersList)
        {
            if (enemySpawnParemeters.dungeonLevel == dungeonLevel)
            {
                return enemySpawnParemeters;
            }
        }

        return null;
    }
}
