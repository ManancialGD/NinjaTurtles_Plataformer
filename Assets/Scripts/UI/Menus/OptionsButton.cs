using UnityEngine;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    public string Value { get; private set; } = null;
    [SerializeField] string[] options;
    [SerializeField] Font textFont;
    [SerializeField] Sprite buttonSprite;
    [SerializeField] Sprite buttonSpriteLast;
    [SerializeField] Vector2 size = new Vector2(100, 25);
    private Button[] buttons;
    private float animationTime = 0.3f;
    private Color defaultColor = new Color(118f / 255f, 118f / 255f, 118f / 255f);
    private Color overlapColor = new Color(130f / 255f, 130f / 255f, 130f / 255f);
    private Color pressedColor = new Color(140f / 255f, 140f / 255f, 140f / 255f);
    private Color selectedColor = new Color(50 / 255f, 222 / 255f, 84 / 255f); // verde

    void Start()
    {
        buttons = new Button[options.Length];

        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas não encontrado.");
            return;
        }

        for (int i = 0; i < options.Length; i++)
        {
            bool optionSelected = false;
            if (Value != null && Value.ToLower() == options[i].ToLower()) optionSelected = true;

            GameObject buttonObject = new GameObject("Button " + (i + 1));
            buttonObject.transform.SetParent(canvas.transform, false);
            RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
            Image image = buttonObject.AddComponent<Image>();

            Button newButton = buttonObject.AddComponent<Button>();


            if (i == options.Length - 1) image.sprite = buttonSpriteLast;//ultimo botão
            else image.sprite = buttonSprite;

            if (optionSelected) image.color = selectedColor;
            else image.color = defaultColor;


            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(buttonObject.transform);

            Text buttonText = textObject.AddComponent<Text>();
            RectTransform textRectTransform = textObject.GetComponent<RectTransform>();

            textRectTransform.sizeDelta = size;
            textRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textRectTransform.anchoredPosition = Vector2.zero;

            buttonText.fontSize = 10;
            buttonText.text = options[i];
            buttonText.alignment = TextAnchor.MiddleCenter;
            if (optionSelected) buttonText.color = Color.black;
            else buttonText.color = Color.white;
            buttonText.font = textFont;


            buttons[i] = newButton;

            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = new Vector2(0, 0);

            ColorBlock colors = newButton.colors;
            colors.normalColor = defaultColor;
            colors.highlightedColor = overlapColor;
            colors.pressedColor = pressedColor;
            colors.selectedColor = selectedColor;
            colors.disabledColor = Color.clear;
            colors.colorMultiplier = 1;
            newButton.colors = colors;

            newButton.interactable = true;
        }
    }

    void Update()
    {
        if (animationTime > 0) AnimateButton();
    }

    void AnimateButton()
    {
        int index = 0;
        foreach (Button button in buttons)
        {
            button.transform.localPosition = new Vector3(button.transform.localPosition.x, transform.position.y - index * (25 * ((0.3f - animationTime) / 0.3f)), button.transform.localPosition.z);
            index++;
        }
        if (animationTime > 0) animationTime -= Time.deltaTime;
        if (animationTime < 0) animationTime = 0;
    }
}