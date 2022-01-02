using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeStressBullet : Bullet
{
    private Vector3 target;
    private Vector3 spawnPosition;

    private void Start()
    {
        
        PlayerMovement player = PlayerMovement.Instance;

        if (player != null)
        {
            this.target = PlayerMovement.Position;

            this.spawnPosition = transform.position;
            float timeToReachTarget = DifficultyManager.Instance.Difficulty.OfficeStressTimeToHitTarget;

            float xVel = (target.x - spawnPosition.x) / timeToReachTarget;

            // S = s0 + v0t + gt^2/2
            // v0t = S-s0 - gt^2/2
            float yVel = ((target.y - spawnPosition.y) / timeToReachTarget)
                            - (Physics2D.gravity.y * timeToReachTarget / 2);

            RB2D.velocity = new Vector2(xVel, yVel);
        }
    }
}