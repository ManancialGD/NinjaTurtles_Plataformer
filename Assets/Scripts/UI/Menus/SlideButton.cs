using UnityEngine;
using UnityEngine.UI;

public class SlideButton : MonoBehaviour
{
    Image[] images; // Array to hold Image components of the button.
    MenuManager menuManager; // Reference to the MenuManager script.
    bool canOperate = true; // Flag to check if the button can operate.
    Vector3 correctPosition = new Vector3(-11.5f, 0f, 0f); // Target position for the sliding button.
    float momentum = 0f; // Momentum value for smooth sliding.
    [SerializeField] public bool Value { get; private set; } // Value representing the button state.
    Color defaultColor; // Default color of the button.
    Vector3 defaultPosition; // Default position of the button.

    void Awake()
    {
        images = GetComponentsInChildren<Image>(); // Get all Image components in child objects.
        if (images.Length > 1)
        {
            defaultColor = images[1].color; // Set the default color from the second image.
            defaultPosition = images[1].rectTransform.localPosition; // Set the default position.
        }
        menuManager = FindObjectOfType<MenuManager>(); // Find the MenuManager in the scene.
        if (menuManager == null) canOperate = false; // Disable operation if no MenuManager found.
        Value = menuManager.DoubleClickDash; // Set Value based on MenuManager's DoubleClickDash.
        if (Value)
        {
            correctPosition = new Vector3(11.5f, defaultPosition.y, defaultPosition.z);
            images[1].color = new Color(50 / 255f, 222 / 255f, 84 / 255f);
        }
        else correctPosition = new Vector3(-11.5f, defaultPosition.y, defaultPosition.z);
        images[1].rectTransform.localPosition = correctPosition; // Set initial position.
    }

    void Update()
    {
        // Update correct position based on Value.
        if (Value && correctPosition != new Vector3(11.5f, defaultPosition.y, defaultPosition.z)) correctPosition = new Vector3(11.5f, defaultPosition.y, defaultPosition.z);
        else if (!Value && correctPosition != new Vector3(-11.5f, defaultPosition.y, defaultPosition.z)) correctPosition = new Vector3(-11.5f, defaultPosition.y, defaultPosition.z);

        // Ensure the button snaps to the correct position if it's too far off.
        if (Mathf.Abs(images[1].rectTransform.localPosition[0] + correctPosition[0]) > Mathf.Abs(correctPosition[0] * 2))
        {
            images[1].rectTransform.localPosition = correctPosition;
            momentum = 0f;
        }

        // Smoothly slide the button towards the correct position.
        if (Mathf.Abs(images[1].rectTransform.localPosition[0] - correctPosition[0]) > 0.01f)
        {
            momentum += (correctPosition[0] / Mathf.Abs(correctPosition[0])) * 0.01f;
            images[1].rectTransform.localPosition = new Vector3(images[1].rectTransform.localPosition[0] + momentum, defaultPosition.y, defaultPosition.z);
        }
        SetButtonColor(); // Update the button color based on its position.

        if (images[1].rectTransform.localPosition.x > 11.5) images[1].rectTransform.localPosition = new Vector3(11.5f, images[1].rectTransform.localPosition.y, images[1].rectTransform.localPosition.z);
        else if (images[1].rectTransform.localPosition.x < -11.5) images[1].rectTransform.localPosition = new Vector3(-11.5f, images[1].rectTransform.localPosition.y, images[1].rectTransform.localPosition.z);

    }

    void SetButtonColor()
    {
        if (momentum == 0) return;

        float lerpValue = (images[1].rectTransform.localPosition.x + 11.5f) / 23f;
        lerpValue = Mathf.Clamp01(lerpValue); // Ensure lerpValue is between 0 and 1.

        Color targetColor = new Color(50 / 255f, 222 / 255f, 84 / 255f);
        images[1].color = Color.Lerp(defaultColor, targetColor, lerpValue); // Lerp color based on position.
    }

    public void SlideButtonClicked()
    {
        if (!canOperate) return;
        Value = !Value; // Toggle the value.
        menuManager.ChangeDashType(); // Notify the MenuManager of the change.
    }
}
