using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    public float rotationSpeed;
    Transform target;
    
    void Start()
    {
        target = FindObjectOfType<Player>().transform; // Finds the player as the target
    }

    /// <summary>
    /// This Method will receive the Theta forneced from the ComputeVelocity from EnemyAim
    /// and will use that value to rotate the object.
    /// if the player is on the left, the angle will be inverted
    /// </summary>
    /// <param name="theta"></param>
    public void SetRotationAngle(float theta)
    {
        float angle = theta * Mathf.Rad2Deg; // Convert from radians to degrees

        // If the player's position is less than the game object's position, set the angle to negative
        if (target.position.x < gameObject.transform.position.x)
        {
            angle = -angle;
        }

        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);
    }
}
