using UnityEngine;

public class ContinuousDamager : Damager
{
    [SerializeField] private float damagePerSecond = 0.0f;

    public float DamagePerSecond { get => damagePerSecond; }
}
