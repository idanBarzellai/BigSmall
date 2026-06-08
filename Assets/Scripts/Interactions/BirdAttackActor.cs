using System.Collections;
using UnityEngine;

public sealed class BirdAttackActor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform elephant;
    [SerializeField] private ScreenObscurer screenObscurer;

    [SerializeField] private GameObject eggPrefab;

    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float heightAboveElephant = 4f;
    [SerializeField] private float startOffsetX = -8f;
    [SerializeField] private float endOffsetX = 8f;

    [Header("Timing")]
    [SerializeField] private float eggDropDistance = 0.6f;
    [SerializeField] private float obscureDuration = 2.5f;



    private bool eggDropped;

    public void Initialize(Transform elephantTarget, ScreenObscurer obscurer)
    {
        elephant = elephantTarget;
        screenObscurer = obscurer;
    }

    private void Start()
    {
        StartCoroutine(FlyRoutine());
    }

    private IEnumerator FlyRoutine()
    {
        if (elephant == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Vector3 start = new Vector3(
            elephant.position.x + startOffsetX,
            elephant.position.y + heightAboveElephant,
            0f
        );

        Vector3 end = new Vector3(
            elephant.position.x + endOffsetX,
            elephant.position.y + heightAboveElephant,
            0f
        );

        transform.position = start;

        while (Vector3.Distance(transform.position, end) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                end,
                speed * Time.deltaTime
            );

            if (!eggDropped &&
                Mathf.Abs(transform.position.x - elephant.position.x) <= eggDropDistance)
            {
                eggDropped = true;
                DropEgg();
            }

            yield return null;
        }

        Destroy(gameObject);
    }

   private void DropEgg()
{
    Debug.Log("Egg dropped!");

    if (eggPrefab == null)
    {
        if (screenObscurer != null)
            screenObscurer.Obscure(obscureDuration);

        return;
    }

    GameObject egg = Instantiate(
        eggPrefab,
        transform.position,
        Quaternion.identity
    );

    EggDropActor eggDrop = egg.GetComponent<EggDropActor>();

    if (eggDrop != null)
    {
        eggDrop.Initialize(
            screenObscurer,
            obscureDuration
        );
    }
}
}