using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

#region Required Components
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FiringWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
#endregion
public class FireWeapon : MonoBehaviour
{
    private float firePrechargeTimer = 0f;
    private float fireRateCooldownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FiringWeaponEvent firingWeaponEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;

    private void Awake()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        firingWeaponEvent = GetComponent<FiringWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    private void OnEnable()
    {
        //Suscribe to the firing weapon event
        firingWeaponEvent.OnFireWeapon += FiringWeaponEvent_OnFiringWeapon;
    }

    private void OnDisable()
    {
        //Unsuscribe to the firing weapon event
        firingWeaponEvent.OnFireWeapon -= FiringWeaponEvent_OnFiringWeapon;
    }

    private void Update()
    {
        fireRateCooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Handle The Firing Weapon Event
    /// </summary>
    private void FiringWeaponEvent_OnFiringWeapon(FiringWeaponEvent firingWeaponEvent, FiringWeaponEventArgs firingWeaponEventArgs)
    {
        WeaponFire(firingWeaponEventArgs);
    }

    /// <summary>
    /// This Method Fires The Weapon
    /// </summary>
    private void WeaponFire(FiringWeaponEventArgs firingWeaponEventArgs)
    {
        WeaponPrecharge(firingWeaponEventArgs);

        if (firingWeaponEventArgs.hasFired)
        {
            if (WeaponIsReadyToFire())
            {
                FireAmmo(firingWeaponEventArgs.aimAngle, firingWeaponEventArgs.weaponAimAngle, firingWeaponEventArgs.weaponAimDirectionVector);

                ResetCooldownTimer();

                ResetPrechargeTimer();
            }
        }
    }

    /// <summary>
    /// This Handles The Weapon Precharge Feature
    /// </summary>
    private void WeaponPrecharge(FiringWeaponEventArgs firingWeaponEventArgs)
    {
        if (firingWeaponEventArgs.firedPreviosFrame)
        {
            firePrechargeTimer -= Time.deltaTime;
        }
        else
        {
            ResetPrechargeTimer();
        }
    }

    /// <summary>
    /// Reset The Precharge Weapon Timer
    /// </summary>
    private void ResetPrechargeTimer()
    {
        firePrechargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }

    /// <summary>
    /// Resets The Weapon Fire Rate Coolldown
    /// </summary>
    private void ResetCooldownTimer()
    {
        fireRateCooldownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    /// <summary>
    /// Returns True If The Weapon Is Ready To Fire
    /// </summary>
    /// <returns></returns>
    private bool WeaponIsReadyToFire()
    {
        //If theres no more remaining ammo and we dont have infinite ammo, then retur false
        if (activeWeapon.GetCurrentWeapon().weaponTotalAmmoRemaining <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
            return false;

        //If the weapon is reloading then return false
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
            return false;

        //If the weapon fire rate timer is greater than zero or if the weapon precharge timer is greater than zero, return false
        if (fireRateCooldownTimer > 0f || firePrechargeTimer > 0)
            return false;

        //If the ammo remaining in the mag is zero and the weapon doesnt have infinity mag capacity, then return false
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfinityMagCapacity && activeWeapon.GetCurrentWeapon().weaponMagAmmoRemaining <= 0)
        {
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);
            return false;
        }

        //Weapon is finally ready to fire, return true
        return true;
    }

    /// <summary>
    /// Sets Up The Bullet/Ammo Using A GameObject From The Object Pool
    /// </summary>
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    /// <summary>
    /// Coroutine that enables the firing of the weapon, the Object Pool is used in this method
    /// </summary>
    /// <param name="currentAmmo">The current ammo for the weapon</param>
    /// <param name="aimAngle">The aim angle for the weapon</param>
    /// <param name="weaponAimAngle">The aim angle used to rotate the weapon</param>
    /// <param name="weaponAimDirectionVector">The vector aim angle for initializing the ammo</param>
    /// <returns></returns>
    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo,float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;

        //Get a random number of bullets that are gonna be fired, per shoot
        int bulletsPerShoot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

        //Get a random interval that the bullets are gonna spawn with
        float ammoSpawnInterval;

        if (bulletsPerShoot > 1)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.minSpawnInterval, currentAmmo.maxSpawnInterval);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }

        //Loop for the number of bullets that are gonna be fired
        while (ammoCounter < bulletsPerShoot)
        {
            ammoCounter++;

            //Get a random ammo prefab from the array in the Ammo Details SO
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            //Get a random speed btw the min and max values in the Ammo Details SO
            float ammoSpeed = Random.Range(currentAmmo.minAmmoSpeed, currentAmmo.maxAmmoSpeed);

            //Get the component with the IFireable interface
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetFirePosition(), Quaternion.identity);

            //Initialise the ammo
            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            //Wait for the ammo per shoot timegap
            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        //Reduce the ammo mag capacity if it doesnt have infinity mag capacity
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfinityMagCapacity)
        {
            activeWeapon.GetCurrentWeapon().weaponMagAmmoRemaining--;
            activeWeapon.GetCurrentWeapon().weaponTotalAmmoRemaining--;
        }

        //Call the fired weapon event
        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());

        //Display the weapon shoot effect
        WeaponShootEffect(aimAngle);

        //Play the sound effects specified for this weapon
        WeaponSoundEffects();
    }

    /// <summary>
    /// Spawns The Weapon Shoot Effect
    /// </summary>
    private void WeaponShootEffect(float aimAngle)
    {
        // Process if there is a shoot effect & prefab
        if (activeWeapon.GetCurrentWeapon().weaponDetails.shootEffect != null
            && activeWeapon.GetCurrentWeapon().weaponDetails.shootEffect.shootEffectPrefab != null)
        {
            // Get weapon shoot effect gameobject from the pool with particle system component
            WeaponShootEffect shootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent
                (activeWeapon.GetCurrentWeapon().weaponDetails.shootEffect.shootEffectPrefab,
                activeWeapon.GetWeaponFireEffectPosition(), Quaternion.identity);

            // Set shoot effect
            shootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetails.shootEffect, aimAngle);

            // Set gameobject active (the particle system is set to automatically disable the
            // gameobject once finished)
            shootEffect.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Play The Sound Effects For This Weapon
    /// </summary>
    private void WeaponSoundEffects()
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect != null)
        {
            SoundManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect);
        }
    }
}
