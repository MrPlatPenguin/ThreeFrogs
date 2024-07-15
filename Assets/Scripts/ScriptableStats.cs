using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableStats : ScriptableObject
{
    public float TestValue = 0;
    [Header("LAYERS")]
    [Tooltip("Set this to the layer your player is on.")]
    public LayerMask CollisionLayers;
    public LayerMask GroundLayer;
    public LayerMask DangerLayer;

    [Header("INPUT")]
    [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
    public bool SnapInput = true;

    [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers."), Range(0.01f, 0.99f)]
    public float VerticalDeadZoneThreshold = 0.3f;

    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers."), Range(0.01f, 0.99f)]
    public float HorizontalDeadZoneThreshold = 0.1f;

    [Header("MOVEMENT")]
    [Tooltip("The top horizontal movement speed.")]
    public float MaxSpeed = 14;

    [Tooltip("The player's capacity to gain horizontal speed.")]
    public float Acceleration = 120;

    [Tooltip("The pace at which the player comes to a stop.")]
    public float GroundDeceleration = 60;

    [Tooltip("Deceleration in air only after stopping input mid-air.")]
    public float AirDeceleration = 30;

    [Tooltip("A constant downward force applied while grounded. Helps on slopes."), Range(0f, -10f)]
    public float GroundingForce = -1.5f;

    [Tooltip("The detection distance for grounding and roof detection."), Range(0f, 0.5f)]
    public float GrounderDistance = 0.05f;

    [Tooltip("When the player is moving at a speed greater than the max speed * this value, they will not be able to adjust their x movement.")]
    public float MaxSpeedControlRegain = 2f;

    public float MaxSlopeIncline = 45f;

    [Header("JUMP")]
    [Tooltip("The immediate velocity applied when jumping.")]
    public float JumpPower = 36;

    public int JumpHoldFrameLimit = 7;

    [Tooltip("The maximum vertical movement speed.")]
    public float MaxFallSpeed = 40;

    [Tooltip("The maximum vertical movement speed while holding down.")]
    public float MaxFastFallSpeed = 50;

    [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity.")]
    public float FallAcceleration = 110;

    [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity.")]
    public float FastFallAcceleration = 110;

    [Tooltip("The gravity multiplier added when jump is released early.")]
    public float JumpEndEarlyGravityModifier = 3;

    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge.")]
    public int CoyoteTime = 5;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground.")]
    public int JumpBuffer = 10;

    [Header("Grapple")]
    //Hook Stuff
    [Tooltip("The blocking layers for the grapple raycast.")]
    public LayerMask GrappleTargets;
    [Tooltip("The maximum distance that the hook looks for a surface for.")]
    public float MaximumGrappleDistance = Mathf.Infinity;
    public int GrappleBufferFrames = 10;

    // Pull Stuff
    [Header("Pull")]
    [Tooltip("The strength of the grapple pull.")]
    public float GrapplePullStrength = 1f;
    [Tooltip("The maximum speed that the player travels at while grappling.")]
    public float MaximumGrapplePullSpeed = 50f;
    [Tooltip("How quickly the elastic of the grapple reaches a rest.")]
    public float GrapplePullResistance = 0.1f;
    public float GrapplePullJumpPower = 10f;

    // Swing Stuff
    [Header("Swing")]
    public float GrappleSwingLengthAdjustSpeed = 1;
    [Tooltip("The strength of gravity affecting the player while grappled.")]
    public float GrappleSwingGravity = 0.125f;
    [Tooltip("How much force is lost over time (Air resistance).")]
    public float GrappleSwingResistance = 0.1f;
    [Tooltip("The maximum speed of the grapple swing is not adding input.")]
    public float MaximumSwingSpeed = 40f;
    [Tooltip("The maximum force the player adds to a swing with manual input on a swing.")]
    public float GrappleFastSwingStrength = 0.01f;
    [Tooltip("The speed at which the players input reaches full strength.")]
    public float GrappleFastSwingAcceleration = 0.5f;
    [Tooltip("The maximum speed of the grapple swing when the player is pushing with the swing.")]
    public float MaximumFastSwingSpeed = 50f;
    [Tooltip("The amount of frames the player is paused for before they start grappling.")]
    public int GrappleAttachPause = 8;
    [Tooltip("The speed the grapple goes towards the hook.")]
    public float GrappleAttatchSpeed = 150f;
    public float MaxGrappleExitSpeed = 10f;
    public float SwingEnterMomentumTransfer = 0.25f;
    public float GrappleExitBoost = 2f;
    public float GrappleSwingJumpPower = 10f;

    [Header("Ledge Detection")]
    public float LedgeDetectionXDistance = 1f;
    public float LedgeDetectionYDistance = 1f;
    public float LedgeHangHeight = 1f;
    public float LedgeHangDistance = 1f;
    public float LedgeJumpPower = 20f;
}