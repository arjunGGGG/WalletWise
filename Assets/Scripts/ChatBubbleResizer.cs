using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ChatBubbleResizer : MonoBehaviour
{
    public TextMeshProUGUI chatText;
    public RectTransform bubbleBackground;
    public Vector2 padding = new Vector2(20f, 15f);
    public float baseHeight = 50f;
    public float heightPerLine = 25f;

    private void Start()
    {
        ResizeBubble();
    }

    public void SetText(string message)
    {
        chatText.text = message;
        ResizeBubble();
    }

    private void ResizeBubble()
    {
        chatText.enableWordWrapping = true;

        chatText.ForceMeshUpdate();

        int numberOfLines = chatText.textInfo.lineCount;

        float bubbleHeight = baseHeight + (numberOfLines - 1) * heightPerLine + padding.y;

        bubbleBackground.sizeDelta = new Vector2(bubbleBackground.sizeDelta.x, bubbleHeight);

        chatText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bubbleHeight - padding.y);

        bubbleBackground.anchorMin = new Vector2(0.5f, 0.5f);
        bubbleBackground.anchorMax = new Vector2(0.5f, 0.5f);
        bubbleBackground.pivot = new Vector2(0.5f, 0.5f);
        bubbleBackground.anchoredPosition = Vector2.zero;
    }
}
