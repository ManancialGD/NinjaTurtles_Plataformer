using UnityEngine;

public class DetectLeo : MonoBehaviour
{
    Transform leo;

    private void Awake()
    {
        leo = FindObjectOfType<LeoMovement>().transform;
    }

    private void Update()
    {
        if (leo.position.x < transform.position.x)
        {
            Rotate(true);
        }
        else if (leo.position.x > transform.position.x)
        {
            Rotate(false);
        }
    }

    private void Rotate(bool b)
    {
        if (b)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); 
        }
        else if (!b)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
