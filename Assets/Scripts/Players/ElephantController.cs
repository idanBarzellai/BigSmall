using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class ElephantController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minMoveSpeed = 4f;
    [SerializeField] private float maxMoveSpeed = 8f;
    [SerializeField] private float accelerationRate = 1.2f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float jumpCooldown = 0.9f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Input")]
    [SerializeField] private SharedKeyboardInputRouter inputRouter;

    private Rigidbody2D rb;
    private bool canMove = true;
    private float currentMoveSpeed;
    private float lastJumpTime = -999f;
    private bool wasGrounded;
private float nextAllowedJumpTime;

    public float CurrentMoveSpeed => currentMoveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        currentMoveSpeed = minMoveSpeed;
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
    }
}

    private void FixedUpdate()
    {
        if (!canMove || inputRouter == null)
            return;

        float moveInput = inputRouter.GetElephantMoveAxis();

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            currentMoveSpeed += accelerationRate * Time.fixedDeltaTime;
            currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, minMoveSpeed, maxMoveSpeed);
        }
        else
        {
            ResetMomentum();
        }

        rb.linearVelocity = new Vector2(moveInput * currentMoveSpeed, rb.linearVelocity.y);
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
}