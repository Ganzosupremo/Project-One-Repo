using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    private ParticleSystem enemyDeathEffectParticleSystem;

    private void Awake()
    {
        enemyDeathEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Sets The Effect Devired From The Passed In Variables
    /// </summary>
    public void SetDeathEffect(EnemyDeathEffectSO enemyDeathEffect)
    {
        // Set shoot effect color gradient
        SetDeathEffectColorGradient(enemyDeathEffect.colorGradient);

        float gravityScale = Random.Range(enemyDeathEffect.minGravityEffect, enemyDeathEffect.maxGravityEffect);

        // Set shoot effect particle system starting values
        SetDeathEffectParticleStartingValues(enemyDeathEffect.particleDuration, enemyDeathEffect.startParticleSize, enemyDeathEffect.startParticleSpeed,
            enemyDeathEffect.startLifetime, gravityScale, enemyDeathEffect.maxParticles);

        // Set shoot effect particle system particle burst particle number
        SetDeathEffectParticleEmission(enemyDeathEffect.emissionRate, enemyDeathEffect.burstNumber);

        // Set shoot effect particle sprite
        SetDeathEffectParticleSprite(enemyDeathEffect.sprite);

        // Set the lifetime min and max velocities
        SetDeathEffectVelocityOverLifeTime(enemyDeathEffect.minVelocityOverLifetime, enemyDeathEffect.maxVelocityOverLifetime);
    }

    /// <summary>
    /// Set The Particle System Color Gradient
    /// </summary>
    private void SetDeathEffectColorGradient(Gradient colorGradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = enemyDeathEffectParticleSystem.colorOverLifetime;
        colorOverLifetime.color = colorGradient;
    }

    /// <summary>
    /// Set the particle system starting values
    /// </summary>
    private void SetDeathEffectParticleStartingValues(float particleDuration, float startParticleSize, float startParticleSpeed, float startLifetime, float gravityEffect, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = enemyDeathEffectParticleSystem.main;

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
    /// Set the particle system's particle burst and number of particles
    /// </summary>
    private void SetDeathEffectParticleEmission(int emissionRate, int burstNumber)
    {
        ParticleSystem.EmissionModule emissionModule = enemyDeathEffectParticleSystem.emission;

        // Set particle burst number
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstNumber);
        emissionModule.SetBurst(0, burst);

        // Set particle emission rate
        emissionModule.rateOverTime = emissionRate;
    }

    /// <summary>
    /// Set the particle system sprite
    /// </summary>
    private void SetDeathEffectParticleSprite(Sprite sprite)
    {
        // Set particle animation
        ParticleSystem.TextureSheetAnimationModule animationModule = enemyDeathEffectParticleSystem.textureSheetAnimation;

        animationModule.SetSprite(0, sprite);
    }

    /// <summary>
    /// Set the velocity over lifetime
    /// </summary>
    private void SetDeathEffectVelocityOverLifeTime(Vector3 minVelocityOverLifetime, Vector3 maxVelocityOverLifetime)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = enemyDeathEffectParticleSystem.velocityOverLifetime;

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
