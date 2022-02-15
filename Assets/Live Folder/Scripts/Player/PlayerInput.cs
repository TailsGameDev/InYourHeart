using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Input
    private float horizontalInput;
    private bool isPressingShootInput;
    private bool isPressingJumpInput;

    private static PlayerInput instance;

    public float HorizontalInput { get => horizontalInput; set => horizontalInput = value; }
    public bool IsPressingShootInput { get => isPressingShootInput; set => isPressingShootInput = value; }
    public bool IsPressingJumpInput { get => isPressingJumpInput; set => isPressingJumpInput = value; }
    public static PlayerInput Instance { get => instance; }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        this.horizontalInput = Input.GetAxisRaw("Horizontal");
        this.isPressingJumpInput = Input.GetButton("Jump");
        this.isPressingShootInput = Input.GetButton("Fire1");
    }

    public bool GetJumpButtonDown()
    {
        return Input.GetButtonDown("Jump");
    }

    public bool GetSkipMessageButtonDown()
    {
        return Input.GetButtonDown("Submit");
    }

    public bool GetFireButtonUp()
    {
        return Input.GetButtonUp("Fire1");
    }
}
