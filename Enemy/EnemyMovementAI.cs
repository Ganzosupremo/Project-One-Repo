using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("The movement details SO containing information about the movement, such as speed etc.")]
    #endregion
    public MovementDetailsSO movementDetails;

    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate fixedUpdateWait;
    private bool shouldChasePlayer = false;
    private List<Vector2Int> surroundingPositionsList = new List<Vector2Int>();

    //public float stopMovingAfterhitCooldown = 0f;
    [HideInInspector] public int updateFramesNumber = 1;
    [HideInInspector] public float enemySpeed;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        enemySpeed = movementDetails.GetMoveSpeed();    
    }

    private void Start()
    {
        //Create the waitforfixedupdate to use in the coroutine
        fixedUpdateWait = new WaitForFixedUpdate();

        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }

    /// <summary>
    /// Moves the enemy, uses the AStar Pathfinding to build a path to the player 
    /// </summary>
    private void MoveEnemy()
    {
        //Movement cooldown timer
        currentEnemyPathRebuildCooldown -= Time.deltaTime;
        
        //Check the distance to the player to determine if the enemy should start chasing after him
        if (!shouldChasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyDetails.enemyChaseDistance)
        {
            shouldChasePlayer = true;
        }
        if (!shouldChasePlayer)
            return;

        //Only process A Star rebuild on certain frames to not overload the CPU usage
        if (Time.frameCount % Settings.targetFramesToSpreadPathfindingOver != updateFramesNumber) return;

        //If the cooldown timer is zero or the player has move more than the required distance, then
        //rebuild the path and move the enemy
        if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition())
            > Settings.playerMoveDistanceToRebuildPath))
        {
            //Reset the path rebuild timer
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            //Reset the player referenced position
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            //Move the enemy using AStar - Triggers the rebuilding of the path
            CreatePath();

            //If a path has been built, move the enemy
            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                //Move the enemy along the path using a coroutine
                moveEnemyRoutine = StartCoroutine(MoveEnemyCoroutine(movementSteps));
            }
        }
    }

    /// <summary>
    /// Coroutine to move the enemy to the next point on the path
    /// </summary>
    private IEnumerator MoveEnemyCoroutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPos = movementSteps.Pop();

            //While not very close continue moving, too close move to the next point
            while (Vector3.Distance(nextPos, transform.position) > 0.2f)
            {
                //Call the movement event
                enemy.movementByPositionEvent.CallMovementByPositionEvent(nextPos, transform.position, enemySpeed, (nextPos - transform.position).normalized);

                yield return fixedUpdateWait;
            }

            yield return fixedUpdateWait;
        }

        // End of path steps - trigger the enemy idle event
        enemy.idleEvent.CallIdleEvent();
    }

    /// <summary>
    /// Use the AStar static class to build a path for the enemy
    /// </summary>
    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        //Gets the player position on the grid
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);
        
        //Gets the enemy position on the grid
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        //Build a path fot the enemy to move
        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        //Take off the first step on path - this is the square the enemy is already on
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else //If theres no path, go idle
        {
            enemy.idleEvent.CallIdleEvent();
        }
    }

    /// <summary>
    /// Set the frame on which the enemy pathfinding will be recalculated - to avoid spikes in the CPU usage
    /// </summary>
    public void UpdateFramesNumber(int updatedFrameNumber)
    {
        this.updateFramesNumber = updatedFrameNumber;
    }

    /// <summary>
    /// Get the nearest player position that isn't on an obstacle
    /// </summary>
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.tilemapLowerBounds.x, 
            playerCellPosition.y - currentRoom.tilemapLowerBounds.y);

        int obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y],
            currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y]);

        //if the player is not on an obstacle, then return the player position
        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        //find the nearest cell that is not an obstacle, whether thats a collision tile or a moveable object
        else
        {
            surroundingPositionsList.Clear();

            //Populate the surrounding position list - this will hold the 8 posible vector locations of a (0,0) grid square
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    surroundingPositionsList.Add(new Vector2Int(i, j));
                }
            }

            //Loop through all position
            for (int l = 0; l < 8; l++)
            {
                //Generate an index for the list
                int index = Random.Range(0, surroundingPositionsList.Count);

                //See of there is an obstacle in the selected surrounded position
                try
                {
                    obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + surroundingPositionsList[index].x,
                        adjustedPlayerCellPosition.y + surroundingPositionsList[index].y], currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPosition.x
                        + surroundingPositionsList[index].x, adjustedPlayerCellPosition.y + surroundingPositionsList[index].y]);

                    if (obstacle != 0)
                    {
                        return new Vector3Int(playerCellPosition.x + surroundingPositionsList[index].x,
                            playerCellPosition.y + surroundingPositionsList[index].y, 0);
                    }
                }
                //Catch errors where the surrounded position is outside the grid
                catch
                {

                }

                surroundingPositionsList.RemoveAt(index);
            }

            //No non-obstacle tiles surronding the player - send the enemy in the direction of an enemy spawn position transform
            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
    #endregion
}
