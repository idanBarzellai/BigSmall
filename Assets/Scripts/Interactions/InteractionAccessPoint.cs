using UnityEngine;

public sealed class InteractionAccessPoint : MonoBehaviour
{
    [SerializeField] private PlayerId allowedPlayer;
    [SerializeField] private SharedInteractionController controller;

    private SpriteRenderer spriteRenderer;
    private Sprite usedSprite;
    private bool consumed;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(
        PlayerId player,
        SharedInteractionController interactionController,
        Color pointColor,
        Sprite replacementSprite)
    {
        allowedPlayer = player;
        controller = interactionController;
        usedSprite = replacementSprite;
        consumed = false;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            if (!col.isTrigger || col.gameObject == gameObject)
                continue;

            InteractionTriggerRelay relay =
                col.GetComponent<InteractionTriggerRelay>();

            if (relay == null)
                relay = col.gameObject.AddComponent<InteractionTriggerRelay>();

            relay.Initialize(this);
        }

        if (spriteRenderer != null)
            spriteRenderer.color = pointColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTriggerEnter(other, GetComponent<Collider2D>());
    }

    public void HandleTriggerEnter(
        Collider2D other,
        Collider2D interactionCollider)
    {
        if (consumed || controller == null || controller.IsUsed)
            return;

        if (allowedPlayer == PlayerId.Elephant)
        {
            TryActivateByElephant(other, interactionCollider);
            return;
        }

        if (allowedPlayer == PlayerId.Mouse)
            TryActivateByMouse(other);
    }

    private void TryActivateByElephant(
        Collider2D other,
        Collider2D interactionCollider)
    {
        ElephantController elephant =
            other.GetComponentInParent<ElephantController>();

        if (elephant == null)
            return;

        Rigidbody2D rb = other.attachedRigidbody;

        if (rb == null)
            return;

        bool isFallingFastEnough = rb.linearVelocity.y < -0.5f;
        float interactionY = interactionCollider != null
            ? interactionCollider.bounds.center.y
            : transform.position.y;
        bool isClearlyAbovePoint = other.bounds.min.y > interactionY;

        if (!isFallingFastEnough || !isClearlyAbovePoint)
            return;

        elephant.ResetMomentum();
        controller.Activate(PlayerId.Elephant);
    }

    private void TryActivateByMouse(Collider2D other)
    {
        MouseController mouse = other.GetComponentInParent<MouseController>();

        if (mouse != null)
            controller.Activate(PlayerId.Mouse);
    }

    public void Consume()
    {
        consumed = true;

        if (spriteRenderer != null && usedSprite != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
            spriteRenderer.sprite = usedSprite;
            spriteRenderer.enabled = true;
        }

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            if (col.isTrigger)
                col.enabled = false;
        }
    }
}

public sealed class InteractionTriggerRelay : MonoBehaviour
{
    private InteractionAccessPoint accessPoint;
    private Collider2D interactionCollider;

    public void Initialize(InteractionAccessPoint target)
    {
        accessPoint = target;
        interactionCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (accessPoint != null)
            accessPoint.HandleTriggerEnter(other, interactionCollider);
    }
}
