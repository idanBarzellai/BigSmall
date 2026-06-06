using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RaceManager raceManager;
    [SerializeField] private ElephantController elephant;
    [SerializeField] private MouseController mouse;
    [SerializeField] private CameraFollower cameraFollower;
[SerializeField] private RoundGenerator roundGenerator;
    private void Awake()
    {
        raceManager.RoundEnded += HandleRoundEnded;
        raceManager.MatchEnded += HandleMatchEnded;
    }

    private void Start()
    {
        raceManager.StartNewMatch();
    }

    private void OnDestroy()
    {
        raceManager.RoundEnded -= HandleRoundEnded;
        raceManager.MatchEnded -= HandleMatchEnded;
    }

    private void HandleRoundEnded(PlayerId winner)
    {
        Debug.Log($"Round Winner: {winner}");

        elephant.SetCanMove(false);
        mouse.SetCanMove(false);

        // temporary restart
        Invoke(nameof(StartNextRound), 2f);
    }

    private void HandleMatchEnded(PlayerId winner)
    {
        Debug.Log($"Match Winner: {winner}");

        elephant.SetCanMove(false);
        mouse.SetCanMove(false);
    }

    private void StartNextRound()
    {
        // temporary reset positions

        elephant.transform.position = new Vector3(0f, 2f, 0f);
        mouse.transform.position = new Vector3(0f, -2f, 0f);

        elephant.SetCanMove(true);
        mouse.SetCanMove(true);

        cameraFollower.ResetForNewRound();

        raceManager.StartNewRound();
    }
}