using System;
using UnityEngine;
using Random = UnityEngine.Random;
using ExplosionForce2D;
using System.Linq;
using ExplosionForce2D.PropertyAttributes;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("Populate with the child component TrailRenderer, that is found in the ammo prefab")]
    #endregion
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0f;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool hasAmmoMaterialSet = false;
    private bool overrideAmmoMovement;
    private bool isColliding = false; //This will prevent the same bullet from hitting two collider at the same time

    #region Header Explosive Ammo Settings
    [Header("Explosive Ammo Settings")]
    #region Tooltip
    [Tooltip("Check if the ammo is explosive")]
    #endregion
    public bool isExplosiveAmmo = false;

    #region Tooltip
    [Tooltip("The Radius of the explosion")]
    #endregion
    public float radius;

    #region Tooltip
    [Tooltip("The Force of the explosion")]
    #endregion
    public float force;

    #region Tooltip
    [Tooltip("The mode of the explosion - it can be Impulse or Force")]
    #endregion
    public ForceMode2D forceMode;

    #region Tooltip
    [Tooltip("The layers that will be affected by the explosion")]
    #endregion
    public LayerMask explosionLayerFilter;

    #region Tooltip
    [Tooltip("The Gameobject that contains the universal explosion script, the gameobject needs to be a child of the bullet gameobject")]
    #endregion
    public GameObject explosion;

    #region Tooltip
    [Tooltip("Check if the explosion will deal damage")]
    #endregion
    [SerializeField] private bool sendExplosionDamage = false;
    [BoolConditionalHide("sendExplosionDamage", true, false)][SerializeField] private int explosionDamage = 1;
    [BoolConditionalHide("sendExplosionDamage", true, false)][SerializeField] private string methodToCall = "ReceiveExplosionDamage";
    [BoolConditionalHide("sendExplosionDamage", true, false)][SerializeField] private bool modifyDamageByDistance = false;

    private Vector3 explosionPos;
    private SingleExplosion explosionScript;
    private Transform explosionTransform;
    private Collider2D[] collider2Ds = new Collider2D[] {};
    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (isExplosiveAmmo)
        {
            explosionTransform = explosion.transform;
            explosionScript = explosion.GetComponent<SingleExplosion>();
        }
    }

    private void Update()
    {
        if (ammoChargeTimer > 0)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!hasAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            hasAmmoMaterialSet = true;
        }

        //Don't move the ammo if the movement has been overriden, meaning this ammo is part of an ammo pattern
        if (!overrideAmmoMovement)
        {
            //Calculate distance vector to move the bullet
            Vector3 distanceVector = ammoSpeed * Time.deltaTime * fireDirectionVector;

            transform.position += distanceVector;

            //Disable after the max range has been reached
            ammoRange -= distanceVector.magnitude;

            if (ammoRange < 0)
            {
                if (ammoDetails.isPlayerAmmo)
                {
                    //Call the multiply score event
                    StaticEventHandler.CallMultiplierEvent(false);
                }
                DisableAmmo();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isColliding) return;

        if (isExplosiveAmmo)
        {
            StartExplosion();
        }

        //Deal damage to objects
        DealDamage(collider);

        //Spawns the ammo hit effect
        HitEffectAmmo();
        
        DisableAmmo();
    }


    /// <summary>
    /// Call the health event to deal damage to the sorroundings
    /// </summary>
    private void DealDamage(Collider2D collider)
    {
        collider.TryGetComponent(out Health health);

        bool enemyHit = false;

        if (health != null)
        {
            isColliding = true;
            health.TakeDamage(ammoDetails.ammoDamage);

            if (health.enemy != null)
                enemyHit = true;
        }

        //If is player ammo then update the multiplier in the UI
        if (ammoDetails.isPlayerAmmo)
        {
            if (enemyHit)
            {
                //Update the multiplier by 1
                StaticEventHandler.CallMultiplierEvent(true);
            }
            else
            {
                //Reduce the multiplier by 1
                StaticEventHandler.CallMultiplierEvent(false);
            }
        }
    }

    /// <summary>
    /// Starts to initiallise the explosion in the explosive bullet
    /// </summary>
    private void StartExplosion()
    {
        Debug.Log("Creating a Single Explosion Script...");
        SingleExplosion singleExplosionScript = explosionScript.GetComponent<SingleExplosion>();

        HandleExplosionPosition();

        InitialiseExplosiveAmmo(singleExplosionScript);
    }

    /// <summary>
    /// Initiliase the parameters of the explosion
    /// </summary>
    /// <param name="singleExplosionScript">The Single Explosion Script Used</param>
    private void InitialiseExplosiveAmmo(SingleExplosion singleExplosionScript)
    {
        singleExplosionScript.explosionForce = this.force;
        singleExplosionScript.explosionRadius = this.radius;
        singleExplosionScript.forceMode = this.forceMode;
        singleExplosionScript.layerFilter = this.explosionLayerFilter;

        Debug.Log("Phase 2 Complete. Activate Explosion On Disable: " + singleExplosionScript.onDisable);

        if (sendExplosionDamage)
        {
            singleExplosionScript.sendExplosionDamage = true;

            singleExplosionScript.explosionDamage = explosionDamage;
            singleExplosionScript.methodToCall = methodToCall;
            singleExplosionScript.modifyDamageByDistance = modifyDamageByDistance;
            Debug.Log("Phase 3 Complete. Explosion Created Succesfully. " + "Deactivate Bullet After Explosion: " + singleExplosionScript.deactivateGameobject);
        }

        ActivateExplosion(singleExplosionScript);
    }

    /// <summary>
    /// Activates the explosion after all the parameters have been set
    /// </summary>
    /// <param name="singleExplosionScript">The Single Explosion Script Passed</param>
    private void ActivateExplosion(SingleExplosion singleExplosionScript)
    {
        singleExplosionScript.Activate();
    }

    /// <summary>
    /// Moves the position of the explosion to the bullet position
    /// </summary>
    private void HandleExplosionPosition()
    {
        explosionPos = transform.position;
        explosionPos.z = 0f;
        explosionTransform.position = explosionPos;
        Debug.Log("Explosion Position Set...");
    }

    /// <summary>
    /// This Method Initialises The Ammo So It Can Be Fired, Using The Specified Variables
    /// If This Ammo Is Part Of An Ammo Pattern, The Ammo Movement Can Be Overriden By Setting The Bool overrideAmmoMovement To True
    /// </summary>
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Bullet
        this.ammoDetails = ammoDetails;

        isColliding = false;

        //Sets the fire direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        spriteRenderer.sprite = ammoDetails.ammoSprite;

        if (ammoDetails.ammoChargeTime > 0)
        {
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            hasAmmoMaterialSet = false;
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            hasAmmoMaterialSet = true;
        }

        //Set ammo range
        ammoRange = ammoDetails.ammoRange;

        //Set ammo speed
        this.ammoSpeed = ammoSpeed;

        //Override the ammo movement
        this.overrideAmmoMovement = overrideAmmoMovement;

        //Activate the ammo gameobject
        gameObject.SetActive(true);
        #endregion

        #region Trail Renderer

        if (ammoDetails.hasAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailLifetime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion
    }

    /// <summary>
    /// Set The Ammo Fire Direction Based On The Input Angle And Direction Adjusted By The Random Spread
    /// </summary>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        float spreadRandomAngle = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        //Get a random toggle between 1 or -1
        int spreadRandomToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        //Adjust the bullet fire angle with the random spread
        fireDirectionAngle += spreadRandomToggle * spreadRandomAngle;

        //Set the bullet rotation if needed
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        //Set the bullet fire direction
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    /// <summary>
    /// Disables The Ammo - Thus Returning It To The Object Pool
    /// </summary>
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Spawns The Ammo Hit Effect Particle System
    /// </summary>
    private void HitEffectAmmo()
    {
        // Process if there is a hit effect & prefab
        if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.bulletHitEffectPrefab != null)
        {
            // Get ammo hit effect gameobject from the pool with particle system component
            AmmoHitEffect hitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent
                (ammoDetails.ammoHitEffect.bulletHitEffectPrefab, transform.position, Quaternion.identity);

            // Set hit effect
            hitEffect.SetAmmoHitEffect(ammoDetails.ammoHitEffect);

            // Set gameobject active (the particle system is set to automatically disable the
            // gameobject once finished)
            hitEffect.gameObject.SetActive(true);
        }
    }

    private void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(explosionPos, radius);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }
#endif
    #endregion
}
