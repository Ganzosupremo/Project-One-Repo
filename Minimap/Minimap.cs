using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    [SerializeField] private GameObject minimapPlayer;

    private Transform playerPosition;

    private void Start()
    {
        playerPosition = GameManager.Instance.GetPlayer().transform;

        //Use the player icon for the cinemachine target
        CinemachineVirtualCamera virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        virtualCamera.Follow = playerPosition;

        //Set the player minimap icon
        SpriteRenderer spriteRenderer = minimapPlayer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMinimapIcon();
        }
    }

    private void Update()
    {
        //Make the minimap icon follow the player
        if (playerPosition != null && minimapPlayer != null)
        {
            minimapPlayer.transform.position = playerPosition.position;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapPlayer), minimapPlayer);
    }
#endif
    #endregion
}
