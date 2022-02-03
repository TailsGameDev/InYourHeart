using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb2d = null;

    public readonly static string TAG = "Bullet";

    public Rigidbody2D RB2D { get => rb2d; }
}
