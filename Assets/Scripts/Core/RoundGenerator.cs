using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class MouseMazeSegment
{
    public float startX;
    public float endX;
    public int openLane;
    public bool earthquakeBlocked;
}

public sealed class RoundGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject interactionAccessPointPrefab;
    [SerializeField] private GameObject birdAttackPrefab;
[SerializeField] private ElephantController elephant;
[SerializeField] private MouseController mouse;

[SerializeField] private ScreenObscurer screenObscurer;
    [SerializeField] private GameObject mazeWallPrefab;
    [SerializeField] private GameObject earthquakeWallPrefab;
    [SerializeField] private GameObject finishLinePrefab;
    [SerializeField] private GameObject mouseConnectorPrefab;
    [SerializeField] private GameObject mouseMazeBoundaryPrefab;
    private readonly List<MouseMazeSegment> mouseMazeSegments = new();
    private readonly List<(float x, int connectorType)> generatedConnectors = new();

    [Header("References")]
    [SerializeField] private RaceManager raceManager;

    [Header("Track")]
    [SerializeField] private float trackLength = 80f;

    [Header("Generation")]
    [SerializeField] private int obstacleCount = 7;
    [SerializeField] private int interactionPointCount = 5;

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
        // mazeWalls.Clear();
        mouseMazeSegments.Clear();
    }

    private void GenerateGround()
    {
        Spawn(groundPrefab, new Vector3(trackLength * 0.5f, 0f, 0f), new Vector3(trackLength, 0.55f, 1f));
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

           controller.SetupReferences(
    birdAttackPrefab,
    elephant.transform,
    screenObscurer,
    this,
    mouse.transform
);

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
// private readonly List<MazeWall> mazeWalls = new();

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
    mouseMazeSegments.Clear();

    for (int segment = 0; segment < segmentCount; segment++)
    {
        float segmentStartX = segment * segmentLength;
        float segmentEndX = segmentStartX + segmentLength;
        float segmentCenterX = segmentStartX + segmentLength * 0.5f;

        int firstBlockedLane = Random.Range(0, 3);
        int secondBlockedLane = Random.Range(0, 3);

        while (secondBlockedLane == firstBlockedLane)
        {
            secondBlockedLane = Random.Range(0, 3);
        }

        int openLane = 0;

        for (int lane = 0; lane < 3; lane++)
        {
            if (lane != firstBlockedLane && lane != secondBlockedLane)
            {
                openLane = lane;
                break;
            }
        }

        mouseMazeSegments.Add(new MouseMazeSegment
        {
            startX = segmentStartX,
            endX = segmentEndX,
            openLane = openLane,
            earthquakeBlocked = false
        });

        CreateMazeWall(segment, firstBlockedLane, segmentCenterX, segmentLength, laneY);
        CreateMazeWall(segment, secondBlockedLane, segmentCenterX, segmentLength, laneY);

        if (segment < segmentCount - 1)
        {
            int connectorType = Random.Range(0, 2);
            float connectorX = segmentEndX;

            generatedConnectors.Add((connectorX, connectorType));

            if (connectorType == 0)
            {
                Spawn(
                    mouseConnectorPrefab,
                    new Vector3(connectorX, -1.5f, 0f),
                    new Vector3(0.4f, 1f, 1f)
                );
            }
            else
            {
                Spawn(
                    mouseConnectorPrefab,
                    new Vector3(connectorX, -2.5f, 0f),
                    new Vector3(0.4f, 1f, 1f)
                );
            }
        }
    }
}

public void TriggerEarthquake(float mouseX)
{
    float[] laneY =
    {
        -1f,
        -2f,
        -3f
    };

    foreach (MouseMazeSegment segment in mouseMazeSegments)
    {
        if (segment.earthquakeBlocked)
            continue;

        if (segment.startX <= mouseX)
            continue;

        float centerX = (segment.startX + segment.endX) * 0.5f;
        float segmentLength = segment.endX - segment.startX;

        GameObject blocker = Spawn(
            earthquakeWallPrefab != null ? earthquakeWallPrefab : mazeWallPrefab,
            new Vector3(centerX, laneY[segment.openLane], 0f),
            new Vector3(segmentLength * 0.9f, 0.75f, 1f)
        );

        blocker.name = $"EarthquakeBlocker_Lane_{segment.openLane}";

        segment.earthquakeBlocked = true;

        Debug.Log($"Earthquake blocked lane {segment.openLane}");

        return;
    }

    Debug.Log("No valid future segment to block");
}

private void CreateMazeWall(
    int segment,
    int lane,
    float segmentCenterX,
    float segmentLength,
    float[] laneY)
{
    GameObject wall = Spawn(
        mazeWallPrefab,
        new Vector3(segmentCenterX, laneY[lane], 0f),
        new Vector3(segmentLength * 0.7f, 0.45f, 1f)
    );

    wall.name = $"MazeWall_S{segment}_L{lane}";

    // MazeWall mazeWall = wall.GetComponent<MazeWall>();

    // if (mazeWall != null)
    // {
    //     mazeWalls.Add(mazeWall);
    // }
}
private void GenerateMouseMazeBoundaries()
{

    // GenerateBoundaryLine(-1.5f, 0);
    // GenerateBoundaryLine(-2.5f, 1);

    // void GenerateBoundaryLine(float y, int connectorType)
    // {
    //     float currentX = 0f;

    //     foreach (var connector in generatedConnectors)
    //     {
    //         if (connector.connectorType != connectorType)
    //             continue;

    //         float leftLength = connector.x - gapWidth * 0.5f - currentX;

    //         if (leftLength > 0.1f)
    //         {
    //             Spawn(
    //                 mouseMazeBoundaryPrefab,
    //                 new Vector3(currentX + leftLength * 0.5f, y, 0f),
    //                 new Vector3(leftLength, 0.2f, 1f)
    //             );
    //         }

    //         currentX = connector.x + gapWidth * 0.5f;
    //     }

    //     float remainingLength = trackLength - currentX;

    //     if (remainingLength > 0.1f)
    //     {
    //         Spawn(
    //             mouseMazeBoundaryPrefab,
    //             new Vector3(currentX + remainingLength * 0.5f, y, 0f),
    //             new Vector3(remainingLength, 0.2f, 1f)
    //         );
    //     }
    // }

    Spawn(
    mouseMazeBoundaryPrefab,
new Vector3(trackLength * 0.5f, -3.8f, 0f),
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