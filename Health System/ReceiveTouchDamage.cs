using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class ReceiveTouchDamage : MonoBehaviour
{
    #region Tooltip
    [Tooltip("The amount of damage to receive")]
    #endregion
    [SerializeField] private int touchDamageAmount;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void TakeTouchDamage(int damageAmount = 0)
    {
        if (touchDamageAmount > 0)
            damageAmount = touchDamageAmount;

        health.TakeDamage(damageAmount);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(touchDamageAmount), touchDamageAmount, true);
    }
#endif
    #endregion
}
