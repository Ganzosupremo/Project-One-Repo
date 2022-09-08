using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Select the layers that the enemy bullets will collide with")]
    #endregion
    [SerializeField] private LayerMask m_LayerMask;

    #region Tooltip
    [Tooltip("Populate this with the WeaponFirePosition Gameobject in the enemy prefab")]
    #endregion
    [SerializeField] private Transform m_WeaponShootPosition;

    private Enemy m_Enemy;
    private EnemyDetailsSO m_EnemyDetails;
    private float m_FiringDelayTimer;
    private float m_FiringDurationTimer;

    private void Awake()
    {
        m_Enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        m_EnemyDetails = m_Enemy.enemyDetails;

        m_FiringDelayTimer = WeaponShootDelay();
        m_FiringDurationTimer = WeaponShootDuration();
    }

    private void Update()
    {
        //Update the timers
        m_FiringDelayTimer -= Time.deltaTime;

        if (m_FiringDelayTimer < 0f)
        {
            if (m_FiringDurationTimer >= 0f)
            {
                m_FiringDurationTimer -= Time.deltaTime;

                FireWeapon();
            }
            else
            {
                //Reset the timers
                m_FiringDelayTimer = WeaponShootDelay();
                m_FiringDurationTimer = WeaponShootDuration();
            }
        }
    }

    /// <summary>
    /// Calculate a random shoot delay btw the min and the max values
    /// </summary>
    private float WeaponShootDelay()
    {
        return Random.Range(m_EnemyDetails.firingMinDelay, m_EnemyDetails.firingMaxDelay);
    }

    /// <summary>
    /// Calculate a random shoot duration btw the min and the max values
    /// </summary>
    private float WeaponShootDuration()
    {
        return Random.Range(m_EnemyDetails.firingMinDuration, m_EnemyDetails.firingMaxDuration);
    }

    /// <summary>
    /// Fires the weapon
    /// </summary>
    private void FireWeapon()
    {
        //Get the distance from the player
        Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;

        //Calculate the distance of the player of the enemy weapon shoot position
        Vector3 weaponDirection = GameManager.Instance.GetPlayer().GetPlayerPosition() - m_WeaponShootPosition.position;

        //Get the weapon to player angle
        float WeaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        //Get the enemy to player angle
        float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);

        //Set the enemy aim direction
        AimDirection enemyAimDirection = HelperUtilities.GetAimDirection(enemyAngleDegrees);

        //Call the weapon aim event
        m_Enemy.aimWeaponEvent.CallAimWeaponEvent(enemyAimDirection, enemyAngleDegrees, WeaponAngleDegrees, weaponDirection);

        //Only fire if the enemy has a weapon
        if (m_EnemyDetails.enemyWeapon != null)
        {
            //Get the ammo range
            float enemyAmmoRange = m_EnemyDetails.enemyWeapon.weaponCurrentAmmo.ammoRange;

            //Is the player in range
            if (playerDirectionVector.magnitude <= enemyAmmoRange)
            {
                //Does this enemy is required to have a line of sight before firing?
                if (m_EnemyDetails.lineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange)) return;

                //Call the fire weapon event
                m_Enemy.firingWeaponEvent.CallOnFireWeaponEvent(true, true, enemyAimDirection, enemyAngleDegrees, WeaponAngleDegrees, weaponDirection);
            }
        }
    }

    /// <summary>
    /// Returns true if the player is in line of sight, returns false otherwise
    /// </summary>
    private bool IsPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRange)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(m_WeaponShootPosition.position, (Vector2)weaponDirection, enemyAmmoRange, m_LayerMask);

        if (raycastHit2D && raycastHit2D.transform.CompareTag(Settings.playerTag))
        {
            return true;
        }

        return false;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(m_WeaponShootPosition), m_WeaponShootPosition);
    }
#endif
    #endregion
}
