using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // Comentario homenageando apecuca

    [SerializeField]
    private PlayerMovement playerMovement = null;
    [SerializeField]
    private PlayerInput playerInput = null;
    [SerializeField]
    private SpriteRenderer spriteRenderer = null;

    [SerializeField]
    private Transform arrowSpawner = null;
    [SerializeField]
    private Transform arrowPrefab = null;

    [SerializeField]
    private float maxChargingAttackImpulse = 0.0f;
    [SerializeField]
    private float chargingSpeed = 0.0f;

    private float currentCharge;

    private void Update()
    {
        // Shooting
        if (Input.GetButtonUp("Fire1"))
        {
            Instantiate(arrowPrefab, arrowSpawner.position, arrowSpawner.rotation)
                .GetComponent<Bullet>().ApplyImpulse(playerMovement.GetVelocityMagnitude() + currentCharge);

            currentCharge = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        // Shooting
        {
            if (playerInput.IsPressingShootInput)
            {
                currentCharge = Mathf.Clamp(currentCharge + (chargingSpeed * maxChargingAttackImpulse),
                                            min: 0.0f, max: maxChargingAttackImpulse);
            }

            spriteRenderer.color = Color.Lerp(Color.white, Color.magenta, currentCharge / maxChargingAttackImpulse);
        }
    }
}
