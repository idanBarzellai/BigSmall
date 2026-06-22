using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class ScreenObscurer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image coverImage;

    [Header("PNG Sequence")]
    [SerializeField] private Sprite[] frames;
    [SerializeField, Min(1f)] private float framesPerSecond = 12f;
    [SerializeField] private bool preserveAspect = true;
    [SerializeField] private Vector2 sequenceAnchoredPosition;

    [Header("Timing")]
    [SerializeField] private float defaultDuration = 2.5f;

    private Coroutine routine;

    private void Awake()
    {
        if (coverImage != null)
        {
            coverImage.color = Color.white;
            coverImage.preserveAspect = preserveAspect;
            coverImage.rectTransform.anchoredPosition = sequenceAnchoredPosition;
            coverImage.gameObject.SetActive(false);
        }
    }

    public void Obscure()
    {
        Obscure(defaultDuration);
    }

    public void Obscure(float duration)
    {
        if (coverImage == null)
            return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ObscureRoutine(duration));
    }

    private IEnumerator ObscureRoutine(float duration)
    {
        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning("ScreenObscurer has no PNG sequence frames assigned.", this);
            routine = null;
            yield break;
        }

        coverImage.color = Color.white;
        coverImage.preserveAspect = preserveAspect;
        coverImage.rectTransform.anchoredPosition = sequenceAnchoredPosition;
        coverImage.sprite = frames[0];
        coverImage.gameObject.SetActive(true);

        float elapsed = 0f;
        float playbackRate = Mathf.Max(framesPerSecond, 1f);

        while (elapsed < duration)
        {
            int frameIndex = Mathf.Min(
                Mathf.FloorToInt(elapsed * playbackRate),
                frames.Length - 1
            );
            coverImage.sprite = frames[frameIndex];

            elapsed += Time.deltaTime;
            yield return null;
        }

        coverImage.gameObject.SetActive(false);
        routine = null;
    }
}
