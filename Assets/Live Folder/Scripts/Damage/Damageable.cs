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
    private System.Action onDamageOverTimeTaken;

    // private static readonly string HEAL_TAG = "Heal";

    public float CurrentLife { get => currentLife; }
    public float MaxLife { get => maxLife; }

    private void Awake()
    {
        currentLife = maxLife;
        transformWrapper = new TransformWrapper(transform);
    }
    public void RegisterOnDamageTaken(System.Action onDamaged)
    {
        onDamageTaken += onDamaged;
    }
    public void RegisterOnDamageOverTimeTaken(System.Action onDamagedOverTime)
    {
        onDamageOverTimeTaken += onDamagedOverTime;
    }
    private void OnDestroy()
    {
        onDamageTaken = null;
        onDamageOverTimeTaken = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        InstantDamager damager = col.GetComponent<InstantDamager>();
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

        DieIfLifeIsLesserThanZero();
    }
    private void DieIfLifeIsLesserThanZero()
    {
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

    private void OnTriggerStay2D(Collider2D col)
    {
        ContinuousDamager continuousDamager = col.GetComponent<ContinuousDamager>();
        if (continuousDamager != null && continuousDamager.CharacterTypeToHit.ToString() == tag)
        {
            OnDamageOverTimeTaken(continuousDamager.DamagePerSecond * Time.deltaTime);
        }
    }
    private void OnDamageOverTimeTaken(float damage)
    {
        currentLife -= damage;
        onDamageOverTimeTaken?.Invoke();
        DieIfLifeIsLesserThanZero();
    }
}
