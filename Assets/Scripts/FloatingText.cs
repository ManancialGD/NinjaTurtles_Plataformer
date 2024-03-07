using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 3f;
    private Vector3 Offset;

    void Start()
    {
        Offset = new Vector3(UnityEngine.Random.Range(-.4f, .4f), .5f + UnityEngine.Random.Range(0f, .1f), 0);
        Destroy(gameObject, destroyTime);
        transform.localPosition += Offset;
    }
}
