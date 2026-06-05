using System.Collections;
using ElephantVsMouse.Gameplay.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ElephantVsMouse.Gameplay.Core
{
    public sealed class WinScreenController : MonoBehaviour
    {
        private PrototypeBootstrapper bootstrapper;
        private RaceManager raceManager;
        private Canvas canvas;
        private GameObject panel;
        private Text scoreText;
        private Text messageText;
        private Coroutine restartRoutine;

        public void Bind(PrototypeBootstrapper bootstrapper, RaceManager raceManager)
        {
            if (this.raceManager != null)
            {
                this.raceManager.MatchEnded -= HandleMatchEnded;
                this.raceManager.RoundEnded -= HandleRoundEnded;
                this.raceManager.ScoreChanged -= HandleScoreChanged;
            }

            this.bootstrapper = bootstrapper;
            this.raceManager = raceManager;

            if (this.raceManager != null)
            {
                this.raceManager.MatchEnded += HandleMatchEnded;
                this.raceManager.RoundEnded += HandleRoundEnded;
                this.raceManager.ScoreChanged += HandleScoreChanged;
            }

            EnsureUi();
            RefreshScoreText();
            HideWinScreen();
        }

        private void OnDestroy()
        {
            if (raceManager != null)
            {
                raceManager.MatchEnded -= HandleMatchEnded;
                raceManager.RoundEnded -= HandleRoundEnded;
                raceManager.ScoreChanged -= HandleScoreChanged;
            }
        }

        private void EnsureUi()
        {
            if (canvas != null && canvas.gameObject != null)
            {
                return;
            }

            GameObject canvasObject = new GameObject("Win Screen Canvas", typeof(RectTransform));
            if (transform.parent != null)
            {
                canvasObject.transform.SetParent(transform.parent, false);
            }
            else
            {
                canvasObject.transform.SetParent(transform, false);
            }
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject scoreObject = new GameObject("Score Text", typeof(RectTransform));
            scoreObject.transform.SetParent(canvasObject.transform, false);
            scoreText = scoreObject.AddComponent<Text>();
            scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            scoreText.fontSize = 24;
            scoreText.alignment = TextAnchor.UpperLeft;
            scoreText.color = Color.white;

            RectTransform scoreRect = scoreText.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0f, 1f);
            scoreRect.anchorMax = new Vector2(0f, 1f);
            scoreRect.pivot = new Vector2(0f, 1f);
            scoreRect.anchoredPosition = new Vector2(16f, -16f);
            scoreRect.sizeDelta = new Vector2(360f, 60f);

            panel = new GameObject("Win Screen Panel", typeof(RectTransform));
            panel.transform.SetParent(canvasObject.transform, false);
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.75f);

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            GameObject textObject = new GameObject("Win Screen Text", typeof(RectTransform));
            textObject.transform.SetParent(panel.transform, false);
            messageText = textObject.AddComponent<Text>();
            messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            messageText.fontSize = 36;
            messageText.alignment = TextAnchor.MiddleCenter;
            messageText.color = Color.white;
            messageText.horizontalOverflow = HorizontalWrapMode.Wrap;
            messageText.verticalOverflow = VerticalWrapMode.Overflow;

            RectTransform textRect = messageText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(40f, 40f);
            textRect.offsetMax = new Vector2(-40f, -40f);
        }

        private void HandleMatchEnded(PlayerId winner)
        {
            ShowEndSequence(winner, 3, true);
        }

        private void HandleRoundEnded(PlayerId winner)
        {
            ShowEndSequence(winner, 1, false);
        }

        private void ShowEndSequence(PlayerId winner, int countdownSeconds, bool isMatchEnd)
        {
            // Ensure UI exists (recreate if destroyed by a restart) before showing
            EnsureUi();
            RefreshScoreText();

            if (restartRoutine != null)
            {
                StopCoroutine(restartRoutine);
            }

            if (panel != null)
            {
                panel.SetActive(true);
            }

            if (messageText != null)
            {
                messageText.text = GetWinnerText(winner) + (isMatchEnd ? "\nRestarting in " : "\nNext round in ") + countdownSeconds;
            }

            restartRoutine = StartCoroutine(RestartCountdown(winner, countdownSeconds, isMatchEnd));
        }

        private IEnumerator RestartCountdown(PlayerId winner, int countdownSeconds, bool isMatchEnd)
        {
            for (int countdown = countdownSeconds; countdown > 0; countdown--)
            {
                if (messageText != null)
                {
                    messageText.text = GetWinnerText(winner) + (isMatchEnd ? "\nRestarting in " : "\nNext round in ") + countdown;
                }

                yield return new WaitForSeconds(1f);
            }

            if (messageText != null)
            {
                messageText.text = GetWinnerText(winner) + (isMatchEnd ? "\nRestarting now" : "\nStarting next round");
            }

            yield return new WaitForSeconds(0.2f);

            if (bootstrapper != null)
            {
                RaceManager rebuiltRaceManager = bootstrapper.RestartWorld();
                Bind(bootstrapper, rebuiltRaceManager);
            }
        }

        private void HandleScoreChanged()
        {
            RefreshScoreText();
        }

        private void HideWinScreen()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }

            if (messageText != null)
            {
                messageText.text = string.Empty;
            }
        }

        private string GetWinnerText(PlayerId winner)
        {
            return winner == PlayerId.Elephant ? "Elephant wins!" : "Mouse wins!";
        }

        private void RefreshScoreText()
        {
            if (scoreText == null || scoreText.gameObject == null || raceManager == null)
            {
                return;
            }

            scoreText.text = "Elephant wins: " + raceManager.TotalElephantWins + "\nMouse wins: " + raceManager.TotalMouseWins;
        }
    }
}