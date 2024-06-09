using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    public string Value { get; private set; } = null;
    public bool MenuOpened { get; private set; } = false;
    private int actionMode = 0; // 1-opening 2-closing
    [SerializeField] Transform mainButton;
    [SerializeField] string[] options;
    [SerializeField] Font textFont;
    [SerializeField] Sprite spriteMain;
    [SerializeField] Sprite spriteMainSelected;
    [SerializeField] Sprite spriteMiddle;
    [SerializeField] Sprite spriteLast;
    [SerializeField] Vector2 size = new Vector2(100, 25);
    private Canvas canvas;
    private Button[] buttons;
    private float animationTime = 0.3f;
    private Color defaultColor = new Color(118f / 255f, 118f / 255f, 118f / 255f);
    private Color overlapColor = new Color(130f / 255f, 130f / 255f, 130f / 255f);
    private Color pressedColor = new Color(140f / 255f, 140f / 255f, 140f / 255f);
    private Color selectedColor = new Color(50 / 255f, 222 / 255f, 84 / 255f); // Green

    void Start()
    {
        buttons = new Button[options.Length];

        canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found.");
            return;
        }

        for (int i = 0; i < options.Length; i++)
        {
            bool optionSelected = false;
            if (Value != null && Value.ToLower() == options[i].ToLower()) optionSelected = true;

            GameObject buttonObject = new GameObject("Button " + (i + 1));
            buttonObject.transform.SetParent(canvas.transform, false);
            RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector3.zero;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = size;
            Image image = buttonObject.AddComponent<Image>();

            Button newButton = buttonObject.AddComponent<Button>();

            int optionIndex = i;

            newButton.onClick.AddListener(() => OnChooseOption(options[optionIndex]));

            if (i == options.Length - 1) image.sprite = spriteLast;//ultimo botão
            else image.sprite = spriteMain;

            if (optionSelected) image.color = selectedColor;
            else image.color = defaultColor;

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(buttonObject.transform);

            Text buttonText = textObject.AddComponent<Text>();
            RectTransform textRectTransform = textObject.GetComponent<RectTransform>();

            textRectTransform.sizeDelta = size;
            textRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textRectTransform.anchoredPosition = Vector3.zero;

            buttonText.fontSize = 10;
            buttonText.text = options[i];
            buttonText.alignment = TextAnchor.MiddleCenter;
            if (optionSelected) buttonText.color = Color.black;
            else buttonText.color = Color.white;
            buttonText.font = textFont;


            buttons[i] = newButton;

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
        mainButton.gameObject.transform.SetAsLastSibling();
    }

    void Update()
    {
        if (actionMode > 0) DisplayAnimation(actionMode);
    }

    void OpenMenu()
    {
        mainButton.gameObject.GetComponent<Image>().sprite = spriteMainSelected;

        Debug.Log("Open");
        animationTime = 0.3f;
        actionMode = 1;
        ChangeInteractability(true);

    }

    void CloseMenu()
    {
        mainButton.gameObject.GetComponent<Image>().sprite = spriteMainSelected;
        Debug.Log("Close");
        animationTime = 0.3f;
        actionMode = 2;

    }

    void DisplayAnimation(int mode)
    {
        if (mode == 1) // menu opening
        {
            int index = 0;
            foreach (Button button in buttons)
            {
                button.transform.position = new Vector3(mainButton.transform.position.x, mainButton.transform.position.y - (index + 1) * (25 * ((0.3f - animationTime) / 0.3f)), mainButton.transform.position.z);
                if (index < buttons.Length - 1 && animationTime < 0.2f) button.image.sprite = spriteMiddle;
                index++;
            }
            if (animationTime > 0) animationTime -= Time.deltaTime;
            if (animationTime <= 0)
            {
                ChangeInteractability(true);
                animationTime = 0;
                actionMode = 0;
            }
        }
        else if (mode == 2) // menu closing
        {
            int index = 0;
            foreach (Button button in buttons)
            {
                button.transform.position = new Vector3(mainButton.transform.position.x, mainButton.transform.position.y - (index + 1) * (25 * (animationTime / 0.3f)), mainButton.transform.position.z);
                if (index < buttons.Length - 1 && animationTime < 0.1f) button.image.sprite = spriteMain;
                index++;
            }
            if (animationTime > 0) animationTime -= Time.deltaTime;
            if (animationTime <= 0)
            {
                mainButton.gameObject.GetComponent<Image>().sprite = spriteMain;
                ChangeInteractability(false);
                animationTime = 0;
                actionMode = 0;
            }
        }

    }

    void ChangeInteractability(bool isInteractable)
    {
        foreach (Button button in buttons)
        {
            button.interactable = isInteractable;
            //button.transform.gameObject.SetActive(isInteractable);
        }
    }

    public void OnClick()
    {
        MenuOpened = !MenuOpened;
        if (MenuOpened) OpenMenu();
        else CloseMenu();
    }

    public void OnChooseOption(string option)
    {
        Value = option;
        mainButton.gameObject.GetComponentInChildren<TMP_Text>().text = option;
        MenuOpened = false;
        CloseMenu();
    }
}