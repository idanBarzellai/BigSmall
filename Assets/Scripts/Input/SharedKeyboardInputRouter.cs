using UnityEngine;


    public  class SharedKeyboardInputRouter : MonoBehaviour
    {
        public float GetElephantMoveAxis()
        {
            float axis = 0f;

            if (Input.GetKey(KeyCode.A))
                axis -= 1f;

            if (Input.GetKey(KeyCode.D))
                axis += 1f;

            return axis;
        }

        public bool IsElephantJumpPressed()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        public Vector2 GetMouseMoveInput()
        {
            Vector2 input = Vector2.zero;

            if (Input.GetKey(KeyCode.LeftArrow))
                input.x -= 1f;

            if (Input.GetKey(KeyCode.RightArrow))
                input.x += 1f;

            if (Input.GetKey(KeyCode.DownArrow))
                input.y -= 1f;

            if (Input.GetKey(KeyCode.UpArrow))
                input.y += 1f;

            if (input.sqrMagnitude > 1f)
                input.Normalize();

            return input;
        }
    }
