using UnityEngine;

public sealed class InteractionAccessPoint : MonoBehaviour
{
    [SerializeField] private PlayerId allowedPlayer;
    [SerializeField] private SharedInteractionController controller;

    private SpriteRenderer spriteRenderer;
    private bool consumed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(
        PlayerId player,
        SharedInteractionController interactionController,
        Color pointColor)
    {
        allowedPlayer = player;
        controller = interactionController;
        consumed = false;

        if (spriteRenderer != null)
            spriteRenderer.color = pointColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    if (consumed || controller == null || controller.IsUsed)
        return;

    if (allowedPlayer == PlayerId.Elephant)
    {
        TryActivateByElephant(other);
        return;
    }

    if (allowedPlayer == PlayerId.Mouse)
    {
        TryActivateByMouse(other);
    }
}

private void TryActivateByElephant(Collider2D other)
{
    ElephantController elephant = other.GetComponent<ElephantController>();

    if (elephant == null)
        return;

    Rigidbody2D rb = other.attachedRigidbody;

    if (rb == null)
        return;

    bool isFallingFastEnough = rb.linearVelocity.y < -0.5f;
    bool isClearlyAbovePoint = other.bounds.min.y > transform.position.y;

    if (!isFallingFastEnough || !isClearlyAbovePoint)
        return;

    elephant.ResetMomentum();

    controller.Activate(PlayerId.Elephant);
}

private void TryActivateByMouse(Collider2D other)
{
    MouseController mouse = other.GetComponent<MouseController>();

    if (mouse == null)
        return;

    controller.Activate(PlayerId.Mouse);
}

    public void Consume()
    {
        consumed = true;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.gray;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }
}