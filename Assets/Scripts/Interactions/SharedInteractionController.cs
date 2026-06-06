using UnityEngine;

public sealed class SharedInteractionController : MonoBehaviour
{
    private bool used;

    private InteractionAccessPoint elephantAccessPoint;
    private InteractionAccessPoint mouseAccessPoint;
    [SerializeField] private GameObject birdAttackPrefab;
[SerializeField] private Transform elephant;
[SerializeField] private ScreenObscurer screenObscurer;

    public bool IsUsed => used;

    public void Initialize(
        InteractionAccessPoint elephantPoint,
        InteractionAccessPoint mousePoint)
    {
        elephantAccessPoint = elephantPoint;
        mouseAccessPoint = mousePoint;
    }

    public void SetupReferences(
    GameObject birdPrefab,
    Transform elephantTransform,
    ScreenObscurer obscurer)
{
    birdAttackPrefab = birdPrefab;
    elephant = elephantTransform;
    screenObscurer = obscurer;
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

    if (birdAttackPrefab == null)
        return;

    GameObject bird = Instantiate(birdAttackPrefab);

    BirdAttackActor actor =
        bird.GetComponent<BirdAttackActor>();

    if (actor != null)
    {
        actor.Initialize(
            elephant,
            screenObscurer
        );
    }
}
}