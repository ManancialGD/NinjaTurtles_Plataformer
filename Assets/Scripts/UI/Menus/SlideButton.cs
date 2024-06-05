using UnityEngine;
using UnityEngine.UI;

public class SlideButton : MonoBehaviour
{
    Image[] images;
    MenuManager menuManager;
    bool canOperate = true;
    Vector3 correctPosition = new Vector3(-11.5f, 0f, 0f);
    float mumentum = 0f;
    [SerializeField] public bool Value { get; private set; }
    Color defaultColor;
    Vector3 defaultPosition;
    // Start is called before the first frame update
    void Start()
    {
        images = GetComponentsInChildren<Image>();
        if (images.Length > 1)
        {
            defaultColor = images[1].color;
            defaultPosition = images[1].rectTransform.localPosition;
        }
        menuManager = FindObjectOfType<MenuManager>();
        if (menuManager == null) canOperate = false;
        Value = menuManager.DoubleClickDash;
        if (Value) correctPosition = new Vector3(11.5f, defaultPosition.y, defaultPosition.z);
        else correctPosition = new Vector3(-11.5f, defaultPosition.y, defaultPosition.z);
        images[1].rectTransform.localPosition = correctPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Value && correctPosition != new Vector3(11.5f, defaultPosition.y, defaultPosition.z)) correctPosition = new Vector3(11.5f, defaultPosition.y, defaultPosition.z);
        else if (!Value && correctPosition != new Vector3(-11.5f, defaultPosition.y, defaultPosition.z)) correctPosition = new Vector3(-11.5f, defaultPosition.y, defaultPosition.z);

        if (Mathf.Abs(images[1].rectTransform.localPosition[0] + correctPosition[0]) > Mathf.Abs(correctPosition[0] * 2))
        {
            images[1].rectTransform.localPosition = correctPosition;
            mumentum = 0f;
        }

        if (Mathf.Abs(images[1].rectTransform.localPosition[0] - correctPosition[0]) > 0.01f)
        {
            mumentum += (correctPosition[0] / Mathf.Abs(correctPosition[0])) * 0.01f;
            images[1].rectTransform.localPosition = new Vector3(images[1].rectTransform.localPosition[0] + mumentum, defaultPosition.y, defaultPosition.z);
        }
        SetButtonColor();
    }

    void SetButtonColor()
    {
        if (mumentum == 0) return;

        float lerpValue = (images[1].rectTransform.localPosition.x + 11.5f) / 23f;

        // (0 e 1)
        lerpValue = Mathf.Clamp01(lerpValue);

        Color targetColor = new Color(50 / 255f, 222 / 255f, 84 / 255f);

        images[1].color = Color.Lerp(defaultColor, targetColor, lerpValue);
    }

    public void SlideButtonClicked()
    {
        if (!canOperate) return;
        Value = !Value;
        menuManager.ChangeDashType();
    }
}
