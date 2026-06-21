using UnityEngine;
using System.Collections;

public sealed class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RaceManager raceManager;
    [SerializeField] private RoundGenerator roundGenerator;
    [SerializeField] private ElephantController elephant;
    [SerializeField] private MouseController mouse;
    [SerializeField] private CameraFollower cameraFollower;
    [SerializeField] private GameUI gameUI;

    [Header("Spawn Positions")]
    [SerializeField] private Vector3 elephantStartPosition = new Vector3(0f, 2f, 0f);
    [SerializeField] private Vector3 mouseStartPosition = new Vector3(0f, -2f, 0f);
[SerializeField] private SharedKeyboardInputRouter inputRouter;

[Header("Round End")]
[SerializeField] private float roundEndDelay = 3f;

private bool elephantReady;
private bool mouseReady;
private bool waitingForReady;
    private bool matchEnded;

    private void Awake()
    {
        raceManager.RoundEnded += HandleRoundEnded;
        raceManager.MatchEnded += HandleMatchEnded;
    }

    private void Start()
    {
    PrepareNewMatch();
    }

private void PrepareNewMatch()
{
    GenerateAndPrepareRound();
    BeginReadyPhase();
}
private void BeginReadyPhase()
{
    elephantReady = false;
    mouseReady = false;
    waitingForReady = true;

    if (gameUI != null)
        gameUI.ShowReadyImages();

    elephant.SetReadyAnimation(false);
    mouse.SetReadyAnimation(false);

    elephant.SetCanMove(false);
    mouse.SetCanMove(false);
}

private void Update()
{
    if (!waitingForReady)
        return;

    if (!mouseReady && inputRouter.IsMouseReadyPressed())
    {
        mouseReady = true;
        mouse.SetReadyAnimation(true);
    }

    if (!elephantReady && inputRouter.IsElephantReadyPressed())
    {
        elephantReady = true;
        elephant.SetReadyAnimation(true);
    }

    if (mouseReady && elephantReady)
    {
        waitingForReady = false;
        StartCoroutine(StartPreparedRoundWithCountdown());
    }
}

private IEnumerator StartPreparedRoundWithCountdown()
{
    if (gameUI != null)
    {
        gameUI.ClearMessage();
        yield return gameUI.ShowCountdown();
    }

    raceManager.StartNewRound();

    elephant.SetCanMove(true);
    mouse.SetCanMove(true);
}

    private void OnDestroy()
    {
        raceManager.RoundEnded -= HandleRoundEnded;
        raceManager.MatchEnded -= HandleMatchEnded;
    }



private void GenerateAndPrepareRound()
{
    roundGenerator.GenerateRound();

    elephant.transform.position = elephantStartPosition;
    mouse.transform.position = mouseStartPosition;

    elephant.SetCanMove(false);
    mouse.SetCanMove(false);

    elephant.ResetAnimationForNewRound();
    mouse.ResetAnimationForNewRound();

    cameraFollower.ResetForNewRound();
}

private void HandleRoundEnded(PlayerId winner)
{
    StartCoroutine(RoundEndRoutine(winner));
}

private IEnumerator RoundEndRoutine(PlayerId winner)
{
    elephant.SetCanMove(false);
    mouse.SetCanMove(false);

    if (winner == PlayerId.Elephant)
    {
        elephant.PlayWinAnimation();
        mouse.PlayLoseAnimation();
    }
    else
    {
        mouse.PlayWinAnimation();
        elephant.PlayLoseAnimation();
    }

    yield return new WaitForSeconds(roundEndDelay);

    GenerateAndPrepareRound();
    BeginReadyPhase();
}

 private void HandleMatchEnded(PlayerId winner)
{
     if (winner == PlayerId.Elephant)
    {
        elephant.PlayWinAnimation();
        mouse.PlayLoseAnimation();
    }
    else
    {
        mouse.PlayWinAnimation();
        elephant.PlayLoseAnimation();
    }

    elephant.SetCanMove(false);
    mouse.SetCanMove(false);

    matchEnded = true;
}


}
