using System.Collections;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUseable
{
    #region Tooltip
    [Tooltip("Set the color that will be used when the chest is materialized")]
    #endregion
    [ColorUsage(false, true)]
    [SerializeField] private Color materializeColor;

    #region Tooltip
    [Tooltip("Set the time it will take to materialize the chest")]
    #endregion
    [SerializeField] private float materializeTime = 4f;

    #region Tooltip
    [Tooltip("Populate with the gameobject ItemSpawnPoint in the chest prefab")]
    #endregion
    [SerializeField] private Transform itemSpawnPos;

    private int healthPercent;
    private int ammoPercent;
    private WeaponDetailsSO weaponDetails;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;
    private bool isEnabled = false;
    private ChestState chestState = ChestState.closed;
    private GameObject chestItemGameobject;
    private ChestItem chestItem;
    private TextMeshPro messageText;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
        messageText = GetComponent<TextMeshPro>();
    }

    public void Initialize(bool shouldMaterialize, int healthPercent, WeaponDetailsSO weaponDetailsSO, int ammoPercent)
    {
        this.healthPercent = healthPercent;
        this.weaponDetails = weaponDetailsSO;
        this.ammoPercent = ammoPercent;

        if (shouldMaterialize)
        {
            StartCoroutine(MaterializeChest());
        }
        else
        {
            EnableChest();
        }
    }

    /// <summary>
    /// Materializes the chest into the world
    /// </summary>
    /// <returns></returns>
    private IEnumerator MaterializeChest()
    {
        SpriteRenderer[] spriteRenderers = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader,
            materializeColor, materializeTime, spriteRenderers, GameResources.Instance.litMaterial));

        EnableChest();
    }

    /// <summary>
    /// Enables the chest
    /// </summary>
    private void EnableChest()
    {
        isEnabled = true;
    }

    /// <summary>
    /// Use the chest - the action will vary depending on the state of the chest itself
    /// </summary>
    public void UseItem()
    {
        if (!isEnabled) return;

        switch (chestState)
        {
            case ChestState.closed:
                OpenChest();
                break;
            
            case ChestState.healthItem:
                CollectHealthItem();
                break;
            
            case ChestState.ammoItem:
                CollectAmmoItem();
                break;
            
            case ChestState.weaponItem:
                CollectWeaponItem();
                break;
            
            case ChestState.empty:
                return;
            
            default:
                break;
        }
    }

    /// <summary>
    /// Opens the chest when its first used    
    /// </summary>
    private void OpenChest()
    {
        animator.SetBool(Settings.used, true);

        SoundManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpenSound);

        //Check if the player already has the weapon - if so set the weapon to null
        if (weaponDetails != null)
        {
            if (GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
                weaponDetails = null;
        }

        UpdateChestState();
    }

    /// <summary>
    /// Create the items based on what should spawn at a give state
    /// </summary>
    private void UpdateChestState()
    {
        if (healthPercent != 0)
        {
            chestState = ChestState.healthItem;
            SpawnHealthItem();
        }
        else if (ammoPercent != 0)
        {
            chestState = ChestState.ammoItem;
            SpawnAmmoItem();
        }
        else if (weaponDetails != null)
        {
            chestState = ChestState.weaponItem;
            SpawnWeaponItem();
        }
        else
        {
            chestState = ChestState.empty;
        }
    }

    /// <summary>
    /// Instantiate a item for the chest
    /// </summary>
    private void InstantiateItem()
    {
        chestItemGameobject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform);

        chestItem = chestItemGameobject.GetComponent<ChestItem>();
    }

    /// <summary>
    /// Instantiate a health item for the player
    /// </summary>
    private void SpawnHealthItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.healthIcon, healthPercent.ToString() + "%", itemSpawnPos.position, materializeColor);
    }

    /// <summary>
    /// Collect the health item and add the health to the player
    /// </summary>
    private void CollectHealthItem()
    {
        //Check if the item exist and has been materialized
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        //Add the health to the player
        GameManager.Instance.GetPlayer().playerHealth.AddHealth(healthPercent);

        //Play the pickup sound
        SoundManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickupSound);

        healthPercent = 0;

        Destroy(chestItemGameobject);

        UpdateChestState();
    }

    /// <summary>
    /// Instantiates the ammo item for the player to use
    /// </summary>
    private void SpawnAmmoItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.bulletSprite, ammoPercent.ToString() + "%", itemSpawnPos.position, materializeColor);
    }

    /// <summary>
    /// Collects the ammo item and adds that ammo to the current weapon
    /// </summary>
    private void CollectAmmoItem()
    {
        //Check if the item exist and has been materialized
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        Player player = GameManager.Instance.GetPlayer();

        //Update the ammo for the current weapon
        player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), ammoPercent);

        //Play the pickup sound
        SoundManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickupSound);

        ammoPercent = 0;

        Destroy(chestItemGameobject);

        UpdateChestState();
    }

    /// <summary>
    /// Instantiates the weapon item for the player to collect
    /// </summary>
    private void SpawnWeaponItem()
    {
        InstantiateItem();

        chestItemGameobject.GetComponent<ChestItem>().Initialize(weaponDetails.weaponSprite, weaponDetails.weaponName, itemSpawnPos.position, materializeColor);
    }

    private void CollectWeaponItem()
    {
        //Check if the item exist and has been materialized
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        //If the player doesn't already has that weapon, then add it
        if (!GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
        {
            GameManager.Instance.GetPlayer().AddWeaponToPlayer(weaponDetails);

            //Play the pickup sound
            SoundManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickupSound);
        }
        else
        {
            StartCoroutine(DisplayMessage("Weapon\nAlready\nEquipped", 5f));
        }

        weaponDetails = null;

        Destroy(chestItemGameobject);

        UpdateChestState();
    }

    /// <summary>
    /// Display a message above the chest
    /// </summary>
    private IEnumerator DisplayMessage(string message, float time)
    {
        messageText.text = message;

        yield return new WaitForSeconds(time);

        messageText.text = "";
    }
}
