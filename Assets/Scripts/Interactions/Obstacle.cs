using UnityEngine;

public enum ObstacleEffectType
{
    ResetMomentum,
    SlowAcceleration
}

public sealed class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleEffectType effectType = ObstacleEffectType.ResetMomentum;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ElephantController elephant = other.GetComponent<ElephantController>();

        if (elephant == null)
            return;

        switch (effectType)
        {
            case ObstacleEffectType.ResetMomentum:
                elephant.ResetMomentum();
                Destroy(gameObject);
                break;

            case ObstacleEffectType.SlowAcceleration:
                elephant.ResetMomentum();
                // Later we will add temporary acceleration reduction.
                Destroy(gameObject);
                break;
        }
    }
}