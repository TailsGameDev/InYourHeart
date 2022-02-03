using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornBullet : Bullet
{
    private void Update()
    {
        transform.up = RB2D.velocity;
    }
}
