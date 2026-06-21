using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class MouseController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private SharedKeyboardInputRouter inputRouter;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform image;
    [SerializeField] private float birdCallLockDuration = 2f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool canMove = true;
    private float birdCallLockUntil;
    private Vector3 mouseBaseScale;
    private Quaternion mouseBaseRotation;
    private Vector3 imageBaseScale;
    private Quaternion imageBaseRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        mouseBaseScale = transform.localScale;
        mouseBaseRotation = transform.localRotation;

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
        if (!CanMoveNow() || inputRouter == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = inputRouter.GetMouseMoveInput();
        UpdateMouseDirection(moveInput);
    }

    private void FixedUpdate()
    {
        if (!CanMoveNow())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;

        if (animator != null)
    animator.SetFloat("movespeed", rb.linearVelocity.magnitude);
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

public void PlayBirdCallAnimation()
{
    birdCallLockUntil = Mathf.Max(
        birdCallLockUntil,
        Time.time + birdCallLockDuration
    );

    moveInput = Vector2.zero;

    transform.localRotation = mouseBaseRotation;

    if (rb != null)
        rb.linearVelocity = Vector2.zero;

    if (animator != null)
        animator.SetTrigger("birdcall");
}

public void ResetAnimationForNewRound()
{
    birdCallLockUntil = 0f;

    if (image != null)
    {
        image.localScale = imageBaseScale;
        image.localRotation = imageBaseRotation;
    }

    transform.localScale = mouseBaseScale;
    transform.localRotation = mouseBaseRotation;

    if (animator == null)
        return;

    animator.ResetTrigger("win");
    animator.ResetTrigger("lose");
    animator.ResetTrigger("birdcall");

    animator.SetFloat("movespeed", 0f);
    animator.SetBool("isready", false);

    animator.Play("empty", 0, 0f);
}

private bool CanMoveNow()
{
    return canMove && Time.time >= birdCallLockUntil;
}

private void UpdateMouseDirection(Vector2 direction)
{
    if (direction.sqrMagnitude < 0.001f)
        return;

    bool facingLeft = direction.x < -0.01f;
    float angle;

    if (facingLeft)
    {
        // Flipping horizontally reverses the apparent rotation direction.
        angle = -Mathf.Atan2(direction.y, -direction.x) * Mathf.Rad2Deg;
    }
    else
    {
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    transform.localRotation =
        mouseBaseRotation * Quaternion.Euler(0f, 0f, angle);
    transform.localScale = new Vector3(
        Mathf.Abs(mouseBaseScale.x) * (facingLeft ? -1f : 1f),
        mouseBaseScale.y,
        mouseBaseScale.z
    );
}
}
