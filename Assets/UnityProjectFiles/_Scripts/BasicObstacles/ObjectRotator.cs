using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("The rotation amount applied to each axis per second")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 50, 0);

    [Tooltip("Should the rotation be relative to the object's local axes?")]
    [SerializeField] private bool useLocalSpace = true;

    void Update()
    {
        RotateObject();
    }

    private void RotateObject()
    {
        // Calculate the rotation step for this frame based on DeltaTime
        // (Step is 1.0f here as the actual speed is defined in rotationSpeed Vector)
        float step = 1.0f * Time.deltaTime;
        
        // Apply the rotation based on the chosen coordinate space
        if (useLocalSpace)
        {
            transform.Rotate(rotationSpeed * step, Space.Self);
        }
        else
        {
            transform.Rotate(rotationSpeed * step, Space.World);
        }
    }
}