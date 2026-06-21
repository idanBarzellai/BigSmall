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
            return Input.GetKeyDown(KeyCode.W);
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

        public bool IsMouseReadyPressed()
{
     return Input.GetKeyDown(KeyCode.LeftArrow) ||
           Input.GetKeyDown(KeyCode.RightArrow) ||
           Input.GetKeyDown(KeyCode.UpArrow) ||
           Input.GetKeyDown(KeyCode.DownArrow);
    
}

public bool IsElephantReadyPressed()
{
    return Input.GetKeyDown(KeyCode.A) ||
           Input.GetKeyDown(KeyCode.S) ||
           Input.GetKeyDown(KeyCode.W) ||
           Input.GetKeyDown(KeyCode.D);
}
    }
