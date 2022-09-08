using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChestSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct RangeByLevel
    {
        public DungeonLevelSO dungeonLevel;
        [Range(0, 100)] public int minSpawnChance;
        [Range(0, 100)] public int maxSpawnChance;
    }

    [SerializeField] private GameObject chestPrefab;

    #region Header Chest Chance Parameters
    [Space(10)]
    [Header("Chest Chance Parameters")]
    #endregion

    #region Tooltip
    [Tooltip("The minimun chance for a chest to spawn")]
    #endregion
    [SerializeField] [Range(0, 100)] private int minChestSpawnChance;

    #region Tooltip
    [Tooltip("The maximum chance for a chest to spawn")]
    #endregion
    [SerializeField] [Range(0, 100)] private int maxChestSpawnChance;

    #region Tooltip
    [Tooltip("The spawn chance for a chance can be overriden by dungeon level")]
    #endregion
    [SerializeField] private List<RangeByLevel> chestSpawnChanceByLevel;

    #region Header Chest Spawn Details
    [Space(10)]
    [Header("Chest Spawn Details")]
    #endregion

    [SerializeField] private ChestSpawnEvent chestSpawnEvent;
    [SerializeField] private ChestSpawnPosition chestSpawnPosition;

    #region Tooltip
    [Tooltip("The minimun number of items to spawn (only one type of item will spawn)")]
    #endregion
    [SerializeField][Range(0, 3)] private int minNumberOfItemsToSpawn;

    #region Tooltip
    [Tooltip("The maximum number of items to spawn (only one type of item will spawn)")]
    #endregion
    [SerializeField][Range(0,3)] private int maxNumberOfItemsToSpawn;

    #region Header Chest Content Details
    [Space(10)]
    [Header("Chest Content Details")]
    #endregion

    #region Tooltip
    [Tooltip("The weapon that will spawn in each level and it's spawn ratio")]
    #endregion
    [SerializeField] private List<SpawnableObjectByLevel<WeaponDetailsSO>> weaponSpawnByLevel;

    #region Tooltip
    [Tooltip("The range the health will have when spawned")]
    #endregion
    [SerializeField] private List<RangeByLevel> healthSpawnByLevel;

    #region Tooltip
    [Tooltip("The range the ammo will have when spawned")]
    #endregion
    [SerializeField] private List<RangeByLevel> ammoSpawnByLevel;

    private bool chestSpawned = false;
    private Room chestRoom;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }

    /// <summary>
    /// On Room Changed Event Handler
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        //Get the room where the chest is
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        //Spawn the chest if the state is on the room entry
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onRoomEntry && chestRoom == roomChangedEventArgs.room)
        {
            SpawnChest();
        }
    }

    /// <summary>
    /// On Room Enemies Defeated Event Handler
    /// </summary>
    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        //Get the room where the chest is, if we don't already have it
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        //Spawn the chest if the state is on the enemies defeated
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onEnemiesDefeated && chestRoom == roomEnemiesDefeatedArgs.room)
        {
            SpawnChest();
        }
    }

    private void SpawnChest()
    {
        chestSpawned = true;

        //Should the chest spawn based on a specified chance? if not, return
        if (!RandomSpawnChance()) return;

        //Get the number of items that the chest will spawn(max 1 of each)
        GetItemsToSpawn(out int ammoNum, out int healtNum, out int weaponNum);

        GameObject chestGameObject = Instantiate(chestPrefab, this.transform);

        //Position the chest
        if (chestSpawnPosition == ChestSpawnPosition.atSpawnerPosition)
        {
            chestGameObject.transform.position = this.transform.position;
        }
        else if (chestSpawnPosition == ChestSpawnPosition.atPlayerPosition)
        {
            //Get nearest position to the player
            Vector3 spawnPosition = HelperUtilities.GetSpawnPointNearestToPlayer(GameManager.Instance.GetPlayer().transform.position);

            //Calculate some random variation
            Vector3 variation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            chestGameObject.transform.position = spawnPosition + variation;
        }
        Chest chest = chestGameObject.GetComponent<Chest>();

        //Initialize the chest
        if (chestSpawnEvent == ChestSpawnEvent.onRoomEntry)
        {
            //Don't use the materialize effect
            chest.Initialize(false, GetHealthPercent(healtNum), GetWeaponDetails(weaponNum), GetAmmoPercent(ammoNum));
        }
        else
        {
            //Use the materialize effect
            chest.Initialize(true, GetHealthPercent(healtNum), GetWeaponDetails(weaponNum), GetAmmoPercent(ammoNum));
        }
    }

    /// <summary>
    /// Check if the chest should be spawned based on the chest spawn chance, returns true if so
    /// </summary>
    private bool RandomSpawnChance()
    {
        int chancePercent = Random.Range(minChestSpawnChance, maxChestSpawnChance + 1);

        //If an override has been set for a particular level
        foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevel)
        {
            if (rangeByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                chancePercent = Random.Range(rangeByLevel.minSpawnChance, rangeByLevel.maxSpawnChance);
                break;
            }
        }

        //Get a random value btw 1 and 100
        int randomPercent = Random.Range(1, 100 + 1);

        if (randomPercent <= chancePercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Get the number of items that the chest will spawn(max 1 of each)
    /// </summary>
    private void GetItemsToSpawn(out int ammo, out int healt, out int weapon)
    {
        ammo = 0;
        healt = 0;
        weapon = 0;

        int numberOfItemsToSpawn = Random.Range(minNumberOfItemsToSpawn, maxNumberOfItemsToSpawn + 1);

        int choice;

        if (numberOfItemsToSpawn == 1)
        {
            choice = Random.Range(0, 3);
            if (choice == 0) { weapon++; return; }
            if (choice == 1) { ammo++; return; }
            if (choice == 2) { healt++; return; }
        }
        else if (numberOfItemsToSpawn == 2)
        {
            choice = Random.Range(0, 3);
            if (choice == 0) { weapon++; ammo++; return; }
            if (choice == 1) { ammo++; healt++; return; }
            if (choice == 2) { healt++; weapon++; return; }
        }
        else if (numberOfItemsToSpawn >= 3)
        {
            weapon++;
            ammo++;
            healt++;
            return;
        }
    }

    /// <summary>
    /// Get the quantity of ammo to spawn in percentage
    /// </summary>
    private int GetAmmoPercent(int ammoNumber)
    {
        if (ammoNumber == 0) return 0;

        foreach (RangeByLevel rangeByLevel in ammoSpawnByLevel)
        {
            if (rangeByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                return Random.Range(rangeByLevel.minSpawnChance, rangeByLevel.maxSpawnChance);
            }
        }

        return 0;
    }

    /// <summary>
    /// Get the quantity of health to spawn in percentage
    /// </summary>
    private int GetHealthPercent(int healthNumber)
    {
        if (healthNumber == 0) return 0;

        foreach (RangeByLevel rangeByLevel in healthSpawnByLevel)
        {
            if (rangeByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                return Random.Range(rangeByLevel.minSpawnChance, rangeByLevel.maxSpawnChance);
            }
        }

        return 0;
    }

    /// <summary>
    /// Get the weapon details to spawn - returns null if no weapon will spawn or if the player already has it
    /// </summary>
    private WeaponDetailsSO GetWeaponDetails(int weaponNumber)
    {
        if (weaponNumber == 0) return null;

        //Create an instance of the RandomSpawnableObject class to spawn the weapon based on it's ratio
        RandomSpawnableObject<WeaponDetailsSO> randomWeapon = new RandomSpawnableObject<WeaponDetailsSO>(weaponSpawnByLevel);

        WeaponDetailsSO weaponDetails = randomWeapon.GetItem();

        return weaponDetails;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestPrefab), chestPrefab);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minChestSpawnChance), minChestSpawnChance,
            nameof(maxChestSpawnChance), maxChestSpawnChance, true);

        if (chestSpawnChanceByLevel != null && chestSpawnChanceByLevel.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(chestSpawnChanceByLevel), chestSpawnChanceByLevel);

            foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevel)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevel), rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.minSpawnChance), rangeByLevel.minSpawnChance,
                    nameof(rangeByLevel.maxSpawnChance), rangeByLevel.maxSpawnChance, true);
            }
        }

        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minNumberOfItemsToSpawn), minNumberOfItemsToSpawn,
            nameof(maxNumberOfItemsToSpawn), maxNumberOfItemsToSpawn, true);

        if (weaponSpawnByLevel != null && weaponSpawnByLevel.Count > 0)
        {
            foreach (SpawnableObjectByLevel<WeaponDetailsSO> spawnableObject in weaponSpawnByLevel)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(spawnableObject.dungeonLevel), spawnableObject.dungeonLevel);

                foreach (SpawnableObjectRatio<WeaponDetailsSO> weaponRatio in spawnableObject.spawnableObjectRatioList)
                {
                    HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRatio.dungeonObject), weaponRatio.dungeonObject);
                    HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponRatio.ratio), weaponRatio.ratio, true);
                }
            }
        }

        if (healthSpawnByLevel != null && healthSpawnByLevel.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(healthSpawnByLevel), healthSpawnByLevel);

            foreach (RangeByLevel rangeByLevel in healthSpawnByLevel)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevel), rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.minSpawnChance), rangeByLevel.minSpawnChance, nameof(rangeByLevel.maxSpawnChance), rangeByLevel.maxSpawnChance, true);
            }
        }

        if (ammoSpawnByLevel != null && ammoSpawnByLevel.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoSpawnByLevel), ammoSpawnByLevel);

            foreach (RangeByLevel rangeByLevel in ammoSpawnByLevel)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevel), rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.minSpawnChance), rangeByLevel.minSpawnChance, nameof(rangeByLevel.maxSpawnChance), rangeByLevel.maxSpawnChance, true);
            }
        }
    }
#endif
    #endregion
}
