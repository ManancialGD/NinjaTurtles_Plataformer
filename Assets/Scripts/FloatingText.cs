using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 3f;
    private Vector3 Offset;

    void Start()
    {
        Offset = new Vector3(UnityEngine.Random.Range(-.4f * 32, .4f * 32), .5f * 32 + UnityEngine.Random.Range(0f, .1f * 32), 0);
        Destroy(gameObject, destroyTime);
        transform.localPosition += Offset;
    }
}
