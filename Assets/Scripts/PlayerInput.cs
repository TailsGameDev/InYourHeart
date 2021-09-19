using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float GetHorizontalAxis()
    {
        return Input.GetAxisRaw("Horizontal");
    }
}
