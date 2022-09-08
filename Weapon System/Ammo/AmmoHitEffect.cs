using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Sets The Shoot Effect Devired From The Passed In Hit Effects
    /// </summary>
    public void SetAmmoHitEffect(AmmoHitEffectSO ammoHitEffect)
    {
        // Set shoot effect color gradient
        SetAmmoHitEffectColorGradient(ammoHitEffect.colorGradient);

        // Set shoot effect particle system starting values
        SetAmmoHitEffectParticleStartingValues(ammoHitEffect.particleDuration, ammoHitEffect.startParticleSize, ammoHitEffect.startParticleSpeed,
            ammoHitEffect.startLifetime, ammoHitEffect.gravityEffect, ammoHitEffect.maxParticles);

        // Set shoot effect particle system particle burst particle number
        SetAmmoHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstNumber);

        // Set shoot effect particle sprite
        SetAmmoHitEffectParticleSprite(ammoHitEffect.sprite);

        // Set shoot effect lifetime min and max velocities
        SetAmmoHitEffectVelocityOverLifeTime(ammoHitEffect.minVelocityOverLifetime, ammoHitEffect.maxVelocityOverLifetime);
    }

    /// <summary>
    /// Set The Shoot Effect Particle System Color Gradient
    /// </summary>
    private void SetAmmoHitEffectColorGradient(Gradient colorGradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetime.color = colorGradient;
    }

    /// <summary>
    /// Set shoot effect particle system starting values
    /// </summary>
    private void SetAmmoHitEffectParticleStartingValues(float particleDuration, float startParticleSize, float startParticleSpeed, float startLifetime, float gravityEffect, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;

        // Set particle system duration
        mainModule.duration = particleDuration;

        // Set particle start size
        mainModule.startSize = startParticleSize;

        // Set particle start speed
        mainModule.startSpeed = startParticleSpeed;

        // Set particle start lifetime
        mainModule.startLifetime = startLifetime;

        // Set particle starting gravity
        mainModule.gravityModifier = gravityEffect;

        // Set max particles
        mainModule.maxParticles = maxParticles;
    }

    /// <summary>
    /// Set shoot effect particle system particle burst particle number
    /// </summary>
    private void SetAmmoHitEffectParticleEmission(int emissionRate, int burstNumber)
    {
        ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;

        // Set particle burst number
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstNumber);
        emissionModule.SetBurst(0, burst);

        // Set particle emission rate
        emissionModule.rateOverTime = emissionRate;
    }

    /// <summary>
    /// Set shoot effect particle system sprite
    /// </summary>
    private void SetAmmoHitEffectParticleSprite(Sprite sprite)
    {
        // Set particle animation
        ParticleSystem.TextureSheetAnimationModule animationModule = ammoHitEffectParticleSystem.textureSheetAnimation;

        animationModule.SetSprite(0, sprite);
    }

    /// <summary>
    /// Set the shoot effect velocity over lifetime
    /// </summary>
    private void SetAmmoHitEffectVelocityOverLifeTime(Vector3 minVelocityOverLifetime, Vector3 maxVelocityOverLifetime)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = ammoHitEffectParticleSystem.velocityOverLifetime;

        // Define min, max X velocity
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve
        {
            mode = ParticleSystemCurveMode.TwoConstants,
            constantMin = minVelocityOverLifetime.x,
            constantMax = maxVelocityOverLifetime.x
        };
        velocityOverLifetime.x = minMaxCurveX;

        // Define min, max Y velocity
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve
        {
            mode = ParticleSystemCurveMode.TwoConstants,
            constantMin = minVelocityOverLifetime.y,
            constantMax = maxVelocityOverLifetime.y
        };
        velocityOverLifetime.y = minMaxCurveY;

        // Define min, max Z velocity
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve
        {
            mode = ParticleSystemCurveMode.TwoConstants,
            constantMin = minVelocityOverLifetime.z,
            constantMax = maxVelocityOverLifetime.z
        };
        velocityOverLifetime.z = minMaxCurveZ;
    }
}
