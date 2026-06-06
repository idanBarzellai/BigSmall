using UnityEngine;

public enum InteractionPointState
{
    Available,
    Used,
    Disabled
}

public abstract class InteractionPoint : MonoBehaviour
{
    [Header("State")]
    [SerializeField] protected InteractionPointState state = InteractionPointState.Available;

    [Header("Linked Point")]
    [SerializeField] protected InteractionPoint linkedPoint;

    [Header("Visuals")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Color availableColor = Color.white;
    [SerializeField] protected Color usedColor = Color.gray;
    [SerializeField] protected Color disabledColor = Color.black;

    public bool IsAvailable => state == InteractionPointState.Available;
    public InteractionPoint LinkedPoint => linkedPoint;

    protected virtual void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        RefreshVisual();
    }

    public void SetLinkedPoint(InteractionPoint point)
    {
        linkedPoint = point;
    }

    public void SetPairColor(Color color)
    {
        availableColor = color;
        RefreshVisual();
    }

    public void DisablePoint()
    {
        if (state != InteractionPointState.Available)
            return;

        state = InteractionPointState.Disabled;
        RefreshVisual();
    }

    protected void MarkUsed()
    {
        if (state != InteractionPointState.Available)
            return;

        state = InteractionPointState.Used;
        RefreshVisual();
    }

    protected void RefreshVisual()
    {
        if (spriteRenderer == null)
            return;

        switch (state)
        {
            case InteractionPointState.Available:
                spriteRenderer.color = availableColor;
                break;

            case InteractionPointState.Used:
                spriteRenderer.color = usedColor;
                break;

            case InteractionPointState.Disabled:
                spriteRenderer.color = disabledColor;
                break;
        }
    }
}