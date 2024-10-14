using UnityEngine;

public class Follower : MonoBehaviour
{
    // Reference to the marble GameObject
    [SerializeField] private Transform marble;

    private void Update()
    {
        if (marble != null)
        {
            // Move the follower to the marble's position
            transform.position = marble.position;
        }
    }
}