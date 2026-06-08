using UnityEngine;

public sealed class Obstacle : MonoBehaviour
{
    [SerializeField] private CameraFollower cameraFollower;

    private void Awake()
    {
        if (cameraFollower == null)
            cameraFollower = FindFirstObjectByType<CameraFollower>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ElephantController elephant = other.GetComponent<ElephantController>();

        if (elephant == null)
            return;

        elephant.CrashMomentum();

        if (cameraFollower != null)
            cameraFollower.Shake();

        Destroy(gameObject);
    }
}