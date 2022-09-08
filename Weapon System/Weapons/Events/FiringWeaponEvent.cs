using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FiringWeaponEvent : MonoBehaviour
{
    public Action<FiringWeaponEvent, FiringWeaponEventArgs> OnFireWeapon;

    public void CallOnFireWeaponEvent(bool hasFired, bool firedPreviosFrame, AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnFireWeapon?.Invoke(this, new FiringWeaponEventArgs()
        {
            hasFired = hasFired,
            firedPreviosFrame = firedPreviosFrame,
            aimDirection = aimDirection,
            aimAngle = aimAngle,
            weaponAimAngle = weaponAimAngle,
            weaponAimDirectionVector = weaponAimDirectionVector
        });
    }
}

public class FiringWeaponEventArgs : EventArgs
{
    public bool hasFired;
    public bool firedPreviosFrame;
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
}