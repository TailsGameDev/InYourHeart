using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bulletToSpawn = null;
    [SerializeField] private float maxSqrDistanceFromTarget = 0.0f;

    private float timeToNextShoot;
    private float shootCooldown;

    private void Start()
    {
        this.shootCooldown = DifficultyManager.Instance.Difficulty.OfficeStressShootCooldown;
        this.timeToNextShoot = Time.time + shootCooldown;
    }

    /*
    public void Initialize(float shootCooldown)
    {
        this.shootCooldown = shootCooldown;
        this.timeToNextShoot = Time.time + shootCooldown;
    }
    */

    private void Update()
    {
        // TODO: delegate on player death
        if (PlayerMovement.Instance == null)
        {
            enabled = false;
        }
        else if (timeToNextShoot < Time.time 
            && maxSqrDistanceFromTarget > Vector3.SqrMagnitude(PlayerMovement.Position - transform.position))
        {
            timeToNextShoot = Time.time + shootCooldown;

            Instantiate(bulletToSpawn, transform.position, transform.rotation);
        }
    }
}
