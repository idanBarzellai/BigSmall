using UnityEngine;

public sealed class FinishLine : MonoBehaviour
{
    [SerializeField] private RaceManager raceManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTriggerEnter(other);
    }

    public void HandleTriggerEnter(Collider2D other)
    {
        if (raceManager == null || !raceManager.IsRoundActive)
            return;

        if (other.GetComponentInParent<ElephantController>() != null)
        {
            raceManager.RegisterWinner(PlayerId.Elephant);
            return;
        }

        if (other.GetComponentInParent<MouseController>() != null)
            raceManager.RegisterWinner(PlayerId.Mouse);
    }

    public void SetRaceManager(RaceManager manager)
    {
        raceManager = manager;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            if (col.gameObject == gameObject)
                continue;

            FinishLineTriggerRelay relay =
                col.GetComponent<FinishLineTriggerRelay>();

            if (relay == null)
                relay = col.gameObject.AddComponent<FinishLineTriggerRelay>();

            relay.Initialize(this);
        }
    }
}

public sealed class FinishLineTriggerRelay : MonoBehaviour
{
    private FinishLine finishLine;

    public void Initialize(FinishLine target)
    {
        finishLine = target;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (finishLine != null)
            finishLine.HandleTriggerEnter(other);
    }
}
