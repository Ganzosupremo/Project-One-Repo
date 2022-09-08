using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MoveableItem : MonoBehaviour
{
    #region Header Sound Effect
    [Header("Sound Effect")]
    #endregion

    #region Tooltip
    [Tooltip("The sound when this object is move around")]
    #endregion
    [SerializeField] private SoundEffectSO moveSound;

    private Rigidbody2D m_therb;
    private InstantiatedRoom m_instantiatedRoom;
    private Vector3 m_previosPosition;

    [HideInInspector] public BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        m_instantiatedRoom = GetComponentInParent<InstantiatedRoom>();
        m_therb = GetComponent<Rigidbody2D>();

        //Add this item to the item obstacles array
        m_instantiatedRoom.moveableItemsList.Add(this);
    }

    /// <summary>
    /// Update the obstacle position when something comes into contact with it
    /// </summary>
    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateObstacles();
    }

    /// <summary>
    /// Update the position of the obstacle
    /// </summary>
    private void UpdateObstacles()
    {
        //Make sure the obstacle remains within a room
        ConfineItemToRoom();

        //Update the moveable items in the obstacles array
        m_instantiatedRoom.UpdateMoveableObstacles();

        //Capture the new position, post collision
        m_previosPosition = transform.position;

        //Play the sound effect if the object is moving
        if (Mathf.Abs(m_therb.velocity.x) > 0.001f || Mathf.Abs(m_therb.velocity.y) > 0.001f)
        {
            //Play the sound effect every 10 frames
            if (moveSound != null && Time.frameCount % 10 == 0)
            {
                SoundManager.Instance.PlaySoundEffect(moveSound);
            }
        }
    }

    /// <summary>
    /// Confine the moveable object to the bounds of the room
    /// </summary>
    private void ConfineItemToRoom()
    {
        Bounds itemBounds = boxCollider2D.bounds;
        Bounds roomBounds = m_instantiatedRoom.roomColliderBounds;

        //If the object is being moved beyond the room bounds, set this object's position to the previous position
        if (itemBounds.min.x <= roomBounds.min.x ||
            itemBounds.max.x >= roomBounds.max.x ||
            itemBounds.min.x <= roomBounds.min.x ||
            itemBounds.max.y >= roomBounds.max.y)
        {
            transform.position = m_previosPosition;
        }
    }
}
