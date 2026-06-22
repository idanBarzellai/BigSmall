using UnityEngine;

public sealed class Obstacle : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        ElephantController elephant =
            other.GetComponentInParent<ElephantController>();

        if (elephant == null)
            return;

        elephant.CrashMomentum();
        AudioManager.Play(GameSound.ElephantOuch);

        Destroy(gameObject);
    }
}
