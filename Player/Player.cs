using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region Required Components
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(ReceiveTouchDamage))]
[RequireComponent(typeof(DealTouchDamage))]
[RequireComponent(typeof(Destroy))]
[RequireComponent(typeof(DestroyEvent))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementByPositionEvent))]
[RequireComponent(typeof(MovementByPosition))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(ActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FiringWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
#endregion Required Components
public class Player : MonoBehaviour
{
    #region Player Necessary Details
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public Health playerHealth;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public DestroyEvent destroyEvent;
    [HideInInspector] public PlayerControl playerControl;
    #endregion

    #region Player Movement Necessary Details
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public MovementByVelocityEvent movementByVelEvent;
    [HideInInspector] public MovementByPositionEvent movementByPosEvent;

    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public ActiveWeaponEvent activeWeaponEvent;
    [HideInInspector] public ActiveWeapon currentWeapon;
    [HideInInspector] public FiringWeaponEvent firingWeaponEvent;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public ReloadWeapon reloadWeapon;
    #endregion

    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    public List<Weapon> weaponList = new List<Weapon>();

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        healthEvent = GetComponent<HealthEvent>();
        destroyEvent = GetComponent<DestroyEvent>();
        playerControl = GetComponent<PlayerControl>();

        movementByVelEvent = GetComponent<MovementByVelocityEvent>();
        movementByPosEvent = GetComponent<MovementByPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();

        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        activeWeaponEvent = GetComponent<ActiveWeaponEvent>();
        currentWeapon = GetComponent<ActiveWeapon>();
        firingWeaponEvent = GetComponent<FiringWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        reloadWeapon = GetComponent<ReloadWeapon>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    /// <summary>
    /// Handles the health event
    /// </summary>
    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        //If the health of the player is less or equal to zero
        if (healthEventArgs.healthAmount <= 0f)
        {
            destroyEvent.CallOnDestroyEvent(true, 0);
        }
    }

    /// <summary>
    /// Initializes The Player And Sets Its Health
    /// </summary>
    public void Initialize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        //Set player health
        SetPlayerHealth();

        //Initialise the initial weapon for the player
        CreatePlayerInitialWeapon();
    }

    /// <summary>
    /// Sets The Player Initial Weapon 
    /// </summary>
    private void CreatePlayerInitialWeapon()
    {
        weaponList.Clear();

        foreach (WeaponDetailsSO weaponDetails in playerDetails.initialWeaponList)
        {
            //Add the weapon to the player to use
            AddWeaponToPlayer(weaponDetails);
        }
    }

    private void SetPlayerHealth()
    {
        playerHealth.SetStartingHealth(playerDetails.playerHealthAmount);
    }

    /// <summary>
    /// Returns The Player Position
    /// </summary>
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Add The Weapon To The Player Weapon Dictionary
    /// </summary>
    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
    {
        Weapon weapon = new Weapon()
        {
            weaponDetails = weaponDetails,
            weaponReloadTimer = 0f,
            weaponMagAmmoRemaining = weaponDetails.weaponMagMaxCapacity,
            weaponTotalAmmoRemaining = weaponDetails.weaponTotalAmmoCapacity,
            isWeaponReloading = false
        };

        //Adds the weapon to the weapon list
        weaponList.Add(weapon);

        //Set the position of the newly added weapon in the list
        weapon.weaponListPosition = weaponList.Count;

        //Activate the newly added weapon
        activeWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }

    /// <summary>
    /// Returns true if the weapon is already held by the player, returns false otherwise
    /// </summary>
    public bool IsWeaponHeldByPlayer(WeaponDetailsSO weaponDetailsSO)
    {
        foreach (Weapon weapon in weaponList)
        {
            if (weapon.weaponDetails == weaponDetailsSO) return true;
        }

        return false;
    }
}
