using ElephantVsMouse.Gameplay.Input;
using UnityEngine;

namespace ElephantVsMouse.Gameplay.Players
{
    public sealed class MouseController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
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

            Vector2 moveInput = inputRouter.GetMouseMoveInput();
            body.linearVelocity = moveInput * moveSpeed;
        }

        public void SetInputEnabled(bool enabledState)
        {
            CanAcceptInput = enabledState;
        }
    }
}