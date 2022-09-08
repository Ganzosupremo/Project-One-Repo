using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEngine.Rendering.DebugUI;

public class WeaponStatesUI : MonoBehaviour
{
    #region Header UI Objects References
    [Header("References For The UI Objects")]
    #endregion

    #region Tooltip
    [Tooltip("Put here the child gameobject called, WeaponImage")]
    #endregion
    [SerializeField] private Image weaponImage;

    #region Tooltip
    [Tooltip("Put here the transform of the child gameobject called, BulletHolder")]
    #endregion
    [SerializeField] private Transform bulletHolderTransform;

    #region
    [Tooltip("Put here the reloadText child gameobject")]
    #endregion
    [SerializeField] private TextMeshProUGUI reloadText;

    #region Tooltip
    [Tooltip("Put here the RemainingBulletsText child gameobject")]
    #endregion
    [SerializeField] private TextMeshProUGUI remainingBulletsText;

    #region Tooltip
    [Tooltip("Put here the WeaponName child gameobject")]
    #endregion
    [SerializeField] private TextMeshProUGUI weaponName;

    #region Tooltip
    [Tooltip("Put here the transform of the ReloadBar child gameobject")]
    #endregion 
    [SerializeField] private Transform reloadBar;

    #region Tooltip
    [Tooltip("Put here the image of the ReloadBar child gameobject")]
    #endregion
    [SerializeField] private Image reloadBarImage;
    
    #region Tooltip
    [Tooltip("Put here the gameobject of the Reloab bar image, it's used for lean tween")]
    #endregion
    [SerializeField] private GameObject reloadBarGameObject;

    #region Tooltip
    [Tooltip("The time for tweening the reload bar")]
    #endregion
    [SerializeField] private float tweenTime;

    #region Tooltip
    [Tooltip("Colors of the reload bar")]
    #endregion
    [SerializeField] private Color beginColor, endColor;

    private Player player;
    private List<GameObject> bulletIconList = new();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextRoutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
    }

    private void Start()
    {
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
        LeanTween.init(900);
    }

    private void OnEnable()
    {
        //Suscribe to four diferent events: Active Weapon, Weapon Fired Event, Reloading Weapon And Weapon Reloaded 
        player.activeWeaponEvent.OnSetActiveWeapon += ActiveWeaponEvent_OnSetActiveWeapon;

        player.weaponFiredEvent.AfterWeaponFired += WeaponFiredEvent_AfterWeaponFired;

        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnWeaponReloading;

        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;


    }

    private void OnDisable()
    {
        //Unsuscribe to four diferent events: Active Weapon, Weapon Fired Event, Reloading Weapon And Weapon Reloaded 
        player.activeWeaponEvent.OnSetActiveWeapon -= ActiveWeaponEvent_OnSetActiveWeapon;

        player.weaponFiredEvent.AfterWeaponFired -= WeaponFiredEvent_AfterWeaponFired;

        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnWeaponReloading;

        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void ActiveWeaponEvent_OnSetActiveWeapon(ActiveWeaponEvent activeWeaponEvent, ActiveWeaponEventArgs activeWeaponEventArgs)
    {
        SetActiveWeapon(activeWeaponEventArgs.playerWeapon);
    }

    /// <summary>
    /// Sets The Active Weapon On The UI
    /// </summary>
    private void SetActiveWeapon(Weapon playerWeapon)
    {
        UpdateActiveWeaponImage(playerWeapon.weaponDetails);
        UpdateActiveWeaponName(playerWeapon);
        UpdateAmmoText(playerWeapon);
        UpdateBulletIcons(playerWeapon);

        if (playerWeapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(playerWeapon);
        }
        else
        {
            ResetWeaponReloadBar(playerWeapon);
        }

        UpdateWeaponReloadText(playerWeapon);
    }

    /// <summary>
    /// Event Handler
    /// </summary>
    private void WeaponFiredEvent_AfterWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    /// <summary>
    /// Updates The UI When The Bullets Had Been Fired
    /// </summary>
    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateBulletIcons(weapon);
        UpdateWeaponReloadText(weapon);
    }

    /// <summary>
    /// Event Handler
    /// </summary>
    private void ReloadWeaponEvent_OnWeaponReloading(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    /// <summary>
    /// Event Handler
    /// </summary>
    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    /// <summary>
    /// Updates The UI When The Weapon Has Been Reloaded
    /// </summary>
    private void WeaponReloaded(Weapon weapon)
    {
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateWeaponReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateBulletIcons(weapon);
            ResetWeaponReloadBar(weapon);
        }
    }

    /// <summary>
    /// Shows The Active Weapon Image On The UI
    /// </summary>
    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

    /// <summary>
    /// Shows The Active Weapon Name And Position On The UI
    /// </summary>
    private void UpdateActiveWeaponName(Weapon playerWeapon)
    {
        weaponName.text = "<" + playerWeapon.weaponListPosition + ">" + playerWeapon.weaponDetails.weaponName.ToUpper();
    }

    /// <summary>
    /// Shows The Total Ammo Remaining On The UI
    /// </summary>
    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            remainingBulletsText.text = "INFINITE POWER!";
        }
        else
        {
            remainingBulletsText.text = weapon.weaponTotalAmmoRemaining.ToString() + "/" + weapon.weaponDetails.weaponTotalAmmoCapacity.ToString();
        }
    }

    /// <summary>
    /// Shows The Weapon Bullets Remaining On The UI
    /// </summary>
    private void UpdateBulletIcons(Weapon weapon)
    {
        ClearAlreadyLoadedIcons();

        for (int i = 0; i < weapon.weaponMagAmmoRemaining; i++)
        {
            // Instantiate ammo icon prefab
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, bulletHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiSpacingForAmmoIcoin * i);

            bulletIconList.Add(ammoIcon);
        }
    }

    /// <summary>
    /// Clear The Bullet Icons On The UI
    /// </summary>
    private void ClearAlreadyLoadedIcons()
    {
        // Loop through icon gameobjects and destroy
        foreach (GameObject ammoIcon in bulletIconList)
        {
            Destroy(ammoIcon);
        }

        bulletIconList.Clear();
    }

    /// <summary>
    /// Updates The Reload Bar, So It Shows When A Weapon Is Reloading
    /// </summary>
    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfinityMagCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateWeaponReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));

    }

    /// <summary>
    /// Update The Weapon Bullet Text To Say 'Reload' When There Are No More Bullets Left
    /// </summary>
    private void UpdateWeaponReloadText(Weapon weapon)
    {
        if ((!weapon.weaponDetails.hasInfinityMagCapacity) && (weapon.weaponMagAmmoRemaining <= 0 || weapon.isWeaponReloading))
        {
            reloadBarImage.color = Color.red;

            StopBlinkingReloadTextRoutine();

            blinkingReloadTextRoutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    /// <summary>
    /// Start The Coroutine So The Reload Text Kinda Blinks
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "NIGGA!!";
            SoundManager.Instance.PlaySoundEffect(GameResources.Instance.reloadSound);
            yield return new WaitForSeconds(0.6f);

            reloadText.text = "RELOAD!!";
            yield return new WaitForSeconds(0.7f);
        }
    }

    /// <summary>
    /// Stops The Reload Text From Blinking
    /// </summary>
    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextRoutine();

        reloadText.text = "";
    }

    /// <summary>
    /// Stops The Blinking Reload Text Coroutine
    /// </summary>
    private void StopBlinkingReloadTextRoutine()
    {
        if (blinkingReloadTextRoutine != null)
        {
            StopCoroutine(blinkingReloadTextRoutine);
        }
    }

    /// <summary>
    /// Animates The Weapon Reload Bar
    /// </summary>
    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        //Set the reload bar color to red
        reloadBarImage.color = Color.red;

        //Animate the reload bar
        while (currentWeapon.isWeaponReloading)
        {
            float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;

            //reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);
            
            //Lean Tween to animate the reload weapon bar
            LeanTween.scaleX(reloadBarGameObject, barFill * 1.009f, barFill).setEaseInBounce();

            yield return null;
        }
    }

    /// <summary>
    /// Sets The Weapon Reload Bar Color To Green
    /// </summary>
    private void ResetWeaponReloadBar(Weapon weapon)
    {
        StopReloadWeaponCoroutine();

        reloadBarImage.color = Color.green;

        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    /// <summary>
    /// Stops The Coroutine For Reloading The Weapon
    /// </summary>
    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage),weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(bulletHolderTransform), bulletHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(remainingBulletsText), remainingBulletsText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBarImage), reloadBarImage);
    }
#endif
    #endregion
}
