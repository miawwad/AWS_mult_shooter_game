using UnityEngine;

public class TargetRotateZAxis : MonoBehaviour
{
    public float rotationAngle = 30f; // How much to rotate left and right
    public float rotationSpeed = 2f;  // How fast the rotation swings

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        float zRotation = Mathf.Sin(Time.time * rotationSpeed) * rotationAngle;
        transform.rotation = startRotation * Quaternion.Euler(0, 0, zRotation);
    }
}
