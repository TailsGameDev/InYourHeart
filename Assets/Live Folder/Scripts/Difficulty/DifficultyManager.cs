using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [System.Serializable]
    private struct Difficulties
    {
        [SerializeField] private DifficultyVariables easyDifficulty;
        [SerializeField] private DifficultyVariables mediumDifficulty;
        [SerializeField] private DifficultyVariables hardDifficulty;

        public DifficultyVariables EasyDifficulty { get => easyDifficulty; }
        public DifficultyVariables MediumDifficulty { get => mediumDifficulty; }
        public DifficultyVariables HardDifficulty { get => hardDifficulty; }
    }

    [System.Serializable]
    public struct DifficultyVariables
    {
        [SerializeField] private float officeStressShootCooldown;
        [SerializeField] private float officeStressTimeToHitTarget;

        public float OfficeStressShootCooldown { get => officeStressShootCooldown; }
        public float OfficeStressTimeToHitTarget { get => officeStressTimeToHitTarget; }
    }


    [SerializeField] private Difficulties difficulties;

    private DifficultyVariables currentDifficulty;

    private static DifficultyManager instance;

    public static DifficultyManager Instance { get => instance; set => instance = value; }
    
    public DifficultyVariables Difficulty { get => instance.currentDifficulty; }

    private void Awake()
    {
        instance = this;

        currentDifficulty = difficulties.MediumDifficulty;
    }
}
