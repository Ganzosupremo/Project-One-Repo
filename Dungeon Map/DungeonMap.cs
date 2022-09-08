using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DungeonMap : SingletonMonoBehaviour<DungeonMap>
{
    #region Header GameObject References
    [Space(10)]
    [Header("References")]
    #endregion

    [SerializeField] private GameObject minimapUI;

    private Camera dungeonMapCamera;
    private Camera mainCamera;
    private PlayerInput inputActions;
    private float waitBeforeTeleporting = 0.7f;
    private float counter = 0.7f;

    private void Start()
    {
        mainCamera = Camera.main;

        counter = 0.7f;

        Transform playerTransform = GameManager.Instance.GetPlayer().transform;

        //Populate the cinemachine camera target with the player
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        dungeonMapCamera = GetComponentInChildren<Camera>();
        dungeonMapCamera.gameObject.SetActive(false);

        inputActions = new();
        inputActions.OverviewMap.Enable();
    }

    private void Update()
    {
        waitBeforeTeleporting -= Time.deltaTime;

        bool leftButtonPressed = inputActions.OverviewMap.Click.IsPressed();

        //Get the room that was clicked
        if (leftButtonPressed && GameManager.Instance.currentGameState == GameState.dungeonOverviewMap && waitBeforeTeleporting <= 0)
        {
            GetRoomClicked();
        }
    }

    /// <summary>
    /// Get the room that was clicked on the overview dungeon map
    /// </summary>
    private void GetRoomClicked()
    {
        Vector3 worldPosition = dungeonMapCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

        //Check for collisions at the mouse position
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 1f);

        foreach (Collider2D collider2D in collider2Ds)
        {
            if (collider2D.GetComponent<InstantiatedRoom>() != null)
            {
                InstantiatedRoom instantiatedRoom = collider2D.GetComponent<InstantiatedRoom>();

                //If the room has been cleared of enemies an has been previously visited, the player can be teleported there
                if (instantiatedRoom.room.isClearOfEnemies && instantiatedRoom.room.isPreviouslyVisited)
                {
                    StartCoroutine(TeleportPlayerToRoom(worldPosition, instantiatedRoom.room));
                }
            }
        }

        waitBeforeTeleporting = counter;
    }

    /// <summary>
    /// Teleport the player to the clicked room
    /// </summary>
    private IEnumerator TeleportPlayerToRoom(Vector3 worldPosition, Room room)
    {
        StaticEventHandler.CallRoomChangedEvent(room);

        //Fade the screen to black
        yield return StartCoroutine(GameManager.Instance.Fade(0f, 1f, 0.1f, Color.black));

        ClearDungeonOverviewMap();

        //Disable the player during the fade
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        //Get the nearest spawn point to the player that is in the room
        Vector3 nearestSpawnPoint = HelperUtilities.GetSpawnPointNearestToPlayer(worldPosition);

        //Teleport the player to that new position
        GameManager.Instance.GetPlayer().transform.position = nearestSpawnPoint;

        //Return the screen to normal
        yield return StartCoroutine(GameManager.Instance.Fade(1f, 0f, 1f, Color.black));

        //Enable the player again
        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();
    }

    /// <summary>
    /// Displays the map on all the screen
    /// </summary>
    public void DisplayDungeonOverviewMap()
    {
        //Set the game states
        GameManager.Instance.previousGameState = GameManager.Instance.currentGameState;
        GameManager.Instance.currentGameState = GameState.dungeonOverviewMap;

        //Disable the player
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        //Disable the main camera and display the overview map
        mainCamera.gameObject.SetActive(false);
        dungeonMapCamera.gameObject.SetActive(true);

        //Ensure all room are active
        ActivateDungeonRoomsForDisplay();

        //Disable the small minimap UI component
        minimapUI.SetActive(false);
    }

    public void ClearDungeonOverviewMap()
    {
        //Restore the game states
        GameManager.Instance.currentGameState = GameManager.Instance.previousGameState;
        GameManager.Instance.previousGameState = GameState.dungeonOverviewMap;

        //Renable the player
        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();

        //Renable the main camera and disable the overview map
        mainCamera.gameObject.SetActive(true);
        dungeonMapCamera.gameObject.SetActive(false);

        minimapUI.SetActive(true);
    }

    private void ActivateDungeonRoomsForDisplay()
    {
        foreach (KeyValuePair<string,Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            room.instantiatedRoom.gameObject.SetActive(true);
        }
    }
}
