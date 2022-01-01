using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartArrow : Bullet
{
    private void Update()
    {
        transform.right = RB2D.velocity;
    }

    public void ApplyImpulse(float impulse)
    {
        RB2D.AddForce(transform.right * impulse, ForceMode2D.Impulse);
    }
}
