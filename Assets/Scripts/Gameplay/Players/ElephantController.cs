using UnityEngine;

namespace ElephantVsMouse.Gameplay.Players
{
    public sealed class ElephantController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float jumpForce = 8f;

        public bool CanAcceptInput { get; private set; }

        public void SetInputEnabled(bool enabledState)
        {
            CanAcceptInput = enabledState;
        }
    }
}