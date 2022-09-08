using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/Enemy Details")]
public class EnemyDetailsSO : ScriptableObject
{
    #region Header Base Enemy Details
    [Space(10)]
    [Header("Base Enemy Details")]
    #endregion

    #region Tooltip
    [Tooltip("The name for the enemy")]
    #endregion
    public string enemyName;

    #region Tooltip
    [Tooltip("The prefab for the enemy")]
    #endregion
    public GameObject enemyPrefab;

    #region Tooltip
    [Tooltip("Distance from which the enemy will chase the player")]
    #endregion
    public float enemyChaseDistance = 50f;

    #region Tooltip
    [Tooltip("The Particle System that plays when the enemy dies")]
    #endregion
    public EnemyDeathEffectSO enemyDeathEffect;

    #region Tooltip
    [Tooltip("The type of money this enemy will drop")]
    #endregion
    public MoneyDetailsSO moneyDetails;

    #region Header Enemy Materials
    [Space(10)]
    [Header("Materials For The Enemy")]
    #endregion
    
    #region Tooltip
    [Tooltip("This is the standard lit material")]
    #endregion
    public Material standardEnemyMaterial;

    #region Header Enemy Materialize Settings
    [Space(10)]
    [Header("Settings For Enemy Materialize")]
    #endregion

    #region Tooltip
    [Tooltip("The it will take for the enemy to materialize")]
    #endregion
    public float enemyMaterializeTime;

    #region Tooltip
    [Tooltip("The shader that'll be used when the enemy materializes")]
    #endregion
    public Shader enemyMaterializeShader;

    #region Tooltip
    [Tooltip("The color of the materialize effect - it's and HDR, so the intensity can make the color glow")]
    #endregion
    [ColorUsage(true, true)]
    public Color enemyMaterializeColor;
    //#region Tooltip
    //[Tooltip("The color of the materialize effect when the enemy dies - it's and HDR, so the intensity can make the color glow")]
    //#endregion
    //[ColorUsage(true, true)]
    //public Color enemyDeathMaterializeColor;

    #region Header Enemy's Weapon Settings
    [Space(10)]
    [Header("Enemy Weapon Settings")]
    #endregion
    #region Tooltip
    [Tooltip("The weapon for the enemy - leave blank if the enemy won't have a weapon")]
    #endregion
    public WeaponDetailsSO enemyWeapon;

    #region Tooltip
    [Tooltip("The minimum delay in seconds btw shoots for the enemy - the end value will be a random value btw min and max")]
    #endregion
    public float firingMinDelay = 0.1f;
    #region Tooltip
    [Tooltip("The maximum delay in seconds btw shoots for the enemy - the end value will be a random value btw min and max")]
    #endregion
    public float firingMaxDelay = 1f;

    #region Tooltip
    [Tooltip("The minimum duration in seconds that the enemy is going to fire for - the end value will be a random value btw min and max")]
    #endregion
    public float firingMinDuration = 1f;
    #region Tooltip
    [Tooltip("The maximum duration in seconds that the enemy is going to fire for - the end value will be a random value btw min and max")]
    #endregion
    public float firingMaxDuration = 2f;

    #region Tooltip
    [Tooltip("Check this if the enemy will need a sight of the player before shooting - if it's not selected the enemy" +
        " will shoot regardless of obstacles, but whenever the player is in range")]
    #endregion
    public bool lineOfSightRequired;

    #region Header Enemy Health
    [Space(10)]
    [Header("Enemy Health Parameters")]
    #endregion
    
    #region Tooltip
    [Tooltip("The Enemy Health for each individual level")]
    #endregion
    public EnemyHealthDetails[] enemyHealthDetailsArray;

    #region Tooltip
    [Tooltip("Select if the enemy will have immunity after hit - if so, select the immunity time under this field")]
    #endregion
    public bool immuneAfterHit = false;

    #region Tooltip
    [Tooltip("Immunity time in seconds after hit")]
    #endregion
    public float immunityTime;

    #region Tooltip
    [Tooltip("The time the enemy will stop moving after hit")]
    #endregion
    public float startDazeTime;

    #region Tooltip
    [Tooltip("Select if the enemy will have a health bar displayed")]
    #endregion
    public bool isHealthBarDisplayed = false;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(standardEnemyMaterial), standardEnemyMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(moneyDetails), moneyDetails);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyChaseDistance), enemyChaseDistance, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);

        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingMinDelay), firingMinDelay, 
            nameof(firingMaxDelay), firingMaxDelay, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingMinDuration), firingMinDuration,
            nameof(firingMaxDuration), firingMaxDuration, false);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
        if (immuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(immunityTime), immunityTime, false);
        }
    }
#endif
    #endregion
}
