using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapon System/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    #region Header Base Weapon Details
    [Space(10)]
    [Header("Base Weapon Details")]
    #endregion

    public string weaponName;
    
    #region Tooltip
    [Tooltip("The sprite for the weapon - the sprite should have the option 'generate physics shape' selected")]
    #endregion
    public Sprite weaponSprite;

    #region Header Weapon Configuration
    [Space(10)]
    [Header("Weapon Configuration Section")]
    #endregion

    #region Tooltip
    [Tooltip("The offset position for the end of the weapon from the sprite pivot point")]
    #endregion
    public Vector3 weaponFirePosition;

    #region Tooltip
    [Tooltip("The current ammo for the weapon")]
    #endregion
    public AmmoDetailsSO weaponCurrentAmmo;

    #region Tooltip
    [Tooltip("The current shoot effect for the weapon")]
    #endregion
    public WeaponShootEffectSO shootEffect;

    #region Tooltip
    [Tooltip("The firing sound effect for the weapon")]
    #endregion
    public SoundEffectSO weaponFiringSoundEffect;

    #region Tooltip
    [Tooltip("The reload sound effect for the weapon")]
    #endregion
    public SoundEffectSO weaponReloadSoundEffect;

    #region Header Weapon Operating Values
    [Space(10)]
    [Header("Weapon Operating Values")]
    #endregion
    #region Tooltip
    [Tooltip("Select if the weapon will have infinite ammo - leave it selected")]
    #endregion 
    public bool hasInfiniteAmmo = false;

    #region Tooltip
    [Tooltip("Select if the weapon will have infinite magazine capacity")]
    #endregion 
    public bool hasInfinityMagCapacity = false;

    #region Tooltip
    [Tooltip("The weapon magazine capacity - how many shoots before needing to reload the weapon")]
    #endregion 
    public int weaponMagMaxCapacity = 7;

    #region Tooltip
    [Tooltip("The max overral ammo that can be held at any given time for this specific weapon")]
    #endregion 
    public int weaponTotalAmmoCapacity = 200;

    #region Tooltip
    [Tooltip("The fire rate for this weapon - 0.2 means 5 shoots per second")]
    #endregion 
    public float weaponFireRate = 0.2f;

    #region Tooltip
    [Tooltip("The time the weapons needs before it can fire a bullet - a minigun needs to precharge before shooting")]
    #endregion 
    public float weaponPrechargeTime = 0f;

    #region Tooltip
    [Tooltip("The time the weapons takes to reload a magazine")]
    #endregion 
    public float weaponReloadTime = 0f;


    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);
        
        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponTotalAmmoCapacity), (float)weaponTotalAmmoCapacity, false);
        }

        if (!hasInfinityMagCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponMagMaxCapacity), (float)weaponMagMaxCapacity, false);
        }
    }
#endif
    #endregion
}
