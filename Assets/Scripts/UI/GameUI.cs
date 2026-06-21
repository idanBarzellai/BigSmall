using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RaceManager raceManager;

    [Header("Center Images")]
    [SerializeField] private Image topCenterImage;
    [SerializeField] private Image bottomCenterImage;
    [SerializeField] private Sprite topReadySprite;
    [SerializeField] private Sprite bottomReadySprite;

    [Header("Countdown (3, 2, 1)")]
    [SerializeField] private Sprite[] countdownSprites;
    [SerializeField] private float countdownStepDuration = 1f;

    [Header("Trophy Win Images")]
    [SerializeField] private Image trophyImagePrefab;
    [SerializeField] private RectTransform elephantTrophyContainer;
    [SerializeField] private RectTransform mouseTrophyContainer;

    private readonly List<Image> elephantTrophies = new();
    private readonly List<Image> mouseTrophies = new();

    private void OnEnable()
    {
        if (raceManager != null)
            raceManager.ScoreChanged += UpdateWinImages;
    }

    private void OnDisable()
    {
        if (raceManager != null)
            raceManager.ScoreChanged -= UpdateWinImages;
    }

    private void Start()
    {
        UpdateWinImages();
        ShowReadyImages();
    }

    public void ShowReadyImages()
    {
        SetCenterImage(topCenterImage, topReadySprite, true);
        SetCenterImage(bottomCenterImage, bottomReadySprite, true);
    }

    public IEnumerator ShowCountdown()
    {
        if (countdownSprites == null || countdownSprites.Length == 0)
        {
            ClearMessage();
            yield break;
        }

        SetCenterImagesActive(true);

        foreach (Sprite countdownSprite in countdownSprites)
        {
            if (countdownSprite == null)
                continue;

            if (topCenterImage != null)
                topCenterImage.sprite = countdownSprite;

            if (bottomCenterImage != null)
                bottomCenterImage.sprite = countdownSprite;

            yield return new WaitForSeconds(countdownStepDuration);
        }

        ClearMessage();
    }

    public void ClearMessage()
    {
        SetCenterImagesActive(false);
    }

    private void UpdateWinImages()
    {
        if (raceManager == null)
            return;

        SetTrophyCount(
            elephantTrophies,
            elephantTrophyContainer,
            raceManager.ElephantWins
        );

        SetTrophyCount(
            mouseTrophies,
            mouseTrophyContainer,
            raceManager.MouseWins
        );
    }

    private void SetTrophyCount(
        List<Image> trophies,
        RectTransform container,
        int visibleCount)
    {
        if (trophyImagePrefab == null || container == null)
            return;

        while (trophies.Count < visibleCount)
        {
            Image trophy = Instantiate(trophyImagePrefab, container);
            trophy.gameObject.SetActive(true);
            trophies.Add(trophy);
        }

        for (int i = 0; i < trophies.Count; i++)
        {
            if (trophies[i] != null)
                trophies[i].gameObject.SetActive(i < visibleCount);
        }
    }

    private static void SetCenterImage(Image image, Sprite sprite, bool active)
    {
        if (image == null)
            return;

        image.sprite = sprite;
        image.gameObject.SetActive(active && sprite != null);
    }

    private void SetCenterImagesActive(bool active)
    {
        if (topCenterImage != null)
            topCenterImage.gameObject.SetActive(active);

        if (bottomCenterImage != null)
            bottomCenterImage.gameObject.SetActive(active);
    }
}
