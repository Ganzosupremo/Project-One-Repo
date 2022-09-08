using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header Object References
    [Space(10)]
    [Header("Object References")]
    #endregion


    #region Tooltip
    [Tooltip("Populate With The BoxCollider2D In The DoorCollider GameObject")]
    #endregion
    
    [SerializeField] private BoxCollider2D doorCollider;

    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator doorAnimator;
    private int playRandomSound;

    private void Awake()
    {
        //Disable the collider by default
        doorCollider.enabled = false;

        doorAnimator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Settings.playerTag) || other.CompareTag(Settings.playerWeapon))
        {
            playRandomSound = Random.Range(0, GameResources.Instance.openCloseDoorSoundEffect.Length);
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        //When the player gets far enough from a room, the room will be disabled and all the things in it too, this means
        //the animator state for the door gets reset, therefore we need to restore the animator state
        doorAnimator.SetBool(Settings.openDoor, isOpen);
    }

    /// <summary>
    /// Opens The Door When The Player Triggers The Door Collider
    /// </summary>
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            //Set the 'open' parameter in the animator
            doorAnimator.SetBool(Settings.openDoor, true);

            //Play one random sound effect from the array
            SoundManager.Instance.PlaySoundEffect(GameResources.Instance.openCloseDoorSoundEffect[playRandomSound]);
        }
    }

    /// <summary>
    /// Locks The Doors When The Player Enters The Room
    /// </summary>
    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        //Set the open parameter to false to lock the door
        doorAnimator.SetBool(Settings.openDoor, false);
    }

    /// <summary>
    /// Unlocks The Door
    /// </summary>
    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (previouslyOpened == true)
        {
            isOpen = false;
            OpenDoor();
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion
}
