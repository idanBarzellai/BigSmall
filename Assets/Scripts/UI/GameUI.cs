using System.Collections;
using TMPro;
using UnityEngine;

public sealed class GameUI : MonoBehaviour
{
    [SerializeField] private RaceManager raceManager;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text messageText;

    private void OnEnable()
    {
        raceManager.ScoreChanged += UpdateScore;
        raceManager.RoundEnded += ShowRoundWinner;
        raceManager.MatchEnded += ShowMatchWinner;
    }

    private void OnDisable()
    {
        raceManager.ScoreChanged -= UpdateScore;
        raceManager.RoundEnded -= ShowRoundWinner;
        raceManager.MatchEnded -= ShowMatchWinner;
    }

    private void Start()
    {
        UpdateScore();
        ClearMessage();
    }

    private void UpdateScore()
    {
        scoreText.text =
            $"Round {raceManager.CurrentRound}\n" +
            $"Elephant {raceManager.ElephantWins} - {raceManager.MouseWins} Mouse";
    }

    private void ShowRoundWinner(PlayerId winner)
    {
        messageText.text = $"{winner} wins the round!";
    }

  private void ShowMatchWinner(PlayerId winner)
{
    messageText.text = $"{winner} wins the match!\nPress Space to restart";
}

    public IEnumerator ShowCountdown()
    {
        messageText.text = "3";
        yield return new WaitForSeconds(1f);

        messageText.text = "2";
        yield return new WaitForSeconds(1f);

        messageText.text = "1";
        yield return new WaitForSeconds(1f);

        messageText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        ClearMessage();
    }

    public void ClearMessage()
    {
        messageText.text = "";
    }
}