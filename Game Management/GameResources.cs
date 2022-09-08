using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Dungeon
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("This Must Be Populated With The Dungeon RoomNodeTypeListSO")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header Player Selection Main Menu
    [Space(10)]
    [Header("Player Selection")]
    #endregion
    public GameObject playerSelectionPrefab;

    #region Header Player Details
    [Space(10)]
    [Header("Player Details")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the player details SO - This is used in the main menu to select a player")]
    #endregion
    public List<PlayerDetailsSO> playerDetailsList;

    #region Tooltip
    [Tooltip("The current player Scriptable object - this is used to reference the current player between scenes")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header Game Audio
    [Space(10)]
    [Header("Game Audio")]
    #endregion
    #region Tooltip
    [Tooltip("The master mixer group")]
    #endregion
    public AudioMixerGroup musicMasterMixerGroup;

    #region Tooltip
    [Tooltip("The music full snapshot")]
    #endregion
    public AudioMixerSnapshot musicOnFull;

    #region Tooltip
    [Tooltip("The low music snapshot")]
    #endregion
    public AudioMixerSnapshot musicOnLow;

    #region Tooltip
    [Tooltip("The music off snapshot")]
    #endregion
    public AudioMixerSnapshot musicOff;

    #region Header Sound Management
    [Space(10)]
    [Header("Sound Management")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the audio mixers master groups")]
    #endregion
    public AudioMixerGroup masterSoundsMixerGroup;
    
    #region Tooltip
    [Tooltip("Music for the main menu")]
    #endregion
    public MusicTrackSO mainMenuMusic;

    #region Tooltip
    [Tooltip("This music is played when the player presses the quit button on the main menu")]
    #endregion
    public MusicTrackSO quitMenuMusic;

    #region Tooltip
    [Tooltip("Music when the game is won")]
    #endregion
    public MusicTrackSO gameCompleteMusic;

    #region Tooltip
    [Tooltip("The sound effects for the door - will be picked randomly")]
    #endregion
    public SoundEffectSO[] openCloseDoorSoundEffect;

    #region Tooltip
    [Tooltip("The Flip sound effect for the table")]
    #endregion
    public SoundEffectSO flipTableSoundEffect;

    #region Tooltip
    [Tooltip("The open chest sound effect for the chest")]
    #endregion
    public SoundEffectSO chestOpenSound;

    #region Tooltip
    [Tooltip("The health pickup sound effect")]
    #endregion
    public SoundEffectSO healthPickupSound;

    #region Tooltip
    [Tooltip("The weapon pickup sound effect")]
    #endregion
    public SoundEffectSO weaponPickupSound;

    #region Tooltip
    [Tooltip("The ammo pickup sound effect")]
    #endregion
    public SoundEffectSO ammoPickupSound;

    #region Tooltip
    [Tooltip("The sound effect for the weapon reload sound")]
    #endregion
    public SoundEffectSO reloadSound;

    #region Tooltip
    [Tooltip("The sound effect when the player takes damage")]
    #endregion
    public SoundEffectSO afterHitInmunitySound;

    #region Tooltip
    [Tooltip("The array of the enemy hits - will be choosen randomly")]
    #endregion
    public SoundEffectSO[] enemyHitSoundArray;

    #region Tooltip
    [Tooltip("The sound effects when the player loses in a dungeon level a.k.a dies")]
    #endregion
    public SoundEffectSO[] playerDeathSoundEffects;

    #region Tooltip
    [Tooltip("The sound effects for when the player enters a chest room")]
    #endregion
    public SoundEffectSO[] chestRoomSoundEffects;

    #region Header Materials
    [Space(10)]
    [Header("Materials")]
    #endregion
    #region Tooltip
    [Tooltip("Dimmed Materials")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("The Default Sprite Lit Material")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("The Default Sprite Unlit Material")]
    #endregion
    public Material unlitMaterial;

    #region Tooltip
    [Tooltip("Populate with tha variable lit shader")]
    #endregion
    public Shader variableLitShader;

    #region Tooltip
    [Tooltip("Populate with tha Materialize Shader")]
    #endregion
    public Shader materializeShader;

    #region Header Special Tilemap Tiles
    [Space(10)]
    [Header("Special Tilemap Tiles")]
    #endregion
    #region Tooltip
    [Tooltip("The Collision tiles the enemies can't go to")]
    #endregion
    public TileBase[] enemyUnwalkableCollisionTilesArray;

    #region Tooltip
    [Tooltip("The prefered path tile for the enemy navigation")]
    #endregion
    public TileBase preferredEnemyPathTile;

    #region Header UI
    [Space(10)]
    [Header("Game UI")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the AmmoIcon prefab")]
    #endregion
    public GameObject ammoIconPrefab;

    #region Tooltip
    [Tooltip("Populate with the Score prefab")]
    #endregion
    public GameObject scorePrefab;
    
    #region Tooltip
    [Tooltip("Populate with the HeartIconUI prefab")]
    #endregion
    public GameObject heartUIPrefab;

    #region Header Chest Parameters
    [Space(10)]
    [Header("Chest Related")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the chest prefab")]
    #endregion
    public GameObject chestItemPrefab;

    #region Tooltip
    [Tooltip("Populate with the Healt Icon Sprite")]
    #endregion
    public Sprite healthIcon;

    #region Tooltip
    [Tooltip("Populate with the Bullet Icon Sprite")]
    #endregion
    public Sprite bulletSprite;

    #region Header Minimap Parameters
    [Space(10)]
    [Header("Minimap References")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the BossSkull_Minimap prefab")]
    #endregion
    public GameObject skullMinimapPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerSelectionPrefab), playerSelectionPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(scorePrefab), scorePrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(mainMenuMusic), mainMenuMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(quitMenuMusic), quitMenuMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(gameCompleteMusic), gameCompleteMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadSound), reloadSound);
        HelperUtilities.ValidateCheckNullValue(this, nameof(afterHitInmunitySound), afterHitInmunitySound);
        
        HelperUtilities.ValidateCheckNullValue(this, nameof(flipTableSoundEffect), flipTableSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestOpenSound), chestOpenSound);
        HelperUtilities.ValidateCheckNullValue(this, nameof(masterSoundsMixerGroup), masterSoundsMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(materializeShader), materializeShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);

        HelperUtilities.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnFull), musicOnFull);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnLow), musicOnLow);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOff), musicOff);

        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestItemPrefab), chestItemPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(healthIcon), healthIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(bulletSprite), bulletSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(skullMinimapPrefab), skullMinimapPrefab);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(openCloseDoorSoundEffect), openCloseDoorSoundEffect);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(playerDetailsList), playerDetailsList);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(chestRoomSoundEffects), chestRoomSoundEffects);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(playerDeathSoundEffects), playerDeathSoundEffects);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHitSoundArray), enemyHitSoundArray);
    }
#endif
    #endregion
}
