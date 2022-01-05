using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    private float maxLife = 0.0f;

    // [SerializeField]
    // private TakenDamageVFX takenDamageVFX;
    // [SerializeField]
    // private TakenDamageVFX healVFX;

    [SerializeField]
    private Transform deadBody = null;

    private float currentLife;
    private TransformWrapper transformWrapper;

    private System.Action onDamageTaken;

    // private static readonly string HEAL_TAG = "Heal";

    public float CurrentLife { get => currentLife; }
    public float MaxLife { get => maxLife; }

    private void Awake()
    {
        currentLife = maxLife;
        transformWrapper = new TransformWrapper(transform);
    }

    private void OnDestroy()
    {
        onDamageTaken = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Damager damager = damager = col.GetComponent<Damager>();
        if (damager != null)
        {
            float damage = damager.Damage;
            if (damage >= 0)
            {
                if (damager.CharacterTypeToHit.ToString() == tag)
                {
                    damager.OnDamageDealt();
                    OnDamageTaken(damager.Damage);
                }
            }
            else
            {
                damager.OnDamageDealt();
                OnHeal(damage);
            }
        }
    }
    public void OnDamageTaken(float damage)
    {
        currentLife -= damage;

        // TakenDamageVFX vfx = Instantiate(takenDamageVFX, 
        //         transformWrapper.Position, Quaternion.identity);
        // vfx.Text.text = damage.ToString();

        onDamageTaken?.Invoke();

        if (currentLife <= 0.0f)
        {
            if (deadBody != null)
            {
                Instantiate(deadBody, transformWrapper.Position, transformWrapper.Rotation);
            }
            Destroy(gameObject);
        }
    }
    public void OnHeal(float healAmount)
    {
        currentLife = Mathf.Clamp( currentLife + healAmount, min: 0.0f, max: maxLife );

        // TakenDamageVFX vfx = Instantiate(healVFX,
        //         transformWrapper.Position, Quaternion.identity);
        // vfx.Text.text = healAmount.ToString();
    }

    public void RegisterOnDamageTaken(System.Action onDamaged)
    {
        onDamageTaken += onDamaged;
    }
}
