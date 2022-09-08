using System;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region UNITS
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;
    #endregion

    #region DUNGEON BUILD SETTINGS
    public const int maxDungeonRebuildAttemptsForNodeGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region ROOM SETTINGS
    public const float fadeInTime = 0.5f; //Time the rooms and doors take to fade in
    public const int maxChildCorridors = 4; //Max number of child corridors leading from a room - max should be 3 although
                                            //this is not recommended since it can cause the dungeon building to fail, since the room are more likely to not fit together
    public const float doorUnlockDelay = 1.5f;   
    #endregion

    #region ANIMATOR PARAMETERS
    //Animation parameters for the player
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollDown = Animator.StringToHash("rollDown");
    
    public static float playerAnimationSpeed = 8f;
    public static float enemyAnimationSpeed = 3f;

    //Animation parameters for the door
    public static int openDoor = Animator.StringToHash("open");

    //Animation parameter for the destroyable items
    public static int destroy = Animator.StringToHash("destroy");
    public static string destroyedState = "Destroyed";

    //Animation parameters for the table
    public static int flipUp = Animator.StringToHash("flipUp");
    public static int flipRight = Animator.StringToHash("flipRight");
    public static int flipLeft = Animator.StringToHash("flipLeft");
    public static int flipDown = Animator.StringToHash("flipDown");

    //Animation parameters for the chest
    public static int used = Animator.StringToHash("use");
    #endregion

    #region GAMEOBJECTS TAGS
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    #endregion

    #region GAME AUDIO
    public const float musicFadeInTime = 0.8f;
    public const float musicFadeOutTiem = 0.8f;
    #endregion

    #region FIRING CONTROL
    public const float useAimAngleDistance = 3.5f; // If the target distance is less than this value, then the aim angle
                                                   // value will be used, this is calculated from the player, else the weapon aim angle will be used, this is calculated from the weapon fire position.
    #endregion

    #region A STAR PATHFINDING PARAMETERS
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const int targetFramesToSpreadPathfindingOver = 60;

    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;
    #endregion

    #region ENEMY PARAMETERS
    public const int defaultEnemyHealth = 20;
    #endregion

    #region UI PARAMETERS
    public const float uiSpacingForAmmoIcoin = 4f;
    public const float uiHeartSpacing = 16f;
    #endregion

    #region TOUCH DAMAGE SETTINGS
    public const float cooldownBtwTouchDamage = 0.5f;
    #endregion

    #region High Scores
    public const int maxNumberOfHighScoresToSave = 100;
    #endregion
}
