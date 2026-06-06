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

        bool validActivation =
            allowedPlayer == PlayerId.Elephant
                ? other.GetComponent<ElephantController>() != null
                : other.GetComponent<MouseController>() != null;

        if (!validActivation)
            return;

        controller.Activate(allowedPlayer);
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