using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Frog Data", menuName = "Frog Data")]
public class FrogData : ScriptableObject
{
    public float groundingDistance = 1f;
    public LayerMask groundLayer;
    public float fallAcceleration = 10f;
    public float maxFallSpeed = 10f;
    public float verticalJumpForce = 10f;
    public float horizontalJumpForce = 10f;
    public float maxJumpTime = 0.5f;

    public float HangDistance = 0.1f;
    public float HangRange = 0.1f;
    public float MaxXOffset = 0.5f;
}
