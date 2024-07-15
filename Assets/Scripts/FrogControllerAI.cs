using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogControllerAI : MonoBehaviour
{
    public enum EFrogState
    {
        Idle,
        Jumping,
        Falling,
        Hanging,
        MovementDisabled
    }

    public bool Possesed;

    [field:SerializeField] public EFrogState CurrentState { get; private set; } = EFrogState.Falling;
    public Vector2 velocity;
    [SerializeField] BoxCollider2D collisionBox;
    [SerializeField] Transform footPosition;
    [SerializeField] FrogData movementData;

    // Animation
    [Header("Animation")]
    [SerializeField] SpriteAnimator anim;
    [SerializeField] SpriteAnimation idle, jump, fall, hang;

    //Audio
    [Header("Sound")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;

    Rigidbody2D rb;


    //Jumping
    float jumpTime = 0f;
    bool leftPressed;
    bool rightPressed;

    // Hanging
    [SerializeField] LineRenderer tounge;
    SpringJoint2D springJoint;
    
    HangPoint[] hangPoints;
    HangPoint targetedHangPoint;
    bool hangPressed;

    private void Awake()
    {
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        springJoint = gameObject.AddComponent<SpringJoint2D>();
        springJoint.enabled = false;
        springJoint.frequency = 2f;
        springJoint.enableCollision = true;
    }

    private void Start()
    {
        hangPoints = FindObjectsOfType<HangPoint>();
        transform.position = new Vector2(Random.Range(-1.7f, 1.5f), transform.position.y);
    }

    private void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if (!Possesed)
        {
            leftPressed= false;
            rightPressed= false;
            hangPressed= false;
            return;
        }
        leftPressed = Input.GetKey(KeyCode.A);
        rightPressed = Input.GetKey(KeyCode.D);
        hangPressed = Input.GetKey(KeyCode.W);
        GetClosestHangPoint();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //velocity = rb.velocity;

        HandleState();

        rb.velocity = velocity;
    }

    void HandleState()
    {
        switch (CurrentState)
        {
            case EFrogState.Idle:
                IdleUpdate();
                break;
            case EFrogState.Jumping:
                JumpUpdate();
                break;
            case EFrogState.Falling:
                FallingUpdate();
                break;
            case EFrogState.MovementDisabled:
                break;
            case EFrogState.Hanging:
                HangingUpdate();
                break;
            default:
                break;
        }
    }

    bool GetGround(out RaycastHit2D hit)
    {
        hit = new RaycastHit2D();
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, collisionBox.size, 0, Vector3.down, movementData.groundingDistance, movementData.groundLayer);
        foreach (RaycastHit2D hita in hits)
        {
            if (hita.collider.gameObject != gameObject)
            {
                hit = hita;
                break;
            }
        }
        Debug.DrawRay(footPosition.position, Vector3.down * movementData.groundingDistance);
        return hit;
    }

    bool GetGround()
    {
        return GetGround(out RaycastHit2D hit);
    }

    void FallingEnter()
    {
        CurrentState = EFrogState.Falling;

    }

    void FallingUpdate()
    {
        velocity.y -= movementData.fallAcceleration * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -movementData.maxFallSpeed, Mathf.Infinity);
        if (velocity.y < 0)
            anim.SetAnimation(fall);
        else
            anim.SetAnimation(jump);


        if (velocity.y < 0 && GetGround())
            IdleEnter();
    }

    void JumpEnter(int scale)
    {
        CurrentState = EFrogState.Jumping;
        velocity.y = movementData.verticalJumpForce;
        velocity.x = movementData.horizontalJumpForce * (float)scale;
        jumpTime = 0f;
        anim.SetAnimation(jump);
        PlaySound(jumpSound);
    }

    void JumpUpdate()
    {
        jumpTime += Time.deltaTime;
        if (jumpTime >= movementData.maxJumpTime)
            FallingEnter();
    }

    float waitTime;
    float waitedTime;
     
    void IdleEnter()
    {
        CurrentState = EFrogState.Idle;
        velocity.y = -2f;
        velocity.x = 0f;
        waitTime = Random.Range(0, 4);
        waitedTime = 0;
        anim.SetAnimation(idle);
        PlaySound(landSound);
    }


    void IdleUpdate()
    {
        waitedTime += Time.deltaTime;
        if (waitedTime < waitTime)
            return;
        int random = Random.Range(0, 2);
        if (transform.position.x > 1f)
            random = 0;
        else if (transform.position.x < -1.2f)
            random = 1;
            if (random == 0)
        {
            JumpEnter(-1);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            JumpEnter(1);
            transform.localScale = new Vector3(-1, 1, 1);
        }

    }
    void PlaySound(AudioClip clip)
    {
        if (clip == null)
            return;
        audioSource.PlayOneShot(clip);
    }

    public void Posses()
    {
        Possesed = true;
    }

    public void Unposses()
    {
        Possesed = false;
        if (targetedHangPoint != null)
            targetedHangPoint.Untarget();
    }

    void HangingEnter()
    {
        if (targetedHangPoint == null)
            return;
        targetedHangPoint.Hang();
        CurrentState = EFrogState.Hanging;
        springJoint.enabled = true;
        tounge.enabled = true;
        springJoint.autoConfigureDistance = false;
        springJoint.distance = movementData.HangDistance;
        anim.SetAnimation(hang);
    }

    void HangingUpdate()
    {
        springJoint.connectedAnchor = targetedHangPoint.transform.position;
        tounge.SetPosition(0, transform.position);
        tounge.SetPosition(1, targetedHangPoint.transform.position);

        if (leftPressed || rightPressed)
        {
            springJoint.enabled = false;
            tounge.enabled = false;
            targetedHangPoint.Release();
            FallingEnter();
        }
    }

    void GetClosestHangPoint()
    {
        if (CurrentState == EFrogState.Hanging)
            return;
        float closestDistance = Mathf.Infinity;
        HangPoint newPoint = null;
        foreach (HangPoint hangPoint in hangPoints)
        {
            // Check in range of frog
            if (Vector2.Distance(transform.position, hangPoint.transform.position) > movementData.HangRange)
                continue;

            // Check above frog
            float xOffset = Mathf.Abs(transform.position.x - hangPoint.transform.position.x);
            if (xOffset > movementData.MaxXOffset)
                continue;

            float checkingDistance = Vector2.Distance(Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition), hangPoint.transform.position);
            if (checkingDistance < closestDistance)
            {
                // Check line of sight of frog
                Vector2 pointVector = hangPoint.transform.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, pointVector, pointVector.magnitude, LayerMask.GetMask("Ground"));
                if (hit)
                    continue;
                closestDistance = checkingDistance;
                newPoint = hangPoint;
            }

        }
        if (newPoint != targetedHangPoint)
        {
            if(newPoint != null)
                newPoint.Target();
            if(targetedHangPoint != null)
                targetedHangPoint.Untarget();
        }
        targetedHangPoint = newPoint;
    }
}
