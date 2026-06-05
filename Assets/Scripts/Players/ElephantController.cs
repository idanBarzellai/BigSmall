using UnityEngine;


    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class ElephantController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float jumpForce = 9f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayers;

        [Header("Input")]
        [SerializeField] private SharedKeyboardInputRouter inputRouter;

        private Rigidbody2D rb;
        private bool canMove = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }

        private void Update()
        {
            if (!canMove || inputRouter == null)
                return;

            if (inputRouter.IsElephantJumpPressed() && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        private void FixedUpdate()
        {
            if (!canMove || inputRouter == null)
                return;

            float moveInput = inputRouter.GetElephantMoveAxis();
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
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
