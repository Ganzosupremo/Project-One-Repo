using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapon System/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header Base Ammo Details
    [Space(10)]
    [Header("Base Ammo Details")]
    #endregion

    #region Tooltip
    [Tooltip("The name the ammo will have")]
    #endregion
    public string ammoName;
    public bool isPlayerAmmo;

    #region Header Sprite, Prefab and Materials
    [Space(10)]
    [Header("Ammo Sprite, Prefab and Materials")]
    #endregion

    #region Tooltip
    [Tooltip("The sprite for the ammo")]
    #endregion
    public Sprite ammoSprite;

    #region Tooltip
    [Tooltip("This can be populated with the prefab that will be used for the ammo, multiple prefabs can be used and " +
        "they will be randomly selected. The prefab can be an ammo pattern as long it is compatible with the IFireable interface.")]
    #endregion
    public GameObject[] ammoPrefabArray;

    #region Tooltip
    [Tooltip("The material used for the ammo")]
    #endregion
    public Material ammoMaterial;

    #region Tooltip
    [Tooltip("The ammo charge time - If the ammo should charge briefly before it can move. For example when creating an ammo pattern, the ammo should charge first before moving")]
    #endregion
    public float ammoChargeTime = 0.1f;

    #region Tooltip
    [Tooltip("If the ammo has a charge time, then specify the material that should be used to render the ammo while it's charging")]
    #endregion
    public Material ammoChargeMaterial;

    #region Ammo Hit Effect
    [Space(10)]
    [Header("Ammo Hit Effect")]
    #endregion
    #region Tooltip
    [Tooltip("The SO that defines the parameters for the ammo hit effect prefab")]
    #endregion
    public AmmoHitEffectSO ammoHitEffect;

    #region Header Base Ammo Parameters
    [Space(10)]
    [Header("Base Ammo Parameters")]
    #endregion

    #region Tooltip
    [Tooltip("The damage for each individual ammo")]
    #endregion
    public int ammoDamage = 1;

    #region Tooltip
    [Tooltip("The min speed for each ammo - the speed will be a random value between the min and max speed")]
    #endregion
    public float minAmmoSpeed = 20f;

    #region Tooltip
    [Tooltip("The max speed for each ammo - the speed will be a random value between the min and max speed")]
    #endregion
    public float maxAmmoSpeed = 20f;

    #region Tooltip
    [Tooltip("The range of the ammo, or ammo patterns, in unity units")]
    #endregion
    public float ammoRange = 20f;
    
    #region Tooltip
    [Tooltip("The rotation speed for the ammo, it only works with ammo patterns - in degrees per second")]
    #endregion
    public float ammoRotationSpeed = 1f;

    #region Header Ammo Spread Details
    [Space(10)]
    [Header("Base Ammo Spreed Details")]
    #endregion

    #region Tooltip
    [Tooltip("This is the min spread angle for the ammo. A higher spread means less accuracy. A random value is calculated between the min and the max spread values")]
    #endregion
    public float ammoSpreadMin = 0f;
    
    #region Tooltip
    [Tooltip("This is the max spread angle for the ammo. A higher spread means less accuracy. A random value is calculated between the min and the max spread values")]
    #endregion
    public float ammoSpreadMax = 1f;

    #region Header Ammo Spawn Details
    [Space(10)]
    [Header("Base Ammo Spawn Details")]
    #endregion

    #region Tooltip
    [Tooltip("This is the min number of bullets that will spawn per shoot. A random value is calculated between the min and the max values")]
    #endregion
    public int ammoSpawnAmountMin = 1;

    #region Tooltip
    [Tooltip("This is the max number of bullets that will spawn per shoot. A random value is calculated between the min and the max values")]
    #endregion
    public int ammoSpawnAmountMax = 1;

    #region Tooltip
    [Tooltip("The ammo spawn interval between them so they don't appear all at once. It is randomly calculated with the min and max values")]
    #endregion
    public float minSpawnInterval = 0f;

    #region Tooltip
    [Tooltip("The ammo spawn interval between them so they don't appear all at once. It is randomly calculated with the min and max values")]
    #endregion
    public float maxSpawnInterval = 0f;

    #region Header Ammo Trail Details
    [Space(10)]
    [Header("Base Ammo Trail Details")]
    #endregion

    #region Tooltip
    [Tooltip("Select if the ammo should have a trail, if not deselect")]
    #endregion
    public bool hasAmmoTrail = false;

    #region Tooltip
    [Tooltip("The ammo trail lifetime in seconds")]
    #endregion
    public float ammoTrailLifetime = 3f;

    #region Tooltip
    [Tooltip("The ammo trail material")]
    #endregion
    public Material ammoTrailMaterial;

    #region Tooltip
    [Tooltip("The starting width for the trail")]
    #endregion
    [Range(0f, 2f)] public float ammoTrailStartWidth;

    #region Tooltip
    [Tooltip("The ending width for the trail")]
    #endregion
    [Range(0f, 2f)] public float ammoTrailEndWidth;

    #region Validate
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        
        if (ammoChargeTime > 0)
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(minAmmoSpeed), minAmmoSpeed, nameof(maxAmmoSpeed), maxAmmoSpeed, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin, nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(minSpawnInterval), minSpawnInterval, nameof(maxSpawnInterval), maxSpawnInterval, true);
        }

        if (hasAmmoTrail)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailLifetime), ammoTrailLifetime, false);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }
    }
#endif
    #endregion
}
