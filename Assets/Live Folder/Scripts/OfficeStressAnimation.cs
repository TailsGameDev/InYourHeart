using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeStressAnimation : MonoBehaviour
{
    [SerializeField] private Damageable damageable = null;
    [SerializeField] private float damageAnimationTime = 0.0f;
    private bool isDoingDamageAnimation;
    private float timeToEndDamageAnimation;

    [SerializeField] private Animator animator = null;
    private float originalAnimatorSpeed;

    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Color damagedColor;
    private Color originalColor;

    private void Awake()
    {
        damageable.RegisterOnDamageTaken(OnDamageTaken);
        
        originalAnimatorSpeed = animator.speed;

        originalColor = spriteRenderer.color;
    }

    private void OnDamageTaken()
    {
        isDoingDamageAnimation = true;
        timeToEndDamageAnimation = Time.time + damageAnimationTime;
        
        animator.speed = 0.0f;

        spriteRenderer.color = damagedColor;
    }

    private void Update()
    {
        if (isDoingDamageAnimation && Time.time > timeToEndDamageAnimation)
        {
            isDoingDamageAnimation = false;
            animator.speed = originalAnimatorSpeed;

            spriteRenderer.color = originalColor;
        }
    }
}
