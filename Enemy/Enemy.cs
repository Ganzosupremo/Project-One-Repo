using System.Collections;
using UnityEngine.Rendering;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Windows.Forms.DataVisualization.Charting;

#region Required Components
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(DestroyEvent))]
[RequireComponent(typeof(Destroy))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(MovementByPositionEvent))]
[RequireComponent(typeof(MovementByPosition))]
[RequireComponent(typeof(ActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FiringWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(DealTouchDamage))]
#endregion
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FiringWeaponEvent firingWeaponEvent;
    [HideInInspector] public FireWeapon fireWeapon;

    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    [HideInInspector] public MovementByPositionEvent movementByPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public EnemyMovementAI enemyMovementAI;

    private MaterializeEffect materializeEffect;
    private CircleCollider2D circleCollider;
    private PolygonCollider2D polygonCollider;


    private ActiveWeaponEvent activeWeaponEvent;
    private HealthEvent healthEvent;
    private Health health;

    private void Awake()
    {
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        movementByPositionEvent = GetComponent<MovementByPositionEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        firingWeaponEvent = GetComponent<FiringWeaponEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        activeWeaponEvent = GetComponent<ActiveWeaponEvent>();
        idleEvent = GetComponent<IdleEvent>();

        materializeEffect = GetComponent<MaterializeEffect>();
        circleCollider = GetComponent<CircleCollider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    /// <summary>
    /// Event handler for the health event when the enemy loses health
    /// </summary>
    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0)
        {
            SetEnemyDeathEffect();

            DropMoney();

            EnemyDestroyed();
        }
    }

    /// <summary>
    /// This method is called when the enemy's health is 0
    /// </summary>
    private void EnemyDestroyed()
    {
        DestroyEvent destroyEvent = GetComponent<DestroyEvent>();
        destroyEvent.CallOnDestroyEvent(false, health.GetStartingHealth());
    }

    /// <summary>
    /// This method, I hope, will make the enemy drop some good money
    /// </summary>
    private void DropMoney()
    {
        if (enemyDetails.moneyDetails.moneyPrefabs != null && enemyDetails.moneyDetails != null)
        {
            GameObject moneyPrefab = enemyDetails.moneyDetails.moneyPrefabs[Random.Range(0, enemyDetails.moneyDetails.moneyPrefabs.Length)];

                Money money = (Money)PoolManager.Instance.ReuseComponent
                    (moneyPrefab, transform.position, Quaternion.identity);

            money.InitializeMoney(enemyDetails.moneyDetails, enemyDetails.moneyDetails.moneyValue);

            money.gameObject.SetActive(true);
        }
    }

    private void SetEnemyDeathEffect()
    {
        // Process if there's a death effect & prefab
        if (enemyDetails.enemyDeathEffect.enemyDeathEffectPrefab != null && enemyDetails.enemyDeathEffect != null)
        {
            // Get ammo hit effect gameobject from the pool with particle system component
            EnemyDeathEffect deathEffect = (EnemyDeathEffect)PoolManager.Instance.ReuseComponent
                (enemyDetails.enemyDeathEffect.enemyDeathEffectPrefab, transform.position, Quaternion.identity);

            // Set hit effect
            deathEffect.SetDeathEffect(enemyDetails.enemyDeathEffect);

            // Set gameobject active (the particle system is set to automatically disable the
            // gameobject once finished)
            deathEffect.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// This method initializes the enemy with the enemy details SO
    /// </summary>
    /// <param name="enemyDetails">The SO used to pass the parameters for the enemy</param>
    /// <param name="enemySpawnNumber">The number of enemies that will spawn, every type of enemy has it's own counter</param>
    /// <param name="dungeonLevel">The dungeon level this enemy will spawn on</param>
    public void InitializeEnemyDetails(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingHealth(dungeonLevel);

        SetEnemyStartingWeapon();

        SetEnemyAnimationSpeed();

        //Calls the materialize effect class
        StartCoroutine(MaterializeEnemy());
    }

    /// <summary>
    /// Set the enemy movement update frame
    /// </summary>
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        //Set the frame on which the enemy will process it's updates
        enemyMovementAI.UpdateFramesNumber(enemySpawnNumber % Settings.targetFramesToSpreadPathfindingOver);
    }

    /// <summary>
    /// Sets the starting health for the enemy depending on the dungeon level
    /// </summary>
    private void SetEnemyStartingHealth(DungeonLevelSO dungeonLevel)
    {
        foreach (EnemyHealthDetails enemyHealthDetails in enemyDetails.enemyHealthDetailsArray)
        {
            if (enemyHealthDetails.dungeonLevel == dungeonLevel)
            {
                health.SetStartingHealth(enemyHealthDetails.enemyHealthAmount);
                return;
            }
        }

        health.SetStartingHealth(Settings.defaultEnemyHealth);
    }

    /// <summary>
    /// Set the enemy starting weapon with the weapon details SO
    /// </summary>
    private void SetEnemyStartingWeapon()
    {
        //Proceed if the enemy has a weapon
        if (enemyDetails.enemyWeapon != null)
        {
            Weapon weapon = new Weapon()
            {
                weaponDetails = enemyDetails.enemyWeapon,
                weaponReloadTimer = 0f,
                weaponMagAmmoRemaining = enemyDetails.enemyWeapon.weaponMagMaxCapacity,
                weaponTotalAmmoRemaining = enemyDetails.enemyWeapon.weaponTotalAmmoCapacity,
                isWeaponReloading = false
            };

            //Set the weapon for the enemy
            activeWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
    }

    /// <summary>
    /// Sets the animation speed for the enemies, to match movement speed
    /// </summary>
    private void SetEnemyAnimationSpeed()
    {
        animator.speed = enemyMovementAI.enemySpeed / Settings.enemyAnimationSpeed;
    }

    /// <summary>
    /// Materializes the enemy in the dungeon
    /// </summary>
    private IEnumerator MaterializeEnemy()
    {
        //Disables the enemy while it's been materialized
        EnableEnemy(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor,
            enemyDetails.enemyMaterializeTime, spriteRendererArray, enemyDetails.standardEnemyMaterial));

        //Enables the enemy again, after it has been materialzed
        EnableEnemy(true);
    }

    /// <summary>
    /// Enables/Disables the enemy
    /// </summary>
    /// <param name="isEnabled">False to Disable, True to Enable</param>
    public void EnableEnemy(bool isEnabled)
    {
        //Enable/Disable the colliders
        circleCollider.enabled = isEnabled;
        polygonCollider.enabled = isEnabled;

        //Enable/Disable the enemy movement AI
        enemyMovementAI.enabled = isEnabled;

        //Enable/Disable the fire weapon
        fireWeapon.enabled = isEnabled;
    }
}
