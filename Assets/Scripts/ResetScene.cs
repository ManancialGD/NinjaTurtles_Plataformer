using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    public string resetButtonName = "Reset";

    // Update é chamado a cada frame
    void Update()
    {
        if (Input.GetButtonDown(resetButtonName))
        {
            SceneManager.LoadScene("Tutorial");
        }
    }
}
