using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement playerMovement = null;
    
    public void ApplyJumpTriggerImpulse(Vector2 impulse)
    {
        playerMovement.ApplyJumpTriggerImpulse(impulse);
    }
}
