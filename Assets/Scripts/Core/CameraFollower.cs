using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class CameraFollower : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform elephant;
    [SerializeField] private Transform mouse;
    [SerializeField] private RaceManager raceManager;

    [Header("Follow")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float verticalCenter = 0f;

    [Header("Lose Detection")]
    [SerializeField] private float loseBuffer = 1.5f;

    private Camera cam;
    private bool hasRegisteredOffscreenLoss;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (elephant == null || mouse == null || raceManager == null)
            return;

        FollowLeader();
        CheckOffscreenLoss();
    }

    private void FollowLeader()
    {
        Transform leader = elephant.position.x >= mouse.position.x ? elephant : mouse;

        Vector3 current = transform.position;
        Vector3 target = new Vector3(leader.position.x, verticalCenter, current.z);

        float t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(current, target, t);
    }

    private void CheckOffscreenLoss()
    {
        if (!raceManager.IsRoundActive || hasRegisteredOffscreenLoss)
            return;

        Transform leader = elephant.position.x >= mouse.position.x ? elephant : mouse;
        Transform trailer = leader == elephant ? mouse : elephant;

        float halfWidth = cam.orthographicSize * cam.aspect;
        float leftEdge = transform.position.x - halfWidth;

        if (trailer.position.x < leftEdge - loseBuffer)
        {
            hasRegisteredOffscreenLoss = true;

            PlayerId winner = trailer == elephant
                ? PlayerId.Mouse
                : PlayerId.Elephant;

            raceManager.RegisterWinner(winner);
        }
    }

    public void ResetForNewRound()
    {
        hasRegisteredOffscreenLoss = false;

        if (elephant != null && mouse != null)
        {
            float startX = Mathf.Max(elephant.position.x, mouse.position.x);
            transform.position = new Vector3(startX, verticalCenter, transform.position.z);
        }
    }
}