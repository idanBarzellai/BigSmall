using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class ScreenObscurer : MonoBehaviour
{
    [SerializeField] private Image coverImage;
    [SerializeField] private float defaultDuration = 2.5f;

    private Coroutine routine;

    private void Awake()
    {

        if (coverImage != null)
            coverImage.gameObject.SetActive(false);
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
        coverImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        coverImage.gameObject.SetActive(false);
    }
}