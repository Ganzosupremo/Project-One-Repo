using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSelectionUI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with the player hand that is found in the WeaponAnchorPosition")]
    #endregion
    public SpriteRenderer playerHandSpriteRenderer;

    #region Tooltip
    [Tooltip("Populate with the no weapon hand")]
    #endregion
    public SpriteRenderer playerHandNoWeapon;

    #region Tooltip
    [Tooltip("Populate with the player weapon that can be found in the WeaponAnchorPosition")]
    #endregion
    public SpriteRenderer weaponSpriteRenderer;

    public Animator animator;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandNoWeapon), playerHandNoWeapon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSpriteRenderer), playerHandSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(animator), animator);
    }
#endif
    #endregion
}
