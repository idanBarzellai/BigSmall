using UnityEngine;

public sealed class SharedInteractionController : MonoBehaviour
{
    private bool used;

    private InteractionAccessPoint elephantAccessPoint;
    private InteractionAccessPoint mouseAccessPoint;

    public bool IsUsed => used;

    public void Initialize(
        InteractionAccessPoint elephantPoint,
        InteractionAccessPoint mousePoint)
    {
        elephantAccessPoint = elephantPoint;
        mouseAccessPoint = mousePoint;
    }

    public void Activate(PlayerId activator)
    {
        if (used)
            return;

        used = true;

        if (elephantAccessPoint != null)
            elephantAccessPoint.Consume();

        if (mouseAccessPoint != null)
            mouseAccessPoint.Consume();

        if (activator == PlayerId.Elephant)
            TriggerEarthquake();
        else
            TriggerBirdAttack();
    }

    private void TriggerEarthquake()
    {
        Debug.Log("EARTHQUAKE!");
    }

    private void TriggerBirdAttack()
    {
        Debug.Log("BIRD ATTACK!");
    }
}