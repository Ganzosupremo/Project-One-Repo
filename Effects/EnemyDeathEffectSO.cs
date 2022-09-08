using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDeathEffect_", menuName = "Scriptable Objects/Enemy/Enemy Death Effect")]
public class EnemyDeathEffectSO : ScriptableObject
{
    #region Header Enemy Death Effect Details
    [Space(10)]
    [Header("Enemy Death Effect Details")]
    #endregion

    #region Tooltip
    [Tooltip("The color gradient for the hit effect. This gradient show the color of particles during their lifetime - from left to right")]
    #endregion Tooltip
    public Gradient colorGradient;

    #region Tooltip
    [Tooltip("The length of time the particle system is emitting particles")]
    #endregion Tooltip
    public float particleDuration = 0.5f;

    #region Tooltip
    [Tooltip("The start particle size for the particle effect")]
    #endregion Tooltip
    public float startParticleSize = 0.2f;

    #region Tooltip
    [Tooltip("The start particle speed for the particle effect")]
    #endregion Tooltip
    public float startParticleSpeed = 3f;

    #region Tooltip
    [Tooltip("The particle lifetime for the particle effect")]
    #endregion Tooltip
    public float startLifetime = 0.5f;

    #region Tooltip
    [Tooltip("The maximum number of particles to be emitted")]
    #endregion Tooltip
    public int maxParticles = 100;

    #region Tooltip
    [Tooltip("The number of particles emitted per second. If zero it will just be the burst number")]
    #endregion Tooltip
    public int emissionRate = 100;

    #region Tooltip
    [Tooltip("How many particles should be emmitted in the particle effect burst")]
    #endregion Tooltip
    public int burstNumber = 30;

    #region Tooltip
    [Tooltip("The gravity on the particles")]
    #endregion
    public float minGravityEffect = 0.01f;
    public float maxGravityEffect = 1f;

    #region Tooltip
    [Tooltip("The sprite for the particle effect.  If none is specified then the default particle sprite will be used")]
    #endregion Tooltip
    public Sprite sprite;

    #region Tooltip
    [Tooltip("The min velocity for the particle over its lifetime. A random value between min and max will be generated.")]
    #endregion Tooltip
    public Vector3 minVelocityOverLifetime;
    #region Tooltip
    [Tooltip("The max velocity for the particle over its lifetime. A random value between min and max will be generated.")]
    #endregion Tooltip
    public Vector3 maxVelocityOverLifetime;

    #region Tooltip
    [Tooltip("enemyDeathEffectPrefab contains the particle system for the death effect and is configured by the EnemyDeathEffectSO")]
    #endregion Tooltip
    public GameObject enemyDeathEffectPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(particleDuration), particleDuration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startLifetime), startLifetime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticles), maxParticles, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstNumber), burstNumber, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyDeathEffectPrefab), enemyDeathEffectPrefab);
    }

#endif
    #endregion Validation
}
