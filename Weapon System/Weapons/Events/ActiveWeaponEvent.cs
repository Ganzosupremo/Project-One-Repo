using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActiveWeaponEvent : MonoBehaviour
{
    public Action<ActiveWeaponEvent, ActiveWeaponEventArgs> OnSetActiveWeapon;

    public void CallSetActiveWeaponEvent(Weapon weapon)
    {
        OnSetActiveWeapon?.Invoke(this, new ActiveWeaponEventArgs() { playerWeapon = weapon });
    }
}

public class ActiveWeaponEventArgs : EventArgs
{
    public Weapon playerWeapon;
}
