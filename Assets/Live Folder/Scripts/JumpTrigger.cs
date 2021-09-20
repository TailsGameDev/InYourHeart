using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    [SerializeField]
    private float upForce = 0.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D col = collision.collider;
        if (col.tag == "Player")
        {
            col.GetComponent<PlayerCollisionHandler>().ApplyJumpTriggerImpulse(Vector2.up * upForce);
        }
    }
}
