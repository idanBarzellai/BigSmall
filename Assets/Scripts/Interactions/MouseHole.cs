using UnityEngine;

public sealed class MouseHole : InteractionPoint
{
    [Header("Mouse Hole Settings")]
    [SerializeField] private float activationCooldown = 0.5f;

    private float lastActivationTime = -999f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsAvailable)
            return;

        if (Time.time < lastActivationTime + activationCooldown)
            return;

        MouseController mouse = other.GetComponent<MouseController>();

        if (mouse != null)
        {
            ActivateByMouse();
            return;
        }

        ElephantController elephant = other.GetComponent<ElephantController>();

        if (elephant != null)
        {
            DisableByElephant(other);
        }
    }

    private void ActivateByMouse()
    {
        lastActivationTime = Time.time;

        Debug.Log("Mouse activated mouse hole");

        // Later: trigger bird attack here.
        MarkUsed();

        // Mouse using this hole disables the linked stomp point.
        if (linkedPoint != null)
            linkedPoint.DisablePoint();
    }

    private void DisableByElephant(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;

        if (rb == null)
            return;

        bool isFalling = rb.linearVelocity.y <= 0f;
        bool isAbovePoint = other.transform.position.y > transform.position.y;

        if (!isFalling || !isAbovePoint)
            return;

        Debug.Log("Elephant destroyed mouse hole");

        DisablePoint();
    }
}