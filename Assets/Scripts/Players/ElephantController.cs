using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class ElephantController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minMoveSpeed = 4f;
    [SerializeField] private float maxMoveSpeed = 8f;
    [SerializeField] private float obstacleRecoverySpeed = 0f;
    [SerializeField] private float accelerationRate = 1.2f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float jumpCooldown = 0.9f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Input")]
    [SerializeField] private SharedKeyboardInputRouter inputRouter;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform image;

    private Rigidbody2D rb;
    private bool canMove = true;
    private float currentMoveSpeed;
    private bool wasGrounded;
private float nextAllowedJumpTime;
    private Vector3 imageBaseScale;
    private Quaternion imageBaseRotation;

    public float CurrentMoveSpeed => currentMoveSpeed;
    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        currentMoveSpeed = minMoveSpeed;

        if (animator == null)
    animator = GetComponentInChildren<Animator>();

        if (image == null && animator != null)
            image = animator.transform;

        if (image != null)
        {
            imageBaseScale = image.localScale;
            imageBaseRotation = image.localRotation;
        }
    }

    private void Update()
{
    if (!canMove || inputRouter == null)
        return;

    bool grounded = IsGrounded();

    if (grounded && !wasGrounded)
    {
        nextAllowedJumpTime = Time.time + jumpCooldown;
    }

    wasGrounded = grounded;

    if (inputRouter.IsElephantJumpPressed() &&
        grounded &&
        Time.time >= nextAllowedJumpTime)
    {
        nextAllowedJumpTime = Time.time + jumpCooldown;

        ResetMomentum();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.35f, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (animator != null)
    animator.SetTrigger("jump");
    }

    moveInput = inputRouter.GetElephantMoveAxis();
    UpdateImageDirection(moveInput);
}

    private void FixedUpdate()
    {
        if (!canMove || inputRouter == null)
            return;


        if (Mathf.Abs(moveInput) > 0.01f)
        {
            currentMoveSpeed += accelerationRate * Time.fixedDeltaTime;
currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, obstacleRecoverySpeed, maxMoveSpeed);


        }
        else
        {
            ResetMomentum();
        }

        rb.linearVelocity = new Vector2(moveInput * currentMoveSpeed, rb.linearVelocity.y);

        if (animator != null)
    animator.SetFloat("movespeed", Mathf.Abs(rb.linearVelocity.x));
    }

    public void ResetMomentum()
    {
        currentMoveSpeed = minMoveSpeed;
    }

    public void SetInputRouter(SharedKeyboardInputRouter router)
    {
        inputRouter = router;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!canMove && rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayers
        );
    }

    public void CrashMomentum()
{
    currentMoveSpeed = obstacleRecoverySpeed;
    if (animator != null)
    animator.SetTrigger("hit");
}

public void SetReadyAnimation(bool value)
{
    if (animator != null)
        animator.SetBool("isready", value);
}

public void PlayWinAnimation()
{
    if (animator != null)
        animator.SetTrigger("win");
}

public void PlayLoseAnimation()
{
    if (animator != null)
        animator.SetTrigger("lose");
}

public void ResetAnimationForNewRound()
{
    ResetMomentum();
    moveInput = 0f;
    wasGrounded = false;
    nextAllowedJumpTime = 0f;

    if (rb != null)
        rb.linearVelocity = Vector2.zero;

    if (image != null)
    {
        image.localScale = imageBaseScale;
        image.localRotation = imageBaseRotation;
    }

    if (animator == null)
        return;

    animator.ResetTrigger("win");
    animator.ResetTrigger("lose");
    animator.ResetTrigger("jump");
    animator.ResetTrigger("hit");

    animator.SetFloat("movespeed", 0f);
    animator.SetBool("isready", false);

    animator.Play("empty", 0, 0f);
}

private void UpdateImageDirection(float direction)
{
    if (image == null || Mathf.Abs(direction) < 0.01f)
        return;

    image.localScale = new Vector3(
        Mathf.Abs(imageBaseScale.x) * (direction < 0f ? -1f : 1f),
        imageBaseScale.y,
        imageBaseScale.z
    );
}
}
