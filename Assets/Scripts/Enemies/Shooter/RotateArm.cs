using System.Collections;
using UnityEngine;

public class RotateArm : MonoBehaviour
{
    // Speed at which the rotation occurs
    public float rotationSpeed = 10;

    // Reference to the target transform
    private Transform target;

    // Flag to check if the arm is currently under recoil
    private bool underRecoil;

    // Amount of recoil in degrees
    [SerializeField] private float recoilAmount = 90;

    // Speed of recoil
    [SerializeField] private float recoilSpeed = 35;

    void Awake()
    {
        // Find and assign the target (Leo) at the start
        target = FindObjectOfType<LeoMovement>().transform;
    }

    /// <summary>
    /// Sets the rotation angle of the arm.
    /// This method receives an angle in radians, converts it to degrees, 
    /// and applies it as a rotation to the object.
    /// If the player is on the left, the angle will be inverted.
    /// </summary>
    /// <param name="theta">The angle in radians to rotate the arm to.</param>
    public void SetRotationAngle(float theta)
    {
        // Exit if currently under recoil
        if (underRecoil) return;

        // Convert angle from radians to degrees
        float angle = theta * Mathf.Rad2Deg;

        // Ensure rotation only around the z-axis
        Quaternion newRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, angle);
        
        // Smoothly interpolate the current rotation to the new rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
    }

    /// <summary>
    /// Coroutine to handle the recoil effect.
    /// This smoothly rotates the arm by the specified recoil amount and then returns to the original position.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    public IEnumerator Recoil()
    {
        // Set underRecoil to true to prevent other rotations
        underRecoil = true;

        // Capture the starting angle
        float startAngle = transform.eulerAngles.z;

        // Calculate the end angle by adding the recoil amount
        float endAngle = startAngle + recoilAmount;

        // Initialize elapsed time
        float elapsedTime = 0;

        // Smoothly interpolate from start angle to end angle over the duration of the recoil speed
        while (elapsedTime < 1f / recoilSpeed)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, elapsedTime * recoilSpeed);
            Quaternion newRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, angle);
            transform.rotation = newRotation;

            // Increment elapsed time
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is set to the end angle
        Quaternion finalRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, endAngle);
        transform.rotation = finalRotation;

        // Reset underRecoil flag to allow other rotations
        underRecoil = false;
    }
}
