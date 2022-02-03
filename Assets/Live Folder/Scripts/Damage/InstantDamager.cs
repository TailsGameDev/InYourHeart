using UnityEngine;

public class InstantDamager : Damager
{
    [SerializeField] private float minDamage = 0.0f;
    [SerializeField] private float maxDamage = 0.0f;

    [SerializeField] private bool destroyOnDamageDealt = true;

    [SerializeField] private AudioClip[] spawnSFXs = null;
    [SerializeField] private float minPitch = 0.0f;
    [SerializeField] private float maxPitch = 0.0f;

    public int Damage 
    {
        get 
        {
            return (int)Random.Range(minDamage, maxDamage);
        }
    }

    private void Awake()
    {
        if (spawnSFXs != null && spawnSFXs.Length > 0)
        {
            SFXPlayer.Instance.PlaySFX
                (
                    audioClip: spawnSFXs[Random.Range(0, spawnSFXs.Length)], 
                    pitch: Random.Range(minPitch, maxPitch)
                );
        }
    }

    public void OnDamageDealt()
    {
        if (destroyOnDamageDealt)
        {
            Destroy(gameObject);
        }
    }
}

public class Damager : MonoBehaviour
{
    public enum CharacterType
    {
        Enemy,
        Player,
    }

    [SerializeField]
    private CharacterType characterTypeToHit;

    public CharacterType CharacterTypeToHit { get => characterTypeToHit; }
}
