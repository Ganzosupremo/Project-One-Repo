using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    #region Header Player Base Details
    [Space(10)]
    [Header("The Basic Details Of The Player")]
    #endregion

    #region Tooltip
    [Tooltip("Player Character Name")]
    #endregion
    public string playerCharacterName;

    #region Tooltip
    [Tooltip("Player Character Prefab")]
    #endregion
    public GameObject playerPrefab;

    #region Tooltip
    [Tooltip("The Runtime Animator Controller For The Player")]
    #endregion
    public RuntimeAnimatorController runtimeAnimatorController;

    #region Header Health System
    [Space(10)]
    [Header("Health System")]
    #endregion

    #region Tooltip
    [Tooltip("Player Starting Health Amount")]
    #endregion
    public int playerHealthAmount;

    #region Tooltip
    [Tooltip("Select if the player will be invencible after hit - select the time in the next field")]
    #endregion
    public bool isImmuneAfterHit = false;

    #region Tooltip
    [Tooltip("Choose how many seconds the player will be invencible for")]
    #endregion
    public float immunityTime;

    #region Header Weapon
    [Space(10)]
    [Header("The Player Weapon")]
    #endregion

    #region Tooltip
    [Tooltip("The initial weapon the player will have")]
    #endregion
    public WeaponDetailsSO initialWeapon;

    #region Tooltip
    [Tooltip("The initial weapon list for different characters, this sets the initialWeapon variable above")]
    #endregion
    public List<WeaponDetailsSO> initialWeaponList;

    #region Header Other
    [Space(10)]
    [Header("Other Things")]
    #endregion

    #region Tooltip
    [Tooltip("Player icon sprite to be used in the minimap")]
    #endregion
    public Sprite playerMinimapIcon;

    #region Tooltip
    [Tooltip("Player hand sprite")]
    #endregion
    public Sprite playerHandSprite;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerMinimapIcon), playerMinimapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);

        HelperUtilities.ValidateCheckNullValue(this, nameof(initialWeapon), initialWeapon);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(initialWeaponList), initialWeaponList);

        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(immunityTime), immunityTime, false);
        }
    }
#endif
    #endregion
}
