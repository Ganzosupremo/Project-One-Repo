using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        //Suscribe to the movement by velocity event
        player.movementByVelEvent.OnMovementByVelocity += MovementByVelocity_OnMovementByVelocity;

        //Suscribe to the movement by position event
        player.movementByPosEvent.OnMovementToPosition += MovementByPosition_OnMovemetnByPosition;

        //Suscribe to the idle event
        player.idleEvent.OnIdle += IdleEvent_OnIdle;

        //Suscribe to the aim weapon event
        player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        //Unsuscribe to the movement by velocity event
        player.movementByVelEvent.OnMovementByVelocity -= MovementByVelocity_OnMovementByVelocity;

        //Unsuscribe to the movement by position event
        player.movementByPosEvent.OnMovementToPosition -= MovementByPosition_OnMovemetnByPosition;

        //Unsuscribe to the idle event
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;

        //Unsuscribe to the aim weapon event
        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    /// <summary>
    /// Weapon Aim Event Handler
    /// </summary>
    /// <param name="aimWeaponEvent"></param>
    /// <param name="aimWeaponArgs"></param>
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponArgs)
    {
        InitializeAimAnimationsParameters();
        InitializeRollAnimationsParameters();
        SetAimWeaponAnimationsParameters(aimWeaponArgs.aimDirection);
    }

    /// <summary>
    /// On Movement Event Handler
    /// </summary>
    /// <param name="movementByVelEvent"></param>
    /// <param name="movementByVelArgs"></param>
    private void MovementByVelocity_OnMovementByVelocity(MovementByVelocityEvent movementByVelEvent, MovementByVelocityArgs movementByVelArgs)
    {
        InitializeRollAnimationsParameters();
        SetMovementAnimationsParameters();
    }

    /// <summary>
    /// On Movement By Position Event Handler
    /// </summary>
    /// <param name="movementByPositionsEvent"></param>
    /// <param name="movementByPositionArgs"></param>
    private void MovementByPosition_OnMovemetnByPosition(MovementByPositionEvent movementByPositionsEvent, MovementByPositionArgs movementByPositionArgs)
    {
        InitializeAimAnimationsParameters();
        InitializeRollAnimationsParameters();
        SetMovementByPositionAnimationParameters(movementByPositionArgs);
    }

    /// <summary>
    /// Idle Event Handler
    /// </summary>
    /// <param name="idleEvent"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        InitializeRollAnimationsParameters();
        SetIdleAnimationsParameters();
    }

    /// <summary>
    /// Initialize The Aim Animations Parameters For The Player
    /// </summary>
    private void InitializeAimAnimationsParameters()
    {
        player.animator.SetBool(Settings.aimUp, false);
        player.animator.SetBool(Settings.aimUpLeft, false);
        player.animator.SetBool(Settings.aimUpRight, false);
        player.animator.SetBool(Settings.aimRight, false);
        player.animator.SetBool(Settings.aimLeft, false);
        player.animator.SetBool(Settings.aimDown, false);
    }

    /// <summary>
    /// Initialize The Roll Animations Parameters
    /// </summary>
    private void InitializeRollAnimationsParameters()
    {
        player.animator.SetBool(Settings.rollDown, false);
        player.animator.SetBool(Settings.rollLeft, false);
        player.animator.SetBool(Settings.rollRight, false);
        player.animator.SetBool(Settings.rollUp, false);
    }

    /// <summary>
    /// Sets The Movement Animation Parameters 
    /// </summary>
    private void SetMovementAnimationsParameters()
    {
        player.animator.SetBool(Settings.isMoving, true);
        player.animator.SetBool(Settings.isIdle, false);
    }

    /// <summary>
    /// Set Movement By Position Animation Parameters
    /// </summary>
    /// <param name="movementByPositionArgs"></param>
    private void SetMovementByPositionAnimationParameters(MovementByPositionArgs movementByPositionArgs)
    {
        //We just animate the roll, when the player is actually rolling
        if (movementByPositionArgs.isRolling)
        {
            if (movementByPositionArgs.moveDirection.x > 0)
            {
                player.animator.SetBool(Settings.rollRight, true);
            }
            else if (movementByPositionArgs.moveDirection.x < 0)
            {
                player.animator.SetBool(Settings.rollLeft, true);
            }
            else if (movementByPositionArgs.moveDirection.y > 0)
            {
                player.animator.SetBool(Settings.rollUp, true);
            }
            else if (movementByPositionArgs.moveDirection.y < 0)
            {
                player.animator.SetBool(Settings.rollDown, true);
            }
        }
    }

    /// <summary>
    /// Sets The Idle Animations Parameters
    /// </summary>
    private void SetIdleAnimationsParameters()
    {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }

    /// <summary>
    /// Set The Aim Animations Parameters For The Player
    /// </summary>
    /// <param name="aimDirection"></param>
    private void SetAimWeaponAnimationsParameters(AimDirection aimDirection)
    {
        switch (aimDirection)
        {
            case AimDirection.Up:
                player.animator.SetBool(Settings.aimUp, true);
                break;
            
            case AimDirection.UpLeft:
                player.animator.SetBool(Settings.aimUpLeft, true);
                break;
            
            case AimDirection.UpRight:
                player.animator.SetBool(Settings.aimUpRight, true);
                break;
            
            case AimDirection.Right:
                player.animator.SetBool(Settings.aimRight, true);
                break;
            
            case AimDirection.Left:
                player.animator.SetBool(Settings.aimLeft, true);
                break;
            
            case AimDirection.Down:
                player.animator.SetBool(Settings.aimDown, true);
                break;
            
            default:
                break;
        }
    }
}
