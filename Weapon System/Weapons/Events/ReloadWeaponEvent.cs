using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ReloadWeaponEvent : MonoBehaviour
{
    public Action<ReloadWeaponEvent, ReloadWeaponEventArgs> OnReloadWeapon;

    public void CallReloadWeaponEvent(Weapon weapon, int topUpAmmoPorcent)
    {
        OnReloadWeapon?.Invoke(this, new ReloadWeaponEventArgs()
        {
            weapon = weapon,
            topUpAmmoPorcent = topUpAmmoPorcent
        });

    }
}

public class ReloadWeaponEventArgs : EventArgs
{
    public Weapon weapon;
    public int topUpAmmoPorcent;
}