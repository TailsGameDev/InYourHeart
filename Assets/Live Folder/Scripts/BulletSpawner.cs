using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private ShooterEnemyDifficulty difficulty = null;
    [SerializeField] private ParabolicBullet bulletToSpawn = null;
    [SerializeField] private float maxSqrDistanceFromTarget = 0.0f;

    private float timeToNextShot;

    private void Start()
    {
        this.timeToNextShot = Time.time + difficulty.GetShootCooldown();
    }

    private void Update()
    {
        // TODO: delegate on player death
        if (PlayerMovement.Instance == null)
        {
            enabled = false;
        }
        else if (timeToNextShot < Time.time 
            && maxSqrDistanceFromTarget > Vector3.SqrMagnitude(PlayerMovement.Position - transform.position))
        {
            timeToNextShot = Time.time + difficulty.GetShootCooldown();

            Instantiate(bulletToSpawn, transform.position, transform.rotation)
                                    .Initialize(difficulty.GetBulletTimeToHitTarget());
        }
    }
}
