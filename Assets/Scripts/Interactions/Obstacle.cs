using UnityEngine;

public sealed class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        ElephantController elephant = other.GetComponent<ElephantController>();

        if (elephant == null)
            return;

        elephant.CrashMomentum();
        Destroy(gameObject);
    }
}