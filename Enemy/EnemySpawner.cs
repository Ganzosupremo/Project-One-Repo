using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonoBehaviour<EnemySpawner>
{
    private int enemiesToSpawn;
    private int currentEnemyCount;
    private int enemiesSpawnedSoFar;
    private int maxConcurrentNumberOfEnemies;

    private Room currentRoom;
    private RoomEnemySpawnParemeters roomEnemySpawnParemeters;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    /// <summary>
    /// Process a change in the room
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        //Play some music
        MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.5f, 0.8f);

        //If the room is any type of corridor of is an entrance, then return
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
            return;

        //If the room is already clear of enemies, then return
        if (currentRoom.isClearOfEnemies) return;

        //Get a random nnumber of enemies to spawn for this room
        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        //Get the enemy spawn parameters for this room
        roomEnemySpawnParemeters = currentRoom.GetEnemySpawnParemeters(GameManager.Instance.GetCurrentDungeonLevel());

        //If no enemies to spawn, return and mark the room as cleared
        if (enemiesToSpawn == 0)
        {
            currentRoom.isClearOfEnemies = true;
            return;
        }

        //Get the number of concurrent enemies to be spawn in this room
        maxConcurrentNumberOfEnemies = GetConcurrentEnemiesToSpawn();

        //Play the battle music
        MusicManager.Instance.PlayMusic(currentRoom.battleMusic, 0.5f, 1f);

        //Annnnd lock the doors
        currentRoom.instantiatedRoom.LockDoorsAfterEntry();

        SpawnEnemies();
    }

    /// <summary>
    /// This method spawns the enemies onto the current room
    /// </summary>
    private void SpawnEnemies()
    {
        if (GameManager.Instance.currentGameState == GameState.BossStage)
        {
            GameManager.Instance.previousGameState = GameState.BossStage;
            GameManager.Instance.currentGameState = GameState.killingBoss;
        }
        else if (GameManager.Instance.currentGameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.currentGameState = GameState.killingEnemies;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    /// <summary>
    /// Spawn the enemies coroutine
    /// </summary>
    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;

        //Create an instance of the helper class used to select a random enemy
        RandomSpawnableObject<EnemyDetailsSO> randomSpawnableObject = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        //See if we have space to spawn the enemies
        if (currentRoom.spawnPositionArray.Length > 0)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                //Wait untile the enemy count is less than the max concurrent enemies
                while (currentEnemyCount >= maxConcurrentNumberOfEnemies)
                {
                    yield return null;
                }

                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                //Creates the enemy and gets the next one to spawn
                CreateEnemy(randomSpawnableObject.GetItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    /// <summary>
    /// Gets a random interval spawn value
    /// </summary>
    private float GetEnemySpawnInterval()
    {
        return Random.Range(roomEnemySpawnParemeters.minSpawnInterval, roomEnemySpawnParemeters.maxSpawnInterval);
    }

    /// <summary>
    /// 
    /// </summary>
    private int GetConcurrentEnemiesToSpawn()
    {
        return Random.Range(roomEnemySpawnParemeters.minConcurrentEnemies, roomEnemySpawnParemeters.maxConcurrentEnemies);
    }

    /// <summary>
    /// Creates an enemy in the specific position
    /// </summary>
    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        //Keep track of the number of enemies already spawned
        enemiesSpawnedSoFar++;

        //Add one to the enemy count - this count is reduced when an enemy is killed
        currentEnemyCount++;

        //Get curren dungeon level
        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        //Instantiate the enemy
        GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);

        //Initialize the enemy parameters
        enemy.GetComponent<Enemy>().InitializeEnemyDetails(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);

        //Suscribe to the destroy enemies event
        enemy.GetComponent<DestroyEvent>().OnDestroy += Enemy_OnDestroyed;
    }

    /// <summary>
    /// Destroy enemy event handler
    /// </summary>
    private void Enemy_OnDestroyed(DestroyEvent destroyEvent, DestroyEventArgs destroyEventArgs)
    {
        //Unsuscribe
        destroyEvent.OnDestroy -= Enemy_OnDestroyed;

        //Reduce the enemy count
        currentEnemyCount--;

        //Call the points scored event, for scoring purposes
        StaticEventHandler.CallPointsScoredEvent(destroyEventArgs.points);

        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
        {
            currentRoom.isClearOfEnemies = true;

            //Set the state of the game
            if (GameManager.Instance.currentGameState == GameState.killingEnemies)
            {
                GameManager.Instance.currentGameState = GameState.playingLevel;
                GameManager.Instance.previousGameState = GameState.killingEnemies;
            }
            else if (GameManager.Instance.currentGameState == GameState.killingBoss)
            {
                GameManager.Instance.currentGameState = GameState.BossStage;
                GameManager.Instance.previousGameState = GameState.killingBoss;
            }

            //Unlock the doors
            currentRoom.instantiatedRoom.UnlockTheDoors(Settings.doorUnlockDelay);

            //Play the normal music again
            MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.5f, 2f);

            //Trigger the static event to indicate the room  is clear of enemies
            StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
        }
    }
}
