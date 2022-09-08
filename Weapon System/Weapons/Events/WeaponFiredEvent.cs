using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponFiredEvent : MonoBehaviour
{
    public Action<WeaponFiredEvent, WeaponFiredEventArgs> AfterWeaponFired;

    public void CallWeaponFiredEvent(Weapon weapon)
    {
        AfterWeaponFired?.Invoke(this, new WeaponFiredEventArgs() { weapon = weapon });
    }
}

public class WeaponFiredEventArgs : EventArgs
{
    public Weapon weapon;
}
