using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyEvent : MonoBehaviour
{
    public Action<DestroyEvent, DestroyEventArgs> OnDestroy;

    public void CallOnDestroyEvent(bool playerDied,int points)
    {
        OnDestroy?.Invoke(this, new DestroyEventArgs()
        {
            playerDeath = playerDied,
            points = points
        });
    }
}

public class DestroyEventArgs : EventArgs
{
    public bool playerDeath;
    public int points;
}
