using UnityEngine;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    [SerializeField] string[] options;
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
            GameObject buttonObject = new GameObject("Button " + (i + 1));

            RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
            Image image = buttonObject.AddComponent<Image>();
            Button newButton = buttonObject.AddComponent<Button>();

            image.color = defaultColor; // Cor padrão do botão


            GameObject textObject = new GameObject("Text");
            Text buttonText = textObject.AddComponent<Text>();
            RectTransform textRectTransform = textObject.GetComponent<RectTransform>();

            textRectTransform.sizeDelta = new Vector2(100, 25);
            textRectTransform.SetParent(buttonObject.transform, false);
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.anchoredPosition = Vector2.zero;


            buttonText.text = options[i];
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.black;
            //buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); // Define a fonte


            buttonObject.transform.SetParent(canvas.transform, false);


            buttons[i] = newButton;

            rectTransform.sizeDelta = new Vector2(100, 25);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
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