using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private SpriteRenderer chargingEffectSpriteRenderer = null;
    [SerializeField] private ParticleSystem chargingParticleSystem = null;

    [SerializeField] private PlayerMovement playerMovement = null;
    [SerializeField] private PlayerInput playerInput = null;

    [SerializeField] private Transform arrowSpawner = null;
    [SerializeField] private Transform arrowPrefab = null;

    [SerializeField] private float chargingSpeed = 0.0f;
    private const float MAX_CHARGE = 1.0f;

    [SerializeField] private float alphaOscilationAmplitudeMultiplier = 0.0f;
    [SerializeField] private float alphaOscilationSpeedMultiplier = 0.0f;

    private float currentCharge;

    private float t;

    private void Update()
    {
        if (playerInput.GetFireButtonUp())
        {
            InstantiateArrowWithMaxSpeed();
            currentCharge = 0.0f;
        }
    }
    private void InstantiateArrowWithMaxSpeed()
    {
        Instantiate(arrowPrefab, arrowSpawner.position, arrowSpawner.rotation).GetComponent<Arrow>().OnSpawned(
                        playerVelocity: PlayerMovement.Instance.GetVelocity(),
                        currentCharge: new ZeroToOneFloat(currentCharge));
    }

    private void FixedUpdate()
    {
        // Charge Logic
        if (playerInput.IsPressingShootInput)
        {
            currentCharge = Mathf.Clamp(currentCharge + (chargingSpeed * MAX_CHARGE),
                                        min: 0.0f, max: MAX_CHARGE);
        }

        // VFX Logic
        {
            Color chargingColor = chargingEffectSpriteRenderer.color;
            if (playerInput.IsPressingShootInput)
            {
                currentCharge = Mathf.Clamp(currentCharge + (chargingSpeed * MAX_CHARGE),
                                            min: 0.0f, max: MAX_CHARGE);
            
                float targetAlpha = currentCharge / MAX_CHARGE;
                float alphaWithEffect = Mathf.Clamp01(targetAlpha + (Mathf.Sin(t * alphaOscilationSpeedMultiplier) * alphaOscilationAmplitudeMultiplier) - alphaOscilationAmplitudeMultiplier);
                t += Time.deltaTime;

                chargingColor.a = alphaWithEffect;
            }
            else
            {
                chargingColor.a = 0.0f;
            }

            if (playerInput.IsPressingShootInput != chargingParticleSystem.enableEmission)
            {
                chargingParticleSystem.enableEmission = playerInput.IsPressingShootInput;
            }

            chargingEffectSpriteRenderer.color = chargingColor;
            // chargingEffectSpriteRenderer.color = Color.Lerp(Color.white, Color.magenta, currentCharge / maxChargingAttackImpulse);
        }
    }
}

public class ZeroToOneFloat
{
    private float value;

    public ZeroToOneFloat(float value)
    {
        this.value = value;

        if (value > 1.0f || value < 0.0f)
        {
            throw new System.Exception("value should be betweeen zero and one");
        }
    }

    public float Value { get => value; }
}