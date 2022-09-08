using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyableItem : MonoBehaviour
{
    #region Header Health
    [Space(10)]
    [Header("Health")]
    #endregion

    [SerializeField] private int startingHealthAmount = 1;

    #region Header Sound Effect
    [Space(10)]
    [Header("Sound Effect")]
    #endregion

    #region Tooltip
    [Tooltip("The sound effect that will be played when this object is destroyed")]
    #endregion
    [SerializeField] private SoundEffectSO destroySoundEffect;

    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private HealthEvent healthEvent;
    private Health health;
    private ReceiveTouchDamage touchDamage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        health.SetStartingHealth(startingHealthAmount);
        touchDamage = GetComponent<ReceiveTouchDamage>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    /// <summary>
    /// Health Event Handler
    /// </summary>
    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0f)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        //Destroy the box collider
        Destroy(boxCollider2D);

        if (destroySoundEffect != null)
        {
            SoundManager.Instance.PlaySoundEffect(destroySoundEffect);
        }

        animator.SetBool(Settings.destroy, true);

        //Let the animation play through
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.destroyedState))
        {
            yield return null;
        }

        //Then destroy all components, but the sprite renderer
        Destroy(animator);
        Destroy(touchDamage);
        Destroy(health);
        Destroy(healthEvent);
        Destroy(this);
    }
}
