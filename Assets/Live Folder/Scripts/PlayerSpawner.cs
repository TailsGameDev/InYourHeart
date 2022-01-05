using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : PlayerSpawnSystem
{
    [SerializeField] private Transform playerPrefab = null;

    [SerializeField] private Transform firstCheckpointInTheLevel = null;

    private float timeToRespawn = 1.5f;

    private void Awake()
    {
        if (CurrentSpawnPoint == null)
        {
            CurrentSpawnPoint = firstCheckpointInTheLevel;
        }

        Instantiate(playerPrefab, CurrentSpawnPoint.position, CurrentSpawnPoint.rotation);
    }

    private void Update()
    {
        if (PlayerMovement.Instance == null)
        {
            timeToRespawn -= Time.deltaTime;
            if (timeToRespawn < 0.0f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}

public class PlayerSpawnSystem : MonoBehaviour
{
    private static Transform currentSpawnPoint = null;

    protected static Transform CurrentSpawnPoint 
        { get => currentSpawnPoint; set => currentSpawnPoint = value; }
}