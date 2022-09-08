using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
#region Tooltip
    [Tooltip("Populate this with the Sprite Renderer on the child weapon gameobject")]
    #endregion
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    #region Tooltip
    [Tooltip("Populate this with the Polygon Collider on the child weapon gameobject")]
    #endregion
    [SerializeField] private PolygonCollider2D weaponPolygonCollider;

    #region Tooltip
    [Tooltip("Populate this with the Transform on the WeaponFirePosition gameobject")]
    #endregion
    [SerializeField] private Transform weaponShootPosition;

    #region Tooltip
    [Tooltip("Populate this with the Transform on the WeaponFirePosition gameobject")]
    #endregion
    [SerializeField] private Transform weaponEffectPosition;

    private ActiveWeaponEvent activeWeaponEvent;
    private Weapon currentWeapon;

    private void Awake()
    {
        activeWeaponEvent = GetComponent<ActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        //Suscribe to the active weapon event
        activeWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        ///Unsuscribe to the active weapon event
        activeWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    /// <summary>
    /// Active Weapon Event Handler
    /// </summary>
    private void SetActiveWeaponEvent_OnSetActiveWeapon(ActiveWeaponEvent activeWeaponEvent, ActiveWeaponEventArgs activeWeaponEventArgs)
    {
        SetWeapon(activeWeaponEventArgs.playerWeapon);
    }

    private void SetWeapon(Weapon playerWeapon)
    {
        currentWeapon = playerWeapon;
        
        //Set the current weapon sprite
        weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;

        //Set the weapon polygon collider based on the shape the weapon sprite has
        if (weaponPolygonCollider != null && weaponSpriteRenderer.sprite != null)
        {
            //Get the sprite physics shape - this returns the sprite physics shape as a list of vectors
            List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            //Set the polygon collider points based on the sprite physics shape points
            weaponPolygonCollider.points = spritePhysicsShapePointsList.ToArray();
        }
        //Set the weapon fire position
        weaponShootPosition.localPosition = currentWeapon.weaponDetails.weaponFirePosition;
    }

    /// <summary>
    /// Gets The Ammo Corresponding To The Specific Weapon
    /// </summary>
    public AmmoDetailsSO GetCurrentAmmo()
    {
        return currentWeapon.weaponDetails.weaponCurrentAmmo;
    }


    /// <summary>
    /// Returns The Current Weapon
    /// </summary>
    /// <returns></returns>
    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
    /// <summary>
    /// Returns The Weapon Fire Position
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFirePosition()
    {
        return weaponShootPosition.position;
    }

    /// <summary>
    /// Returns The Fire Effect Of The Weapon
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWeaponFireEffectPosition()
    {
        return weaponEffectPosition.position;
    }

    /// <summary>
    /// Removes The Current Weapon
    /// </summary>
    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider), weaponPolygonCollider);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPosition), weaponShootPosition);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPosition), weaponEffectPosition);
    }
#endif
    #endregion
}
