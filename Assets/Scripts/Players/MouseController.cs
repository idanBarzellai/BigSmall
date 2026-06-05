using UnityEngine;


    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class MouseController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float acceleration = 15f;

        [Header("Input")]
        [SerializeField] private SharedKeyboardInputRouter inputRouter;

        private Rigidbody2D rb;
        private bool canMove = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            if (!canMove || inputRouter == null)
                return;

            Vector2 input = inputRouter.GetMouseMoveInput();

            Vector2 targetVelocity = input * moveSpeed;

            rb.linearVelocity = Vector2.Lerp(
                rb.linearVelocity,
                targetVelocity,
                acceleration * Time.fixedDeltaTime
            );
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
