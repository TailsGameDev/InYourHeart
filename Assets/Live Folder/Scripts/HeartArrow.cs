using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartArrow : Bullet
{
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

    public void ApplyImpulse(float impulse)
    {
        RB2D.AddForce(transform.right * impulse, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        bool hitEnemy = col.tag == Damager.characterType.Enemy.ToString();
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
