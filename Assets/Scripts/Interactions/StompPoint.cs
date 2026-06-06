using UnityEngine;

public sealed class StompPoint : InteractionPoint
{
    [Header("Stomp Settings")]
    [SerializeField] private float stompCooldown = 0.5f;

    private float lastStompTime = -999f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsAvailable)
            return;

        if (Time.time < lastStompTime + stompCooldown)
            return;

        ElephantController elephant = other.GetComponent<ElephantController>();

        if (elephant != null)
        {
            TryActivateByElephant(other);
            return;
        }

        MouseController mouse = other.GetComponent<MouseController>();

        if (mouse != null)
        {
            DisableByMouse();
        }
    }

    private void TryActivateByElephant(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;

        if (rb == null)
            return;

        bool isFalling = rb.linearVelocity.y <= 0f;
        bool isAbovePoint = other.transform.position.y > transform.position.y;

        if (!isFalling || !isAbovePoint)
            return;

        lastStompTime = Time.time;

        Debug.Log("Elephant activated stomp point");

        // Later: trigger earthquake effect here.
        MarkUsed();

        // Elephant stomping this point disables the linked mouse hole.
        if (linkedPoint != null)
            linkedPoint.DisablePoint();
    }

    private void DisableByMouse()
    {
        Debug.Log("Mouse disabled stomp point");

        DisablePoint();
    }
}