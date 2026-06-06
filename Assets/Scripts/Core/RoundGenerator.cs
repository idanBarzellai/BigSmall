using System.Collections.Generic;
using UnityEngine;

public sealed class RoundGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject interactionAccessPointPrefab;
    [SerializeField] private GameObject mazeWallPrefab;
    [SerializeField] private GameObject finishLinePrefab;
    [SerializeField] private GameObject mouseConnectorPrefab;
    [SerializeField] private GameObject mouseMazeBoundaryPrefab;
    private readonly List<(float x, int connectorType)> generatedConnectors = new();

    [Header("References")]
    [SerializeField] private RaceManager raceManager;

    [Header("Track")]
    [SerializeField] private float trackLength = 80f;

    [Header("Generation")]
    [SerializeField] private int obstacleCount = 7;
    [SerializeField] private int interactionPointCount = 5;
    [SerializeField] private int mazeWallCount = 20;

    private readonly List<GameObject> spawnedObjects = new();

    public float FinishX => trackLength;

    public void GenerateRound()
{
    ClearRound();

    GenerateGround();
    GenerateFinishLine();
    GenerateElephantObstacles();

    GenerateMouseMazeWalls();
    GenerateMouseMazeBoundaries();

    GenerateInteractionPairs();
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
        Spawn(groundPrefab, new Vector3(trackLength * 0.5f, 0f, 0f), new Vector3(trackLength, 1f, 1f));
    }

    private void GenerateFinishLine()
    {
        GameObject finish = Spawn(finishLinePrefab, new Vector3(trackLength, 0f, 0f), new Vector3(0.3f, 8f, 1f));

        FinishLine finishLine = finish.GetComponent<FinishLine>();
        if (finishLine != null)
            finishLine.SetRaceManager(raceManager);
    }

    private void GenerateElephantObstacles()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            float x = Random.Range(8f, trackLength - 8f);

            Spawn(obstaclePrefab, new Vector3(x, 1.15f, 0f), new Vector3(0.8f, 0.8f, 1f));
        }
    }

    private void GenerateInteractionPairs()
{
    Color[] colors =
    {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    List<float> validInteractionXs = new();

    foreach (var connector in generatedConnectors)
    {
        if (connector.connectorType == 0)
        {
            validInteractionXs.Add(connector.x);
        }
    }

    for (int i = 0; i < interactionPointCount; i++)
    {
        float x;

        if (validInteractionXs.Count > 0)
        {
            int randomIndex = Random.Range(0, validInteractionXs.Count);
            x = validInteractionXs[randomIndex];
            validInteractionXs.RemoveAt(randomIndex);
        }
        else
        {
            x = Random.Range(10f, trackLength - 10f);
        }

        Color color = colors[i % colors.Length];

        GameObject controllerObject = new GameObject($"SharedInteraction_{i}");
        controllerObject.transform.SetParent(transform, false);

        SharedInteractionController controller =
            controllerObject.AddComponent<SharedInteractionController>();

        spawnedObjects.Add(controllerObject);

        GameObject elephantPointObject = Spawn(
            interactionAccessPointPrefab,
            new Vector3(x, 0.85f, 0f),
            Vector3.one * 0.55f
        );

        GameObject mousePointObject = Spawn(
            interactionAccessPointPrefab,
            new Vector3(x, -1f, 0f),
            Vector3.one * 0.45f
        );

        InteractionAccessPoint elephantPoint =
            elephantPointObject.GetComponent<InteractionAccessPoint>();

        InteractionAccessPoint mousePoint =
            mousePointObject.GetComponent<InteractionAccessPoint>();

        elephantPoint.Initialize(PlayerId.Elephant, controller, color);
        mousePoint.Initialize(PlayerId.Mouse, controller, color);

        controller.Initialize(elephantPoint, mousePoint);
    }
}

 private void GenerateMouseMazeWalls()
{
    const float segmentLength = 7f;

    float[] laneY =
    {
        -1f, // Top
        -2f, // Middle
        -3f  // Bottom
    };

    int segmentCount = Mathf.RoundToInt(trackLength / segmentLength);
generatedConnectors.Clear();
    for (int segment = 0; segment < segmentCount; segment++)
    {
        float segmentStartX = segment * segmentLength;
        float segmentCenterX = segmentStartX + segmentLength * 0.5f;

        int blockedLane = Random.Range(0, 3);

        for (int lane = 0; lane < 3; lane++)
        {
            if (lane == blockedLane)
            {
                GameObject wall = Spawn(
                    mazeWallPrefab,
                    new Vector3(segmentCenterX, laneY[lane], 0f),
new Vector3(segmentLength * 0.55f, 0.45f, 1f)                );

                wall.name = $"MazeWall_S{segment}_L{lane}";
            }
        }

        if (segment < segmentCount - 1)
        {
            int connectorType = Random.Range(0, 2);

            float connectorX = segmentStartX + segmentLength;
generatedConnectors.Add((connectorX, connectorType));
            if (connectorType == 0)
            {
                Spawn(
                    mouseConnectorPrefab,
                    new Vector3(connectorX, -1.5f, 0f),
new Vector3(0.4f, 1f, 1f)                );
            }
            else
            {
                Spawn(
                    mouseConnectorPrefab,
                    new Vector3(connectorX, -2.5f, 0f),
new Vector3(0.4f, 1f, 1f)                );
            }
        }
    }
}
private void GenerateMouseMazeBoundaries()
{
    const float gapWidth = 2f;

    GenerateBoundaryLine(-1.5f, 0);
    GenerateBoundaryLine(-2.5f, 1);

    void GenerateBoundaryLine(float y, int connectorType)
    {
        float currentX = 0f;

        foreach (var connector in generatedConnectors)
        {
            if (connector.connectorType != connectorType)
                continue;

            float leftLength = connector.x - gapWidth * 0.5f - currentX;

            if (leftLength > 0.1f)
            {
                Spawn(
                    mouseMazeBoundaryPrefab,
                    new Vector3(currentX + leftLength * 0.5f, y, 0f),
                    new Vector3(leftLength, 0.2f, 1f)
                );
            }

            currentX = connector.x + gapWidth * 0.5f;
        }

        float remainingLength = trackLength - currentX;

        if (remainingLength > 0.1f)
        {
            Spawn(
                mouseMazeBoundaryPrefab,
                new Vector3(currentX + remainingLength * 0.5f, y, 0f),
                new Vector3(remainingLength, 0.2f, 1f)
            );
        }
    }

    Spawn(
    mouseMazeBoundaryPrefab,
    new Vector3(trackLength * 0.5f, -3.5f, 0f),
    new Vector3(trackLength, 0.2f, 1f)
);
}

    private GameObject Spawn(GameObject prefab, Vector3 position, Vector3 scale)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity, transform);
        obj.transform.localScale = scale;
        spawnedObjects.Add(obj);
        return obj;
    }
}