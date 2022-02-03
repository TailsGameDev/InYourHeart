using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator = null;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private PlayerMovement playerMovement = null;
    [SerializeField] private Damageable playerDamageable = null;
    [SerializeField] private PlayerInput playerInput = null;

    private void Awake()
    {
        void PaintSpriteAccordinglyToPlayerLife()
        {
            float darkLevel = playerDamageable.CurrentLife / playerDamageable.MaxLife;
            spriteRenderer.color = new Color(darkLevel, darkLevel, darkLevel, a: 1.0f);
        }

        playerDamageable.RegisterOnDamageTaken(() =>
            { 
                PaintSpriteAccordinglyToPlayerLife(); 
                playerMovement.ModifySpeedForHitAnimation(); 
            });
        
        playerDamageable.RegisterOnDamageOverTimeTaken(PaintSpriteAccordinglyToPlayerLife);
    }

    private void Update()
    {
        // Send 0.0f to "horizontal" so it animates like idle if movement is disabled
        float animationHorizontal = playerMovement.MovementEnabled ? playerInput.HorizontalInput : 0.0f;
        playerAnimator.SetFloat("horizontal", animationHorizontal);
        
        playerAnimator.SetBool("Fire1", playerInput.IsPressingShootInput && playerMovement.MovementEnabled);
        playerAnimator.SetBool("isOnGround", playerMovement.IsOnGround);
    }
}
