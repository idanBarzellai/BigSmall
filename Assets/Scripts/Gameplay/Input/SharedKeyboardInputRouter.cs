using UnityEngine;
using UnityEngine.InputSystem;

namespace ElephantVsMouse.Gameplay.Input
{
    public sealed class SharedKeyboardInputRouter : MonoBehaviour
    {
        public float GetElephantMoveAxis()
        {
            float axis = 0f;

            if (Keyboard.current == null)
            {
                return axis;
            }

            if (Keyboard.current.aKey.isPressed)
            {
                axis -= 1f;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                axis += 1f;
            }

            return Mathf.Clamp(axis, -1f, 1f);
        }

        public bool IsElephantJumpPressed()
        {
            return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        public Vector2 GetMouseMoveInput()
        {
            if (Keyboard.current == null)
            {
                return Vector2.zero;
            }

            Vector2 input = Vector2.zero;

            if (Keyboard.current.leftArrowKey.isPressed)
            {
                input.x -= 1f;
            }

            if (Keyboard.current.rightArrowKey.isPressed)
            {
                input.x += 1f;
            }

            if (Keyboard.current.downArrowKey.isPressed)
            {
                input.y -= 1f;
            }

            if (Keyboard.current.upArrowKey.isPressed)
            {
                input.y += 1f;
            }

            return Vector2.ClampMagnitude(input, 1f);
        }
    }
}