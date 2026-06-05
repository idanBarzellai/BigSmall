using System;
using UnityEngine;

public enum PlayerId
{
    Elephant,
    Mouse
}

public sealed class RaceManager : MonoBehaviour
{
    [SerializeField] private int roundsToWin = 2;

    public event Action<PlayerId> RoundEnded;
    public event Action<PlayerId> MatchEnded;
    public event Action ScoreChanged;

    public int ElephantWins { get; private set; }
    public int MouseWins { get; private set; }
    public int CurrentRound { get; private set; }
    public bool IsRoundActive { get; private set; }

    private bool hasRoundWinner;

    private void Awake()
    {
        CurrentRound = 0;
        IsRoundActive = false;
    }

    public void StartNewMatch()
    {
        ElephantWins = 0;
        MouseWins = 0;
        CurrentRound = 0;

        StartNewRound();
        ScoreChanged?.Invoke();
    }

    public void StartNewRound()
    {
        CurrentRound++;
        hasRoundWinner = false;
        IsRoundActive = true;
    }

    public void RegisterWinner(PlayerId winner)
    {
        if (!IsRoundActive || hasRoundWinner)
            return;

        hasRoundWinner = true;
        IsRoundActive = false;

        if (winner == PlayerId.Elephant)
            ElephantWins++;
        else
            MouseWins++;

        ScoreChanged?.Invoke();

        if (ElephantWins >= roundsToWin || MouseWins >= roundsToWin)
            MatchEnded?.Invoke(winner);
        else
            RoundEnded?.Invoke(winner);
    }
}