using System.Collections;
using Jade.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Jade.World
{
    [RequireComponent(typeof(Collider2D))]
    public class EndingSequence2D : MonoBehaviour
    {
        [SerializeField, TextArea(3, 8)]
        private string endingText =
            "玉光自洞心沉静，山海的回声渐渐远去。\n" +
            "你已穿过三重幽窟，也听见了门后未尽的风。\n" +
            "此行暂歇，新的传说，仍在青石与云影之间等候。";

        [SerializeField] private string footerText = "即将返回主菜单";
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private float fadeSeconds = 1.25f;
        [SerializeField] private float holdSeconds = 4f;

        private bool triggered;

        private void Reset()
        {
            Collider2D trigger = GetComponent<Collider2D>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerMotor2D player = other.GetComponent<PlayerMotor2D>();
            if (triggered || player == null)
            {
                return;
            }

            triggered = true;
            DisablePlayerControl(player);
            StartCoroutine(PlayEnding());
        }

        private IEnumerator PlayEnding()
        {
            CanvasGroup canvasGroup = CreateEndingCanvas();
            float fadeDuration = Mathf.Max(0.01f, fadeSeconds);
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, holdSeconds));

            Time.timeScale = 1f;
            if (!string.IsNullOrWhiteSpace(mainMenuSceneName))
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }

        private static void DisablePlayerControl(PlayerMotor2D player)
        {
            Rigidbody2D body = player.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.velocity = Vector2.zero;
            }

            player.enabled = false;
        }

        private CanvasGroup CreateEndingCanvas()
        {
            GameObject canvasObject = new GameObject("EndingSequenceCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            CanvasGroup canvasGroup = canvasObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            Image backdrop = CreateImage("EndingBackdrop", canvasObject.transform, new Color(0.02f, 0.025f, 0.025f, 0.92f));
            StretchToParent(backdrop.rectTransform);

            Text bodyText = CreateText("EndingText", canvasObject.transform, endingText, 42, new Color(0.86f, 1f, 0.92f, 1f));
            RectTransform bodyRect = bodyText.rectTransform;
            bodyRect.anchorMin = new Vector2(0.12f, 0.35f);
            bodyRect.anchorMax = new Vector2(0.88f, 0.72f);
            bodyRect.offsetMin = Vector2.zero;
            bodyRect.offsetMax = Vector2.zero;

            Text footer = CreateText("EndingFooter", canvasObject.transform, footerText, 24, new Color(0.7f, 0.86f, 0.78f, 0.9f));
            RectTransform footerRect = footer.rectTransform;
            footerRect.anchorMin = new Vector2(0.2f, 0.16f);
            footerRect.anchorMax = new Vector2(0.8f, 0.24f);
            footerRect.offsetMin = Vector2.zero;
            footerRect.offsetMax = Vector2.zero;

            return canvasGroup;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            GameObject imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string name, Transform parent, string text, int size, Color color)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            Text label = textObject.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = size;
            label.color = color;
            label.alignment = TextAnchor.MiddleCenter;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = Mathf.Max(14, size / 2);
            label.resizeTextMaxSize = size;
            label.lineSpacing = 1.25f;
            return label;
        }

        private static void StretchToParent(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
