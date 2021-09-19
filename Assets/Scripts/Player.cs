using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput = null;

    [SerializeField]
    private Animator animator = null;

    [SerializeField]
    private Rigidbody2D rb2d = null;
    
    [SerializeField]
    private float maxSqrVel = 10.0f;
    [SerializeField]
    private float acceleration = 10.0f;
    [SerializeField]
    private float stabilizationForce = 5.0f;

    private void Update()
    {
        bool isGoingHorizontal = Mathf.Abs(playerInput.GetHorizontalAxis()) > 0.1f;
        animator.SetFloat("Horizontal", playerInput.GetHorizontalAxis());
        // if (playerInput.GetHorizontalAxis() >)
        if (isGoingHorizontal)
        {
            if (rb2d.velocity.sqrMagnitude < maxSqrVel)
            {
                rb2d.AddForce(acceleration * new Vector2(playerInput.GetHorizontalAxis(), 0.0f));
            }
        }
        else
        {
            rb2d.AddForce(- new Vector2(rb2d.velocity.x, 0.0f).normalized * stabilizationForce);
        }
    }
}
