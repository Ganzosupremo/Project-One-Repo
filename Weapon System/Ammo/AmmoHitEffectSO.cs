using UnityEngine;

[CreateAssetMenu(fileName = "AmmoHitEffect_", menuName = "Scriptable Objects/Weapon System/Ammo Hit Effects")]
public class AmmoHitEffectSO : ScriptableObject
{
    #region Header Bullet Hit Effect Details
    [Space(10)]
    [Header("Bullet Hit Effect Details")]
    #endregion

    #region Tooltip
    [Tooltip("The color gradient for the hit effect.  This gradient show the color of particles during their lifetime - from left to right ")]
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
    [Tooltip("The gravity on the particles - a small negative number will make them float up")]
    #endregion
    public float gravityEffect = -0.01f;

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
    [Tooltip("ammoHitEffectPrefab contains the particle system for the shoot effect - and is configured by the weaponShootEffect SO")]
    #endregion Tooltip
    public GameObject bulletHitEffectPrefab;

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
        HelperUtilities.ValidateCheckNullValue(this, nameof(bulletHitEffectPrefab), bulletHitEffectPrefab);
    }

#endif

    #endregion Validation
}
