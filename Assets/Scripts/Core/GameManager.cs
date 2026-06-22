using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

[Header("Match End")]
[SerializeField, Min(1f)] private float winnerScaleMultiplier = 2f;

private bool elephantReady;
private bool mouseReady;
private bool waitingForReady;
    private bool matchEnded;
    private Vector3 elephantBaseScale;
    private Vector3 mouseBaseScale;
    private readonly List<(Renderer renderer, bool wasEnabled)>
        hiddenWorldRenderers = new();

    private void Awake()
    {
        raceManager.RoundEnded += HandleRoundEnded;
        raceManager.MatchEnded += HandleMatchEnded;
        elephantBaseScale = elephant.transform.localScale;
        mouseBaseScale = mouse.transform.localScale;
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
    AudioManager.PrepareForReadyPhase();
    AudioManager.Play(GameSound.GameStart);

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
    if (matchEnded)
    {
        if (inputRouter != null && inputRouter.IsRestartPressed())
            RestartMatch();

        return;
    }

    if (!waitingForReady)
        return;

    if (!mouseReady && inputRouter.IsMouseReadyPressed())
    {
        mouseReady = true;
        mouse.SetReadyAnimation(true);
        AudioManager.Play(GameSound.MouseReady);
    }

    if (!elephantReady && inputRouter.IsElephantReadyPressed())
    {
        elephantReady = true;
        elephant.SetReadyAnimation(true);
        AudioManager.Play(GameSound.ElephantReady);
    }

    if (mouseReady && elephantReady)
    {
        waitingForReady = false;
        StartCoroutine(StartPreparedRoundWithCountdown());
    }
}

private IEnumerator StartPreparedRoundWithCountdown()
{
    AudioManager.FadeOutGameStart();

    if (gameUI != null)
    {
        gameUI.ClearMessage();
        AudioManager.Play(GameSound.Countdown);
        yield return gameUI.ShowCountdown();
    }

    AudioManager.StartBackgroundMusic();

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
    AudioManager.StopBackgroundMusic();
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
        AudioManager.Play(GameSound.ElephantWin);
        elephant.PlayWinAnimation();
        mouse.PlayLoseAnimation();
    }
    else
    {
        AudioManager.Play(GameSound.MouseWin);
        mouse.PlayWinAnimation();
        elephant.PlayLoseAnimation();
    }

    yield return new WaitForSeconds(roundEndDelay);

    GenerateAndPrepareRound();
    BeginReadyPhase();
}

 private void HandleMatchEnded(PlayerId winner)
{
    if (matchEnded)
        return;

    matchEnded = true;
    waitingForReady = false;
    AudioManager.Play(GameSound.MatchWin);

    Transform winnerTransform;

     if (winner == PlayerId.Elephant)
     {
        elephant.PlayWinAnimation();
        mouse.PlayLoseAnimation();
        winnerTransform = elephant.transform;
    }
    else
    {
        mouse.PlayWinAnimation();
        elephant.PlayLoseAnimation();
        winnerTransform = mouse.transform;
    }

    elephant.SetCanMove(false);
    mouse.SetCanMove(false);

    winnerTransform.localScale = Vector3.Scale(
        winnerTransform.localScale,
        Vector3.one * winnerScaleMultiplier
    );

    if (cameraFollower != null)
        cameraFollower.FocusOnMatchWinner(winnerTransform);

    HideWorldExcept(winnerTransform);
    AudioManager.StopBackgroundMusic();
}

private void RestartMatch()
{
    RestoreWorldRenderers();
    matchEnded = false;
    elephant.transform.localScale = elephantBaseScale;
    mouse.transform.localScale = mouseBaseScale;

    raceManager.ResetMatch();
    AudioManager.ResetForNewMatch();
    GenerateAndPrepareRound();
    BeginReadyPhase();
}

private void HideWorldExcept(Transform winner)
{
    hiddenWorldRenderers.Clear();

    foreach (Renderer renderer in FindObjectsByType<Renderer>())
    {
        if (renderer == null ||
            renderer.transform == winner ||
            renderer.transform.IsChildOf(winner))
        {
            continue;
        }

        hiddenWorldRenderers.Add((renderer, renderer.enabled));
        renderer.enabled = false;
    }
}

private void RestoreWorldRenderers()
{
    foreach ((Renderer renderer, bool wasEnabled) in hiddenWorldRenderers)
    {
        if (renderer != null)
            renderer.enabled = wasEnabled;
    }

    hiddenWorldRenderers.Clear();
}


}
