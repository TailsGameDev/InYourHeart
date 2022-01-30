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
        if (!IsCurrentSpawnPointValid)
        {
            CurrentSpawnPoint = firstCheckpointInTheLevel.position;
            IsCurrentSpawnPointValid = true;
        }
        Instantiate(playerPrefab, CurrentSpawnPoint, Quaternion.identity);
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
    private static bool isCurrentSpawnPointValid;
    private static Vector3 currentSpawnPoint;

    protected static Vector3 CurrentSpawnPoint 
        { get => currentSpawnPoint; set => currentSpawnPoint = value; }
    public static bool IsCurrentSpawnPointValid 
        { get => isCurrentSpawnPointValid; set => isCurrentSpawnPointValid = value; }
}