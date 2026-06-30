using UnityEngine;

public class CollectibleBox : MonoBehaviour
{
    private bool isCollected = false;
    private Collider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<Collider>();
    }

    public bool Collect()
    {
        // Prevents double collection bugs if frames overlap
        if (isCollected) return false;

        isCollected = true;
        
        // Disable physics collider once collected so it doesn't re-trigger
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        return true;
    }
}