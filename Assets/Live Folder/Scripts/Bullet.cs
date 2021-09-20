using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.0f;

    [SerializeField]
    private Rigidbody2D rb2d = null;

    private void Awake()
    {
        rb2d.AddForce(transform.right * speed, ForceMode2D.Impulse);
    }

    private void Update()
    {
        transform.right = rb2d.velocity;
    }

    public void ApplyImpulse(float impulse)
    {
        rb2d.AddForce(transform.right * impulse, ForceMode2D.Impulse);
    }
}
