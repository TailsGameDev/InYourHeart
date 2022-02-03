using UnityEngine;

public class ParabolicBullet : Bullet
{
    private Vector3 target;
    private Vector3 spawnPosition;

    public void Initialize(float timeToHitTarget)
    {
        PlayerMovement player = PlayerMovement.Instance;

        if (player != null)
        {
            this.target = PlayerMovement.Position;

            this.spawnPosition = transform.position;

            float xVel = (target.x - spawnPosition.x) / timeToHitTarget;

            // S = s0 + v0t + gt^2/2
            // v0t = S-s0 - gt^2/2
            float yVel = ((target.y - spawnPosition.y) / timeToHitTarget)
                            - (Physics2D.gravity.y * timeToHitTarget / 2);

            RB2D.velocity = new Vector2(xVel, yVel);
        }
    }
}