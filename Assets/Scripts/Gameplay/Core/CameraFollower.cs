using ElephantVsMouse.Gameplay.Core;
using ElephantVsMouse.Gameplay.Players;
using UnityEngine;

namespace ElephantVsMouse.Gameplay.Core
{
    public sealed class CameraFollower : MonoBehaviour
    {
        public Transform elephantTransform;
        public Transform mouseTransform;
        public RaceManager raceManager;

        [Header("Follow")]
        public float smoothSpeed = 5f;

        [Header("Loss Detection")]
        public float loseBuffer = 1.5f;

        private Camera cam;
        private ElephantController elephantController;
        private MouseController mouseController;
        private bool hasRegisteredLossForCurrentRound;
        private bool previousMatchActive;

        private void Awake()
        {
            cam = GetComponent<Camera>() ?? Camera.main;
        }

        private void LateUpdate()
        {
            if (cam == null)
                return;

            if (elephantTransform == null || mouseTransform == null)
            {
                TryAutoFindPlayers();
                if (elephantTransform == null || mouseTransform == null)
                    return;
            }

            if (raceManager == null)
            {
                raceManager = FindAnyObjectByType<RaceManager>();
            }

            if (raceManager != null)
            {
                if (raceManager.IsMatchActive && !previousMatchActive)
                {
                    hasRegisteredLossForCurrentRound = false;
                }

                previousMatchActive = raceManager.IsMatchActive;
            }

            Transform lead = (elephantTransform.position.x >= mouseTransform.position.x) ? elephantTransform : mouseTransform;
            Transform trail = (lead == elephantTransform) ? mouseTransform : elephantTransform;

            // follow lead's X position, keep camera Y centered (0)
            Vector3 current = cam.transform.position;
            Vector3 target = new Vector3(lead.position.x, 0f, current.z);
            float t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
            float newX = Mathf.Lerp(current.x, target.x, t);
            cam.transform.position = new Vector3(newX, target.y, target.z);

            // loss detection: if trailing player leaves the horizontal camera bounds minus buffer -> they lose
            float halfWidth = cam.orthographicSize * cam.aspect;
            float left = cam.transform.position.x - halfWidth;
            if (!hasRegisteredLossForCurrentRound && trail.position.x < left - loseBuffer && raceManager != null && raceManager.IsMatchActive)
            {
                // trailing player lost; register winner as the other
                PlayerId winner = (trail == elephantTransform) ? PlayerId.Mouse : PlayerId.Elephant;
                raceManager.RegisterRoundWinner(winner);
                hasRegisteredLossForCurrentRound = true;

                // disable input on both players to avoid further movement
                if (elephantController == null)
                {
                    elephantController = elephantTransform.GetComponent<ElephantController>();
                }

                if (mouseController == null)
                {
                    mouseController = mouseTransform.GetComponent<MouseController>();
                }

                if (elephantController != null) elephantController.SetInputEnabled(false);
                if (mouseController != null) mouseController.SetInputEnabled(false);
            }
        }

        private void TryAutoFindPlayers()
        {
            if (elephantTransform == null)
            {
                var e = GameObject.Find("Elephant");
                if (e != null) elephantTransform = e.transform;
            }

            if (mouseTransform == null)
            {
                var m = GameObject.Find("Mouse");
                if (m != null) mouseTransform = m.transform;
            }
        }
    }
}
