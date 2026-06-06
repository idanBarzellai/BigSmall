using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RaceManager raceManager;
    [SerializeField] private RoundGenerator roundGenerator;
    [SerializeField] private ElephantController elephant;
    [SerializeField] private MouseController mouse;
    [SerializeField] private CameraFollower cameraFollower;

    [Header("Spawn Positions")]
    [SerializeField] private Vector3 elephantStartPosition = new Vector3(0f, 2f, 0f);
    [SerializeField] private Vector3 mouseStartPosition = new Vector3(0f, -2f, 0f);

    private void Awake()
    {
        raceManager.RoundEnded += HandleRoundEnded;
        raceManager.MatchEnded += HandleMatchEnded;
    }

    private void Start()
    {
        StartNewMatch();
    }

    private void OnDestroy()
    {
        raceManager.RoundEnded -= HandleRoundEnded;
        raceManager.MatchEnded -= HandleMatchEnded;
    }

    private void StartNewMatch()
    {
        GenerateAndStartRound();
        raceManager.StartNewMatch();
    }

    private void GenerateAndStartRound()
    {
        roundGenerator.GenerateRound();

        elephant.transform.position = elephantStartPosition;
        mouse.transform.position = mouseStartPosition;

        elephant.SetCanMove(true);
        mouse.SetCanMove(true);

        cameraFollower.ResetForNewRound();
    }

    private void HandleRoundEnded(PlayerId winner)
    {
        Debug.Log($"Round Winner: {winner}");

        elephant.SetCanMove(false);
        mouse.SetCanMove(false);

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
        GenerateAndStartRound();
        raceManager.StartNewRound();
    }
}