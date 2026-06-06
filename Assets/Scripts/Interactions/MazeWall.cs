using UnityEngine;

public sealed class MazeWall : MonoBehaviour
{
    private bool moved;

    public void EarthquakeShift()
{
    if (moved)
        return;

    moved = true;

    float direction = Random.value > 0.5f ? 1f : -1f;

    transform.position += new Vector3(0f, direction * 2f, 0f);

    Debug.Log($"{name} shifted by earthquake");
}
}