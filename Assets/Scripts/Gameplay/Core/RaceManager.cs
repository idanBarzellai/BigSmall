using UnityEngine;
using System;

namespace ElephantVsMouse.Gameplay.Core
{
    public sealed class RaceManager : MonoBehaviour
    {
        [SerializeField] private int roundsToWin = 2;

        private static int persistentElephantWins;
        private static int persistentMouseWins;
        private bool roundWinnerRegistered;

        public event Action<PlayerId> MatchEnded;
        public event Action<PlayerId> RoundEnded;
        public event Action ScoreChanged;

        public int ElephantWins { get; private set; }
        public int MouseWins { get; private set; }
        public int TotalElephantWins => persistentElephantWins;
        public int TotalMouseWins => persistentMouseWins;
        public int CurrentRound { get; private set; }
        public bool IsMatchActive { get; private set; }

        public void BeginMatch()
        {
            CurrentRound = 1;
            IsMatchActive = true;
            roundWinnerRegistered = false;
        }

        public void RegisterRoundWinner(PlayerId winner)
        {
            if (!IsMatchActive || roundWinnerRegistered)
            {
                return;
            }

            roundWinnerRegistered = true;

            if (winner == PlayerId.Elephant)
            {
                ElephantWins++;
                persistentElephantWins++;
            }
            else
            {
                MouseWins++;
                persistentMouseWins++;
            }

            ScoreChanged?.Invoke();

            if (ElephantWins >= roundsToWin || MouseWins >= roundsToWin)
            {
                IsMatchActive = false;
                MatchEnded?.Invoke(winner);
                return;
            }

            CurrentRound++;
            RoundEnded?.Invoke(winner);
        }

        public void ResetMatch()
        {
            ElephantWins = 0;
            MouseWins = 0;
            CurrentRound = 0;
            IsMatchActive = false;
            roundWinnerRegistered = false;
        }
    }

    public enum PlayerId
    {
        Elephant,
        Mouse
    }
}