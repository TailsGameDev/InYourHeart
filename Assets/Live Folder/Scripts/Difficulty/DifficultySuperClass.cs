using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySuperClass : MonoBehaviour
{
    protected enum DifficultyLevel
    {
        EASY = 0,
        MEDIUM = 1,
        HARD = 2,
    }

    private DifficultyLevel currentDifficultyLevel;

    protected DifficultyLevel CurrentDifficultyLevel { get => currentDifficultyLevel; }

    private void Awake()
    {
        const DifficultyLevel DEFAULT_DIFFICULTY_LEVEL = DifficultyLevel.EASY;
        this.currentDifficultyLevel = DEFAULT_DIFFICULTY_LEVEL;
    }
}
