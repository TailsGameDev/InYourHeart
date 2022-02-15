using UnityEngine;

public interface Arrow 
{
    public void OnSpawned(Vector3 playerVelocity, ZeroToOneFloat currentCharge);
}

public class HeartArrow : Bullet, Arrow
{
    [SerializeField]
    private InstantDamager instantDamager = null;
    [SerializeField] private GameObject vfx = null;
    [SerializeField] private float speed = 0.0f;
    [SerializeField] private float maxChargeScaleMultiplier = 0.0f;

    private bool isAttachedToObject;
    private Collider2D objectAttachedTo;

    private bool shouldRotateWithSpeed = true;

    private Vector3 objectAttachedToOriginalPosition;
    private Vector3 positionAtAttachmentToObject;

    private void Update()
    {
        if (shouldRotateWithSpeed)
        {
            transform.right = RB2D.velocity;
        }

        if (isAttachedToObject && objectAttachedTo == null)
        {
            Destroy(gameObject);
        }

        if (isAttachedToObject)
        {
            if (objectAttachedTo == null)
            {
                // If the object this arrow is attached to gets destroyed
                Destroy(gameObject);
            }
            else
            {
                // Apply difference in attached object position also to this object
                Vector3 deltaPosition = objectAttachedTo.transform.position - objectAttachedToOriginalPosition;
                transform.position = positionAtAttachmentToObject + deltaPosition;
            }
        }
    }

    public void OnSpawned(Vector3 playerVelocity, ZeroToOneFloat currentCharge)
    {
        RB2D.AddForce(transform.right * (speed/* + playerVelocity.x*/) , ForceMode2D.Impulse);
        
        // VFX
        bool isMaxCharge = currentCharge.Value >= 1.0f;
        vfx.SetActive(isMaxCharge);
        if (isMaxCharge)
        {
            transform.localScale *= maxChargeScaleMultiplier;
        }

        instantDamager.RegisterCalgulateDamage(() =>
            {
                float flexibleSliceOfTheDamage = instantDamager.MaxDamage - instantDamager.MinDamage;
                return (int) (instantDamager.MinDamage + (flexibleSliceOfTheDamage * currentCharge.Value));
            } );
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        bool hitEnemy = col.tag == Damager.CharacterType.Enemy.ToString();
        if (hitEnemy || col.tag == "scenario")
        {
            this.shouldRotateWithSpeed = false;
            transform.right = RB2D.velocity;
            RB2D.velocity = Vector3.zero;
            RB2D.isKinematic = true;
            RB2D.gravityScale = 0.0f;       

            objectAttachedTo = col;
            isAttachedToObject = true;
            objectAttachedToOriginalPosition = objectAttachedTo.transform.position;
            positionAtAttachmentToObject = transform.position;
        }
    }
}
