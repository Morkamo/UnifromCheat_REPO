using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO.Utils
{
    public class HintsController : MonoBehaviour
    {
        public static Canvas globalCanvas;

        public static TextMeshProUGUI CreateHint(string text, string objectName = "UITextObject", float xOffset = 0, float yOffset = 0, int fontSize = 36, Color? color = null, float width = 0)
        {
            if (globalCanvas == null)
            {
                GameObject canvasObject = new GameObject("UnifromCanvas");

                globalCanvas = canvasObject.AddComponent<Canvas>();
                globalCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.matchWidthOrHeight = 0.5f;

                canvasObject.AddComponent<GraphicRaycaster>();
                
                canvasObject.transform.SetParent(null);
                globalCanvas.sortingOrder = 100;

                DontDestroyOnLoad(canvasObject);
            }
            
            GameObject UITextObject = new GameObject(objectName);

            TextMeshProUGUI textMesh = UITextObject.AddComponent<TextMeshProUGUI>();

            textMesh.text = $"<b>{text}</b>";
            textMesh.fontSize = fontSize;
            textMesh.color = color ?? Color.white;
            textMesh.enableWordWrapping = false;

            RectTransform rectTransform = textMesh.GetComponent<RectTransform>();
            UITextObject.transform.SetParent(globalCanvas.transform, false);
            UnifromCheat_REPO.Core.UnifromHints.Add(UITextObject);
            
            rectTransform.sizeDelta = new Vector2(width, fontSize * 2);
            rectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            

            FireLog($"[TextController] Created text '{text}' at {rectTransform.anchoredPosition}");
            
            return textMesh;
        }

        private void Start()
        {
            if (globalCanvas != null)
                globalCanvas.gameObject.SetActive(true);
            
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                DontDestroyOnLoad(eventSystem);
                FireLog("[TextController] Created new EventSystem.");
            }
        }
    }
}
