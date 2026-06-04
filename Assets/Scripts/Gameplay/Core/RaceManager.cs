using UnityEngine;

namespace ElephantVsMouse.Gameplay.Core
{
    public sealed class RaceManager : MonoBehaviour
    {
        [SerializeField] private int roundsToWin = 2;

        public int ElephantWins { get; private set; }
        public int MouseWins { get; private set; }
        public int CurrentRound { get; private set; }
        public bool IsMatchActive { get; private set; }

        public void BeginMatch()
        {
            CurrentRound = 1;
            IsMatchActive = true;
        }

        public void RegisterRoundWinner(PlayerId winner)
        {
            if (!IsMatchActive)
            {
                return;
            }

            if (winner == PlayerId.Elephant)
            {
                ElephantWins++;
            }
            else
            {
                MouseWins++;
            }

            if (ElephantWins >= roundsToWin || MouseWins >= roundsToWin)
            {
                IsMatchActive = false;
                return;
            }

            CurrentRound++;
        }

        public void ResetMatch()
        {
            ElephantWins = 0;
            MouseWins = 0;
            CurrentRound = 0;
            IsMatchActive = false;
        }
    }

    public enum PlayerId
    {
        Elephant,
        Mouse
    }
}