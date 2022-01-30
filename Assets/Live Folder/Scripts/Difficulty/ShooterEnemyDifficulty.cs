using UnityEngine;

public class ShooterEnemyDifficulty : DifficultySuperClass
{
    [System.Serializable]
    private struct Difficulty
    {
        [SerializeField] private DifficultyLevel difficultyLevel;
        [SerializeField] private float shootCooldown;
        [SerializeField] private float timeBulletTakesToHitTarget;

        public float ShootCooldown { get => shootCooldown; }
        public float TimeBulletTakesToHitTarget { get => timeBulletTakesToHitTarget; }
        public DifficultyLevel DifficultyLevel { get => difficultyLevel; }
    }

    [SerializeField] private Difficulty[] difficulties;

    public float GetShootCooldown()
    {
        return GetCurrentDifficulty().ShootCooldown;
    }
    public float GetBulletTimeToHitTarget()
    {
        return GetCurrentDifficulty().TimeBulletTakesToHitTarget;
    }

    private Difficulty GetCurrentDifficulty()
    {
        Difficulty difficulty = difficulties[0];
        for (int d = 0; d < difficulties.Length; d++)
        {
            if (difficulties[d].DifficultyLevel == CurrentDifficultyLevel)
            {
                difficulty = difficulties[d];
                break;
            }
        }
        return difficulty;
    }
}
