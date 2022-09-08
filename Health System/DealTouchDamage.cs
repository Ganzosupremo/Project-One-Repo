using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DealTouchDamage : MonoBehaviour
{
    #region Header DEAL EMOTIONAL DAMAGE
    [Header("DEAL EMOTIONAL DAMAGE!!!")]
    #endregion

    #region Tooltip
    [Tooltip("the damage to deal (can be overridden by the receiver)")]
    #endregion
    [SerializeField] private int touchDamageAmount;

    #region Tooltip
    [Tooltip("Specify which layers should be affected by the touch damage")]
    #endregion
    [SerializeField] private LayerMask layerMask;
    private bool isColliding = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isColliding) return;

        TouchDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isColliding) return;

        TouchDamage(collision);
    }

    private void TouchDamage(Collider2D collision)
    {
        //If the object isn't in the specified layer then return (we're using bitwise comparison)
        int collisionLayerMask = (1 << collision.gameObject.layer);

        if ((layerMask.value & collisionLayerMask) == 0) return;

        //Check to see if the object should take touch damage
        ReceiveTouchDamage receiveTouchDamage = collision.gameObject.GetComponent<ReceiveTouchDamage>();

        if (receiveTouchDamage != null)
        {
            isColliding = true;

            //Reset the touch collision after the time specified
            Invoke(nameof(ResetTouchCollider), Settings.cooldownBtwTouchDamage);

            receiveTouchDamage.TakeTouchDamage(touchDamageAmount);
        }
    }

    /// <summary>
    /// Just reset the bool isColliding
    /// </summary>
    private void ResetTouchCollider()
    {
        isColliding = false;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(touchDamageAmount), touchDamageAmount, true);
    }
#endif
    #endregion
}
