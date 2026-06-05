using ElephantVsMouse.Gameplay.Input;
using ElephantVsMouse.Gameplay.Players;
using UnityEngine;

namespace ElephantVsMouse.Gameplay.Core
{
    [DefaultExecutionOrder(-1000)]
    public sealed class PrototypeBootstrapper : MonoBehaviour
    {
        private static Sprite blockSprite;
        private static GameObject runtimeUiRoot;

        private Camera mainCamera;
        private RaceManager raceManager;
        private SharedKeyboardInputRouter inputRouter;
        private ElephantController elephantController;
        private MouseController mouseController;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateBootstrapper()
        {
            if (FindAnyObjectByType<PrototypeBootstrapper>() != null)
            {
                return;
            }

            GameObject bootstrapObject = new GameObject("PrototypeBootstrapper");
            DontDestroyOnLoad(bootstrapObject);
            bootstrapObject.AddComponent<PrototypeBootstrapper>();
        }

        private void Awake()
        {
            EnsureRuntimeUiRoot();
            BuildWorld();

            WinScreenController winScreen = GetComponent<WinScreenController>();
            if (winScreen == null)
            {
                winScreen = gameObject.AddComponent<WinScreenController>();
            }

            winScreen.Bind(this, raceManager);

            // Ensure a screen obscurer exists for interaction effects (bird egg)
            if (FindAnyObjectByType<ElephantVsMouse.Gameplay.Interactions.ScreenObscurer>() == null)
            {
                gameObject.AddComponent<ElephantVsMouse.Gameplay.Interactions.ScreenObscurer>();
            }
        }

        public RaceManager RestartWorld()
        {
            BuildWorld();
            return raceManager;
        }

        public RaceManager GetRaceManager()
        {
            return raceManager;
        }

        private void BuildWorld()
        {
            ClearRuntimeChildren();

            mainCamera = ConfigureCamera();
            CreateGround();
            CreateMouseMaze();

            inputRouter = CreateInputRouter();
            raceManager = CreateRaceManager();

            mouseController = CreateMouse();
            elephantController = CreateElephant();

            SetupCameraFollower();

            raceManager.ResetMatch();
            raceManager.BeginMatch();

            elephantController.SetInputEnabled(true);
            mouseController.SetInputEnabled(true);
        }

        private void ClearRuntimeChildren()
        {
            for (int index = transform.childCount - 1; index >= 0; index--)
            {
                Transform child = transform.GetChild(index);
                if (runtimeUiRoot != null && child == runtimeUiRoot.transform)
                {
                    continue;
                }

                Destroy(child.gameObject);
            }
        }

        private void EnsureRuntimeUiRoot()
        {
            if (runtimeUiRoot != null)
            {
                return;
            }

            runtimeUiRoot = new GameObject("Runtime UI Root");
            DontDestroyOnLoad(runtimeUiRoot);
        }

        private Camera ConfigureCamera()
        {
            // Find all cameras in the scene and ensure a single active main camera
            Camera[] cams = Resources.FindObjectsOfTypeAll<Camera>();
            Camera mainCamera = null;

            foreach (var c in cams)
            {
                if (c.CompareTag("MainCamera"))
                {
                    mainCamera = c;
                    break;
                }
            }

            if (mainCamera == null && cams.Length > 0)
            {
                mainCamera = cams[0];
            }

            if (mainCamera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                cameraObject.AddComponent<AudioListener>();
                mainCamera = cameraObject.AddComponent<Camera>();
            }

            // Disable other cameras to avoid split-screen
            foreach (var c in cams)
            {
                if (c != mainCamera)
                {
                    c.enabled = false;
                    // also disable any CameraFollower on non-main cameras to avoid duplicate winner registration
                    var cf = c.gameObject.GetComponent<CameraFollower>();
                    if (cf != null)
                    {
                        cf.enabled = false;
                    }
                }
            }

            mainCamera.tag = "MainCamera";
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 6f;
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
            mainCamera.rect = new Rect(0f, 0f, 1f, 1f);

            return mainCamera;
        }

        private void CreateGround()
        {
            GameObject groundObject = new GameObject("Ground");
            groundObject.transform.SetParent(transform, false);
            groundObject.name = "Ground";
            // place ground in the middle so elephant above and mouse below
            groundObject.transform.position = new Vector3(0f, 0f, 0f);
            groundObject.transform.localScale = new Vector3(60f, 1f, 1f);

            SpriteRenderer spriteRenderer = groundObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetBlockSprite();
            spriteRenderer.color = new Color(0.15f, 0.2f, 0.26f, 1f);

            BoxCollider2D collider2D = groundObject.AddComponent<BoxCollider2D>();
            collider2D.size = new Vector2(60f, 1f);

            // Add button-like triggers at specific positions along the platform
            float[] triggerXs = new float[] { -20f, -6f, 6f, 20f };
            var mazeEffect = ScriptableObject.CreateInstance<ElephantVsMouse.Gameplay.Interactions.MazeChangeEffect>();
            var birdEffect = ScriptableObject.CreateInstance<ElephantVsMouse.Gameplay.Interactions.BirdAttackEffect>();

            for (int i = 0; i < triggerXs.Length; i++)
            {
                float x = triggerXs[i];

                // Elephant stomp trigger (on top of ground)
                GameObject stompTrigger = new GameObject($"Elephant Stomp Trigger {i}");
                stompTrigger.transform.SetParent(groundObject.transform, false);
                stompTrigger.transform.localPosition = new Vector3(x, 0.6f, 0f);
                var stompSprite = stompTrigger.AddComponent<SpriteRenderer>();
                stompSprite.sprite = GetBlockSprite();
                stompSprite.color = new Color(1f, 0.2f, 0.2f, 0.9f);
                stompSprite.sortingOrder = 100;
                var stompCollider = stompTrigger.AddComponent<BoxCollider2D>();
                stompCollider.size = new Vector2(2f, 0.2f);
                stompCollider.isTrigger = true;
                var stompPoint = stompTrigger.AddComponent<ElephantVsMouse.Gameplay.Interactions.StompPoint>();
                stompPoint.SetEffect(mazeEffect);

                // Mouse hole trigger (under the ground)
                GameObject mouseTrigger = new GameObject($"Mouse Hole Trigger {i}");
                mouseTrigger.transform.SetParent(groundObject.transform, false);
                mouseTrigger.transform.localPosition = new Vector3(x, -0.6f, 0f);
                var mouseSprite = mouseTrigger.AddComponent<SpriteRenderer>();
                mouseSprite.sprite = GetBlockSprite();
                mouseSprite.color = new Color(1f, 0.2f, 0.2f, 0.9f);
                mouseSprite.sortingOrder = 100;
                var mouseCollider = mouseTrigger.AddComponent<BoxCollider2D>();
                mouseCollider.size = new Vector2(2f, 0.2f);
                mouseCollider.isTrigger = true;
                var mouseHole = mouseTrigger.AddComponent<ElephantVsMouse.Gameplay.Interactions.MouseHole>();
                mouseHole.SetEffect(birdEffect);
            }
        }

        private void CreateMouseMaze()
        {
            CreateMazeWall("Mouse Maze Wall 1", new Vector3(-24f, -0.9f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 2", new Vector3(-21f, -1.5f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 3", new Vector3(-18f, -0.9f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 4", new Vector3(-15f, -1.9f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 5", new Vector3(-12f, -2.5f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 6", new Vector3(-9f, -1.8f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 7", new Vector3(-6f, -2.9f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 8", new Vector3(-3f, -2.1f, 0f), new Vector3(0.4f, 0.4f, 1f));

            CreateMazeWall("Mouse Maze Wall 9", new Vector3(0f, -1.3f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 10", new Vector3(3f, -2.1f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 11", new Vector3(6f, -1.2f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 12", new Vector3(9f, -2.5f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 13", new Vector3(12f, -1.7f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 14", new Vector3(15f, -2.8f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 15", new Vector3(18f, -2f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 16", new Vector3(21f, -1.4f, 0f), new Vector3(0.4f, 0.4f, 1f));
            CreateMazeWall("Mouse Maze Wall 17", new Vector3(24f, -2.5f, 0f), new Vector3(0.4f, 0.4f, 1f));

            CreateMazePathMarker(new Vector3(-22.5f, -1.9f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(-19.5f, -2.2f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(-16.2f, -2.5f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(-13f, -1.9f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(-9.8f, -2.3f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(-6.4f, -2f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(-2.8f, -2.3f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(0.8f, -1.8f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(4.2f, -2.4f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(7.8f, -1.8f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(11.2f, -2.4f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(14.8f, -2f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(18.2f, -2.2f, 0f), new Vector3(0.25f, 0.12f, 1f));
            CreateMazePathMarker(new Vector3(21.8f, -1.8f, 0f), new Vector3(0.25f, 0.12f, 1f));
        }

        private void CreateTrackSegment(string objectName, Vector3 position, Vector3 scale, Color color)
        {
            GameObject segment = new GameObject(objectName);
            segment.transform.SetParent(transform, false);
            segment.transform.position = position;
            segment.transform.localScale = scale;

            SpriteRenderer spriteRenderer = segment.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetBlockSprite();
            spriteRenderer.color = color;

            BoxCollider2D collider2D = segment.AddComponent<BoxCollider2D>();
            collider2D.size = Vector2.one;
        }

        private void CreateMazeWall(string objectName, Vector3 position, Vector3 scale)
        {
            GameObject wall = new GameObject(objectName);
            wall.transform.SetParent(transform, false);
            wall.transform.position = position;
            wall.transform.localScale = scale;

            SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetBlockSprite();
            spriteRenderer.color = new Color(0.12f, 0.14f, 0.18f, 1f);

            BoxCollider2D collider2D = wall.AddComponent<BoxCollider2D>();
            collider2D.size = Vector2.one;
            // add a controller so MazeChangeEffect can toggle walls non-destructively
            wall.AddComponent<ElephantVsMouse.Gameplay.Interactions.MazeWallController>();
        }

        private void CreateMazePathMarker(Vector3 position, Vector3 scale)
        {
            GameObject marker = new GameObject("Mouse Maze Path");
            marker.transform.SetParent(transform, false);
            marker.transform.position = position;
            marker.transform.localScale = scale;

            SpriteRenderer spriteRenderer = marker.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetBlockSprite();
            spriteRenderer.color = new Color(0.22f, 0.26f, 0.30f, 1f);
        }

        private SharedKeyboardInputRouter CreateInputRouter()
        {
            SharedKeyboardInputRouter existingRouter = FindAnyObjectByType<SharedKeyboardInputRouter>();
            if (existingRouter != null)
            {
                return existingRouter;
            }

            GameObject routerObject = new GameObject("Input Router");
            routerObject.transform.SetParent(transform, false);
            DontDestroyOnLoad(routerObject);
            return routerObject.AddComponent<SharedKeyboardInputRouter>();
        }

        private RaceManager CreateRaceManager()
        {
            RaceManager existingManager = FindAnyObjectByType<RaceManager>();
            if (existingManager != null)
            {
                return existingManager;
            }

            GameObject managerObject = new GameObject("Race Manager");
            return managerObject.AddComponent<RaceManager>();
        }

        private ElephantController CreateElephant()
        {
            GameObject elephantObject = new GameObject("Elephant");
            elephantObject.transform.SetParent(transform, false);
            elephantObject.name = "Elephant";
            elephantObject.transform.position = new Vector3(-3f, 2f, 0f);
            // elephant is triple the mouse size
            elephantObject.transform.localScale = Vector3.one * 3f;

            SpriteRenderer spriteRenderer = elephantObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetBlockSprite();
            spriteRenderer.color = new Color(0.85f, 0.58f, 0.18f, 1f);

            Rigidbody2D body = elephantObject.AddComponent<Rigidbody2D>();
            body.freezeRotation = true;
            body.gravityScale = 3f;

            elephantObject.AddComponent<BoxCollider2D>();

            ElephantController controller = elephantObject.AddComponent<ElephantController>();
            return controller;
        }

        private MouseController CreateMouse()
        {
            GameObject mouseObject = new GameObject("Mouse");
            mouseObject.transform.SetParent(transform, false);
            mouseObject.name = "Mouse";
            mouseObject.transform.position = new Vector3(3f, -2f, 0f);
            mouseObject.transform.localScale = Vector3.one;

            SpriteRenderer spriteRenderer = mouseObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetBlockSprite();
            spriteRenderer.color = new Color(0.82f, 0.82f, 0.82f, 1f);

            Rigidbody2D body = mouseObject.AddComponent<Rigidbody2D>();
            body.freezeRotation = true;
            body.gravityScale = 0f;

            mouseObject.AddComponent<BoxCollider2D>();

            MouseController controller = mouseObject.AddComponent<MouseController>();
            return controller;
        }

        private static Sprite GetBlockSprite()
        {
            if (blockSprite != null)
            {
                return blockSprite;
            }

            Texture2D texture = Texture2D.whiteTexture;
            Rect textureRect = new Rect(0f, 0f, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            blockSprite = Sprite.Create(texture, textureRect, pivot, texture.width);
            return blockSprite;
        }

        private void SetupCameraFollower()
        {
            if (mainCamera == null)
            {
                return;
            }

            CameraFollower follower = mainCamera.GetComponent<CameraFollower>();
            if (follower == null)
            {
                follower = mainCamera.gameObject.AddComponent<CameraFollower>();
            }

            follower.elephantTransform = elephantController.transform;
            follower.mouseTransform = mouseController.transform;
            follower.raceManager = raceManager;
            follower.smoothSpeed = 5f;
            follower.loseBuffer = 1.5f;
        }
    }
}