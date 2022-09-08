using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        enemy.movementByPositionEvent.OnMovementToPosition += MovementToPosition_OnMovementToPosition;

        enemy.idleEvent.OnIdle += IdleEvent_OnIdle;

        enemy.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        enemy.movementByPositionEvent.OnMovementToPosition -= MovementToPosition_OnMovementToPosition;

        enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;

        enemy.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    /// <summary>
    /// Firing weapon event handler
    /// </summary>
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitialiseAimAnimationParameters();
        SetWeaponAimAnimationParameters(aimWeaponEventArgs.aimDirection);
    }

    /// <summary>
    /// Event Handler
    /// </summary>
    private void MovementToPosition_OnMovementToPosition(MovementByPositionEvent movementByPositionEvent, MovementByPositionArgs movementByPositionArgs)
    {
        SetMovementAnimationParameters();
    }

    /// <summary>
    /// Event Handler
    /// </summary>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        SetIdleAnimationParameters();
    }

    /// <summary>
    /// Initialise the animation parameters for the aim
    /// </summary>
    private void InitialiseAimAnimationParameters()
    {
        enemy.animator.SetBool(Settings.aimUp, false);
        enemy.animator.SetBool(Settings.aimUpRight, false);
        enemy.animator.SetBool(Settings.aimUpLeft, false);
        enemy.animator.SetBool(Settings.aimRight, false);
        enemy.animator.SetBool(Settings.aimLeft, false);
        enemy.animator.SetBool(Settings.aimDown, false);
    }

    /// <summary>
    /// Set movement animation parameters
    /// </summary>
    private void SetMovementAnimationParameters()
    {
        enemy.animator.SetBool(Settings.isIdle, false);
        enemy.animator.SetBool(Settings.isMoving, true);
    }

    /// <summary>
    /// Set idle animation parameters
    /// </summary>
    private void SetIdleAnimationParameters()
    {
        enemy.animator.SetBool(Settings.isMoving, false);
        enemy.animator.SetBool(Settings.isIdle, true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetWeaponAimAnimationParameters(AimDirection aimDIrection)
    {
        switch (aimDIrection)
        {
            case AimDirection.Up:
                enemy.animator.SetBool(Settings.aimUp, true);
                break;
            case AimDirection.UpLeft:
                enemy.animator.SetBool(Settings.aimUpLeft, true);
                break;
            case AimDirection.UpRight:
                enemy.animator.SetBool(Settings.aimUpRight, true);
                break;
            case AimDirection.Right:
                enemy.animator.SetBool(Settings.aimRight, true);
                break;
            case AimDirection.Left:
                enemy.animator.SetBool(Settings.aimLeft, true);
                break;
            case AimDirection.Down:
                enemy.animator.SetBool(Settings.aimDown, true);
                break;
            default:
                break;
        }
    }
}
