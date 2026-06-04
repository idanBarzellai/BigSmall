using ElephantVsMouse.Gameplay.Input;
using UnityEngine;

namespace ElephantVsMouse.Gameplay.Players
{
    public sealed class ElephantController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float groundCheckRadius = 0.15f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private SharedKeyboardInputRouter inputRouter;

        private Rigidbody2D body;

        public bool CanAcceptInput { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();

            if (inputRouter == null)
            {
                inputRouter = FindFirstObjectByType<SharedKeyboardInputRouter>();
            }
        }

        private void FixedUpdate()
        {
            if (!CanAcceptInput || body == null || inputRouter == null)
            {
                return;
            }

            float moveAxis = inputRouter.GetElephantMoveAxis();
            Vector2 velocity = body.linearVelocity;
            velocity.x = moveAxis * moveSpeed;
            body.linearVelocity = velocity;
        }

        private void Update()
        {
            if (!CanAcceptInput || body == null || inputRouter == null)
            {
                return;
            }

            if (inputRouter.IsElephantJumpPressed() && IsGrounded())
            {
                Vector2 velocity = body.linearVelocity;
                velocity.y = 0f;
                body.linearVelocity = velocity;
                body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        public void SetInputEnabled(bool enabledState)
        {
            CanAcceptInput = enabledState;
        }

        private bool IsGrounded()
        {
            if (groundCheck == null)
            {
                return true;
            }

            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers) != null;
        }
    }
}