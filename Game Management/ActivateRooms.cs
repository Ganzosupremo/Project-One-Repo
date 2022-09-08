using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class ActivateRooms : MonoBehaviour
{
    #region Header Put the Minimap Camera Here
    [Header("The Minimap Camera")]
    #endregion
    [SerializeField] private Camera minimapCamera;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        InvokeRepeating(nameof(EnableRooms), 0.5f, 0.75f);
    }

    private void EnableRooms()
    {
        //If the current game state is dungeonOverviewMap, don't process any room activation so return
        if (GameManager.Instance.currentGameState == GameState.dungeonOverviewMap) return;

        HelperUtilities.CameraWorldPositionBounds(out Vector2Int minimapWorldPositionLowerBounds,
            out Vector2Int minimapWorldPositionUpperBounds, minimapCamera);

        HelperUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLowerBounds,
            out Vector2Int mainCameraWorldPositionUpperBounds, mainCamera);

        //Iterate through all the dungeon rooms to see if there are in the viewport of the camera
        foreach (KeyValuePair<string,Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            //If the room is within the viewport bounds, then activate the room
            if ((room.worldLowerBounds.x <= minimapWorldPositionUpperBounds.x && room.worldLowerBounds.y <= minimapWorldPositionUpperBounds.y) &&
                (room.worldUpperBounds.x >= minimapWorldPositionLowerBounds.x && room.worldUpperBounds.y >= minimapWorldPositionLowerBounds.y))
            {
                room.instantiatedRoom.gameObject.SetActive(true);

                //If the room is within the main camera viewport activate the environment objects
                if ((room.worldLowerBounds.x <= mainCameraWorldPositionUpperBounds.x && room.worldLowerBounds.y <= mainCameraWorldPositionUpperBounds.y) &&
                (room.worldUpperBounds.x >= mainCameraWorldPositionLowerBounds.x && room.worldUpperBounds.y >= mainCameraWorldPositionLowerBounds.y))
                {
                    room.instantiatedRoom.ActivateEnvironmentObjects();
                }
                else
                {
                    room.instantiatedRoom.DeactivateEnvironmentObjects();
                }
            }
            else
            {
                room.instantiatedRoom.gameObject.SetActive(false);
            }
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapCamera), minimapCamera);
    }
#endif
    #endregion
}
