using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bulletToSpawn = null;

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
        if (timeToNextShoot < Time.time)
        {
            timeToNextShoot = Time.time + shootCooldown;

            Instantiate(bulletToSpawn, transform.position, transform.rotation);
        }
    }
}
