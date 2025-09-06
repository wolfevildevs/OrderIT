using UnityEngine;

public class RotateOBS : MonoBehaviour
{
    public float rotationSpeed = 100f;

    public enum RotationAxis { X, Y, Z }
    public RotationAxis chosenAxis = RotationAxis.X;

    void Update()
    {
        Vector3 axis = Vector3.zero;

        switch (chosenAxis)
        {
            case RotationAxis.X:
                axis = Vector3.right;
                break;
            case RotationAxis.Y:
                axis = Vector3.up;
                break;
            case RotationAxis.Z:
                axis = Vector3.forward;
                break;
        }

        transform.Rotate(axis * rotationSpeed * Time.deltaTime);
    }
}