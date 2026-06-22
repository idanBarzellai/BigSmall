using System.Collections;
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

public enum WallOpening
{
    Top,
    Bottom
}

public sealed class RoundGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject elephantInteractionPointPrefab;
    [SerializeField] private GameObject mouseInteractionPointPrefab;
    [SerializeField] private GameObject birdAttackPrefab;
[SerializeField] private ElephantController elephant;
[SerializeField] private MouseController mouse;

[SerializeField] private ScreenObscurer screenObscurer;
    [SerializeField] private GameObject mazeWallPrefab;
    [SerializeField] private Sprite[] mazeWallSprites;
    [SerializeField] private GameObject earthquakeWallPrefab;
    [SerializeField] private GameObject finishLinePrefab;
    [SerializeField] private GameObject mouseConnectorPrefab;
    [SerializeField] private Sprite[] mouseConnectorSprites;
    [SerializeField] private GameObject mouseMazeBoundaryPrefab;
    [SerializeField] private CameraFollower cameraFollower;
    private readonly List<MouseMazeSegment> mouseMazeSegments = new();
    private readonly List<(float x, int connectorType)> generatedConnectors = new();

    [Header("References")]
    [SerializeField] private RaceManager raceManager;

    [Header("Track")]
    [SerializeField] private float trackLength = 80f;

    [Header("Generation")]
    [SerializeField] private int obstacleCount = 7;
    [SerializeField] private int interactionPointCount = 5;
    [SerializeField] private int minimumInteractionPointCount = 5;
[SerializeField] private float interactionSpawnChance = 0.5f;
[SerializeField] private float minDistanceFromObstacle = 2.5f;

[Header("Interaction Point Placement")]
[SerializeField] private float elephantInteractionHeightOffset;
[SerializeField] private float mouseInteractionHeightOffset;

    [Header("Elephant Obstacle Placement")]
[SerializeField] private float obstacleY = 1.05f;
[SerializeField] private float obstacleStartPadding = 10f;
[SerializeField] private float obstacleEndPadding = 10f;
[SerializeField] private float obstacleSegmentInnerPadding = 1f;

[Header("Mouse Maze Wall Placement")]
[SerializeField] private float mazeWallWidthMultiplier = 0.85f;
[SerializeField] private float mazeWallYOffset = 0.25f;

[Header("Earthquake Sprite Animation")]
[SerializeField] private Sprite[] earthquakeAnimationFrames;
[SerializeField] private float earthquakeFramesPerSecond = 12f;

[Header("Mouse Maze Layout")]
[SerializeField] private float groundBuffer = -0.25f;
[SerializeField] private float laneHeight = -1.3f;
[SerializeField] private float mouseHeightBuffer = -0.5f;

 //Mouse height top + bottom + wall height 
private float mouseTopLaneY;
private float mouseMiddleLaneY;
private float mouseBottomLaneY;
private float topMiddleConnectorY;
private float middleBottomConnectorY;
private float bottomBoundaryY;

    private readonly List<GameObject> spawnedObjects = new();
private readonly List<float> obstacleXs = new();
    public float FinishX => trackLength;

    public void GenerateRound()
{
    ClearRound();
        SetupMouseMazeConfiguration();

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
obstacleXs.Clear();
        mouseMazeSegments.Clear();
    }

    private void GenerateGround()
    {
        Spawn(groundPrefab, new Vector3(trackLength * 0.5f, 0f, 0f));
    }

    private void SetupMouseMazeConfiguration()
    {
        mouseTopLaneY = groundBuffer  + mouseHeightBuffer + laneHeight / 2f;
        mouseMiddleLaneY = mouseTopLaneY + mouseHeightBuffer + laneHeight;
        mouseBottomLaneY = mouseMiddleLaneY  + mouseHeightBuffer + laneHeight;
        bottomBoundaryY = -9f;
        topMiddleConnectorY = mouseTopLaneY +  laneHeight / 2f + mouseHeightBuffer /2f;
        middleBottomConnectorY = mouseMiddleLaneY  +  laneHeight / 2f + mouseHeightBuffer /2f;
    }

    private void GenerateFinishLine()
    {
        GameObject finish = Spawn(finishLinePrefab, new Vector3(trackLength, 0f, 0f));

        FinishLine finishLine = finish.GetComponent<FinishLine>();
        if (finishLine != null)
            finishLine.SetRaceManager(raceManager);
    }

private void GenerateElephantObstacles()
{
    float usableLength = trackLength - obstacleStartPadding - obstacleEndPadding;
    float segmentLength = usableLength / obstacleCount;

    for (int i = 0; i < obstacleCount; i++)
    {
        float segmentStart = obstacleStartPadding + i * segmentLength;
        float segmentEnd = segmentStart + segmentLength;

        float x = Random.Range(
            segmentStart + obstacleSegmentInnerPadding,
            segmentEnd - obstacleSegmentInnerPadding
        );

        obstacleXs.Add(x);

        Spawn(
            obstaclePrefab,
            new Vector3(x, obstacleY, 0f)
        );
    }
}
private bool IsTooCloseToObstacle(float x, float minDistance)
{
    foreach (float obstacleX in obstacleXs)
    {
        if (Mathf.Abs(x - obstacleX) < minDistance)
            return true;
    }

    return false;
}

private void GenerateInteractionPairs()
{
    List<float> candidateXs = new();

    foreach (var connector in generatedConnectors)
    {
        candidateXs.Add(connector.x);
    }

    Shuffle(candidateXs);

    List<float> selectedXs = new();

    // Pass 1: random coin flip
    foreach (float x in candidateXs)
    {
        if (IsTooCloseToObstacle(x, minDistanceFromObstacle))
            continue;

        if (Random.value <= interactionSpawnChance)
            selectedXs.Add(x);
    }

    // Pass 2: guarantee at least 5
    foreach (float x in candidateXs)
    {
        if (selectedXs.Count >= minimumInteractionPointCount)
            break;

        if (selectedXs.Contains(x))
            continue;

        if (IsTooCloseToObstacle(x, minDistanceFromObstacle))
            continue;

        selectedXs.Add(x);
    }

    // Pass 3: if obstacles blocked too many, allow close ones anyway
    foreach (float x in candidateXs)
    {
        if (selectedXs.Count >= minimumInteractionPointCount)
            break;

        if (selectedXs.Contains(x))
            continue;

        selectedXs.Add(x);
    }

    if (selectedXs.Count < minimumInteractionPointCount)
    {
        Debug.LogWarning(
            $"Only generated {selectedXs.Count} interaction points because there were not enough connectors."
        );
    }

    for (int i = 0; i < selectedXs.Count; i++)
    {
        CreateInteractionPair(selectedXs[i], i);
    }
}

private void CreateInteractionPair(float x, int index)
{
    Color[] colors =
    {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    Color color = colors[index % colors.Length];

    GameObject controllerObject = new GameObject($"SharedInteraction_{index}");
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
        elephantInteractionPointPrefab,
        new Vector3(x, 0.85f + elephantInteractionHeightOffset, 0f)
    );

    Vector3 mouseInteractionPosition = new Vector3(
        x,
        groundBuffer + mouseHeightBuffer + mouseInteractionHeightOffset,
        0f
    );

    GameObject mousePointObject = Spawn(
        mouseInteractionPointPrefab,
        mouseInteractionPosition
    );

    InteractionAccessPoint elephantPoint =
        elephantPointObject.GetComponent<InteractionAccessPoint>() ??
        elephantPointObject.AddComponent<InteractionAccessPoint>();

    InteractionAccessPoint mousePoint =
        mousePointObject.GetComponent<InteractionAccessPoint>() ??
        mousePointObject.AddComponent<InteractionAccessPoint>();

    elephantPoint.Initialize(PlayerId.Elephant, controller, color);
    mousePoint.Initialize(PlayerId.Mouse, controller, color);

    controller.Initialize(elephantPoint, mousePoint);
}
private void Shuffle(List<float> list)
{
    for (int i = 0; i < list.Count; i++)
    {
        int randomIndex = Random.Range(i, list.Count);

        float temp = list[i];
        list[i] = list[randomIndex];
        list[randomIndex] = temp;
    }
}
private void GenerateMouseMazeWalls()
{
    const float segmentLength = 7f;

    float[] laneY =
{
    mouseTopLaneY,
    mouseMiddleLaneY,
    mouseBottomLaneY
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
                GameObject connector = Spawn(
                    mouseConnectorPrefab,
                    new Vector3(connectorX, topMiddleConnectorY, 0f)
                );

                ApplyRandomSprite(connector, mouseConnectorSprites);
            }
            else
            {
                GameObject connector = Spawn(
                    mouseConnectorPrefab,
                    new Vector3(connectorX, middleBottomConnectorY, 0f)
                );

                ApplyRandomSprite(connector, mouseConnectorSprites);
            }
        }
    }
}

public void TriggerEarthquake(float mouseX)
{
    if (cameraFollower != null)
{
    cameraFollower.TriggerEarthquakeShake();
}
    float[] laneY =
{
    mouseTopLaneY,
    mouseMiddleLaneY,
    mouseBottomLaneY
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
            new Vector3(centerX, laneY[segment.openLane], 0f)
        );

        blocker.name = $"EarthquakeBlocker_Lane_{segment.openLane}";

        StartCoroutine(PlayEarthquakeAnimation(blocker));

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
    WallOpening opening =
        Random.value > 0.5f
            ? WallOpening.Top
            : WallOpening.Bottom;

float yOffset =
    opening == WallOpening.Top
        ? -mazeWallYOffset
        : mazeWallYOffset;

    GameObject wall = Spawn(
    mazeWallPrefab,
    new Vector3(
        segmentCenterX,
        laneY[lane] + yOffset,
        0f
    )
);

    ApplyRandomMazeWallSprite(wall);

    wall.name =
        $"MazeWall_S{segment}_L{lane}_{opening}";
}

private void ApplyRandomMazeWallSprite(GameObject wall)
{
    ApplyRandomSprite(wall, mazeWallSprites);
}

private void ApplyRandomSprite(GameObject target, Sprite[] sprites)
{
    if (target == null || sprites == null || sprites.Length == 0)
        return;

    SpriteRenderer renderer = target.GetComponentInChildren<SpriteRenderer>();

    if (renderer != null)
        renderer.sprite = sprites[Random.Range(0, sprites.Length)];
}

private IEnumerator PlayEarthquakeAnimation(GameObject blocker)
{
    if (blocker == null ||
        earthquakeAnimationFrames == null ||
        earthquakeAnimationFrames.Length == 0)
        yield break;

    SpriteRenderer renderer = blocker.GetComponentInChildren<SpriteRenderer>();

    if (renderer == null)
        yield break;

    float frameDuration = 1f / Mathf.Max(earthquakeFramesPerSecond, 1f);

    foreach (Sprite frame in earthquakeAnimationFrames)
    {
        if (blocker == null)
            yield break;

        if (frame != null)
            renderer.sprite = frame;

        yield return new WaitForSeconds(frameDuration);
    }
}
private void GenerateMouseMazeBoundaries()
{


    Spawn(
    mouseMazeBoundaryPrefab,
new Vector3(trackLength * 0.5f, bottomBoundaryY, 0f)
);
}

private GameObject Spawn(GameObject prefab, Vector3 position)
{
    GameObject obj =
        Instantiate(
            prefab,
            position,
            Quaternion.identity,
            transform
        );

    spawnedObjects.Add(obj);

    return obj;
}
}
