using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ElephantVsMouse.Gameplay.Interactions
{
    public sealed class ScreenObscurer : MonoBehaviour
    {
        private GameObject panelObject;

        private void Awake()
        {
            CreateOverlay();
        }

        private void CreateOverlay()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("_ObscureCanvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            panelObject = new GameObject("ElephantObscurePanel");
            panelObject.transform.SetParent(canvas.transform, false);

            Image image = panelObject.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.85f);

            RectTransform rt = panelObject.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.sizeDelta = Vector2.zero;

            panelObject.SetActive(false);
        }

        public void Obscure(float seconds)
        {
            if (panelObject == null) return;
            StopAllCoroutines();
            StartCoroutine(ObscureRoutine(seconds));
        }

        private IEnumerator ObscureRoutine(float seconds)
        {
            panelObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            panelObject.SetActive(false);
        }
    }
}
