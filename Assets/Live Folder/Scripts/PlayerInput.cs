using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Input
    private float horizontalInput;
    private bool isPressingShootInput;
    private bool isPressingJumpInput;

    public float HorizontalInput { get => horizontalInput; set => horizontalInput = value; }
    public bool IsPressingShootInput { get => isPressingShootInput; set => isPressingShootInput = value; }
    public bool IsPressingJumpInput { get => isPressingJumpInput; set => isPressingJumpInput = value; }

    private void Update()
    {
        this.horizontalInput = Input.GetAxisRaw("Horizontal");
        this.isPressingJumpInput = Input.GetButton("Jump");
        this.isPressingShootInput = Input.GetButton("Fire1");
    }
}
