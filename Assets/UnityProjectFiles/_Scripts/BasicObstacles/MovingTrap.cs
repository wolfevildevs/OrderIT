using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The starting position of the trap")]
    [SerializeField] private Vector3 startPoint;
    
    [Tooltip("The target position the trap moves towards")]
    [SerializeField] private Vector3 endPoint;

    [Tooltip("Movement speed")]
    [SerializeField] private float speed = 2.0f;

    [Tooltip("Wait time at each point before heading back")]
    [SerializeField] private float delayTime = 0.5f;

    [Header("Checkpoint Settings")]
    [Tooltip("If true, the trap will start its motion from the End Point")]
    [SerializeField] private bool startAtEnd = false;

    private Vector3 targetPoint;
    private float nextMoveTime;

    void Start()
    {
        // Initialize position based on the chosen starting side
        if (startAtEnd)
        {
            transform.position = endPoint;
            targetPoint = startPoint;
        }
        else
        {
            transform.position = startPoint;
            targetPoint = endPoint;
        }
    }

    void Update()
    {
        // Only move if the delay time has passed
        if (Time.time >= nextMoveTime)
        {
            MoveObject();
        }
    }

    private void MoveObject()
    {
        // Smoothly move towards the target point
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        // Check if the object reached the destination
        if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
        {
            // Switch target between Start and End points
            targetPoint = (targetPoint == startPoint) ? endPoint : startPoint;
            
            // Set the timer for the next movement
            nextMoveTime = Time.time + delayTime;
        }
    }

    // Visualize the movement path in the Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(startPoint, 0.2f);
        Gizmos.DrawSphere(endPoint, 0.2f);
        Gizmos.DrawLine(startPoint, endPoint);
    }

    // Context menu shortcuts to set points easily in the Inspector
    [ContextMenu("Set Start Point")] void SetStart() => startPoint = transform.position;
    [ContextMenu("Set End Point")]   void SetEnd()   => endPoint = transform.position;
}