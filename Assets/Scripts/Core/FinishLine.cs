using UnityEngine;

public sealed class FinishLine : MonoBehaviour
{
    [SerializeField] private RaceManager raceManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (raceManager == null)
            return;

        if (!raceManager.IsRoundActive)
            return;

        if (other.GetComponent<ElephantController>() != null)
        {
            raceManager.RegisterWinner(PlayerId.Elephant);
            return;
        }

        if (other.GetComponent<MouseController>() != null)
        {
            raceManager.RegisterWinner(PlayerId.Mouse);
        }
    }

    public void SetRaceManager(RaceManager manager)
{
    raceManager = manager;
}
}