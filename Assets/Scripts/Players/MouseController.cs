using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class MouseController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private SharedKeyboardInputRouter inputRouter;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (!canMove || inputRouter == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = inputRouter.GetMouseMoveInput();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
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
}