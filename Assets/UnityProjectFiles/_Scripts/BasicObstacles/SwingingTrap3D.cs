using UnityEngine;

public class SwingingTrap3D : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }
    public enum StartSide { Right, Left }

    [Header("Swinging Settings")]
    [SerializeField] private RotationAxis axisToRotate = RotationAxis.X;
    [SerializeField] private StartSide startingSide = StartSide.Right;

    [Tooltip("The maximum angle the trap swings to either side")]
    [Range(0f, 180f)]
    [SerializeField] private float angleLimit = 45f;

    [Tooltip("The speed of the swinging cycle")]
    [SerializeField] private float speed = 2f;

    [Tooltip("Manual time offset to sync or desync multiple traps")]
    [SerializeField] private float manualTimeOffset = 0f;

    private Quaternion startRotation;

    void Start()
    {
        // Store the initial rotation to use as a baseline for the swing
        startRotation = transform.rotation;
    }

    void Update()
    {
        ApplySwinging();
    }

    private void ApplySwinging()
    {
        // If the user chooses to start from the left, we add a PI offset
        // because Sin(x + PI) is the exact inverse of Sin(x)
        float directionOffset = (startingSide == StartSide.Left) ? Mathf.PI : 0f;

        // Calculate the current angle using the Sine wave for smooth back-and-forth motion
        float currentAngle = Mathf.Sin(Time.time * speed + directionOffset + manualTimeOffset) * angleLimit;

        Quaternion finalRotation = Quaternion.identity;

        // Apply the calculated angle to the specified axis
        switch (axisToRotate)
        {
            case RotationAxis.X:
                finalRotation = Quaternion.Euler(currentAngle, 0, 0);
                break;
            case RotationAxis.Y:
                finalRotation = Quaternion.Euler(0, currentAngle, 0);
                break;
            case RotationAxis.Z:
                finalRotation = Quaternion.Euler(0, 0, currentAngle);
                break;
        }

        // Multiply by the start rotation to maintain the object's original orientation
        transform.rotation = startRotation * finalRotation;
    }
}