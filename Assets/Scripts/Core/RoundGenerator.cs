using System.Collections.Generic;
using UnityEngine;

public sealed class RoundGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject interactionPointPrefab;
    [SerializeField] private GameObject mazeWallPrefab;
    [SerializeField] private GameObject finishLinePrefab;

    [Header("References")]
    [SerializeField] private RaceManager raceManager;

    [Header("Track")]
    [SerializeField] private float trackLength = 80f;

    [Header("Generation")]
    [SerializeField] private int obstacleCount = 7;
    [SerializeField] private int interactionPointCount = 5;
    [SerializeField] private int mazeWallCount = 18;

    private readonly List<GameObject> spawnedObjects = new();

    public float FinishX => trackLength;

    public void GenerateRound()
    {
        ClearRound();

        GenerateGround();
        GenerateFinishLine();
        GenerateElephantObstacles();
        GenerateInteractionPoints();
        GenerateMouseMazeWalls();
    }

    private void ClearRound()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedObjects.Clear();
    }

    private void GenerateGround()
    {
        GameObject ground = Spawn(
            groundPrefab,
            new Vector3(trackLength * 0.5f, 0f, 0f),
            new Vector3(trackLength, 1f, 1f)
        );

        ground.name = "Ground";
    }

    private void GenerateFinishLine()
    {
        GameObject finish = Spawn(
            finishLinePrefab,
            new Vector3(trackLength, 0f, 0f),
            new Vector3(0.3f, 8f, 1f)
        );

        finish.name = "FinishLine";

        FinishLine finishLine = finish.GetComponent<FinishLine>();
        if (finishLine != null)
        {
            finishLine.SetRaceManager(raceManager);
        }
    }

    private void GenerateElephantObstacles()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            float x = Random.Range(8f, trackLength - 8f);

            GameObject obstacle = Spawn(
                obstaclePrefab,
                new Vector3(x, 1.15f, 0f),
                new Vector3(0.8f, 0.8f, 1f)
            );

            obstacle.name = $"ElephantObstacle_{i}";
        }
    }

    private void GenerateInteractionPoints()
    {
        Color[] colors =
        {
            Color.blue,
            Color.green,
            Color.yellow,
            Color.magenta,
            Color.cyan
        };

        for (int i = 0; i < interactionPointCount; i++)
        {
            float x = Random.Range(10f, trackLength - 10f);

            GameObject point = Spawn(
                interactionPointPrefab,
                new Vector3(x, 0.85f, 0f),
                Vector3.one * 0.55f
            );

            point.name = $"InteractionPoint_{i}";

            SpriteRenderer sr = point.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = colors[i % colors.Length];
        }
    }

    private void GenerateMouseMazeWalls()
    {
        for (int i = 0; i < mazeWallCount; i++)
        {
            float x = Random.Range(5f, trackLength - 5f);
            float y = Random.Range(-3.4f, -1.2f);
            float height = Random.Range(0.8f, 2.2f);

            GameObject wall = Spawn(
                mazeWallPrefab,
                new Vector3(x, y, 0f),
                new Vector3(0.35f, height, 1f)
            );

            wall.name = $"MouseMazeWall_{i}";
        }
    }

    private GameObject Spawn(GameObject prefab, Vector3 position, Vector3 scale)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity, transform);
        obj.transform.localScale = scale;
        spawnedObjects.Add(obj);
        return obj;
    }
}