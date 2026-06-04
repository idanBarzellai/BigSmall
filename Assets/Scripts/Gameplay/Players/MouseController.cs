using UnityEngine;

namespace ElephantVsMouse.Gameplay.Players
{
    public sealed class MouseController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;

        public bool CanAcceptInput { get; private set; }

        public void SetInputEnabled(bool enabledState)
        {
            CanAcceptInput = enabledState;
        }
    }
}