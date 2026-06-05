using UnityEngine;

    public sealed class MazeWallController : MonoBehaviour
    {
        private static readonly System.Collections.Generic.List<MazeWallController> RegisteredWalls = new System.Collections.Generic.List<MazeWallController>();

        private SpriteRenderer sr;
        private Collider2D col;
        private bool isOpen = false;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            if (!RegisteredWalls.Contains(this))
            {
                RegisteredWalls.Add(this);
            }
        }

        private void OnDestroy()
        {
            RegisteredWalls.Remove(this);
        }

        public static MazeWallController[] GetRegisteredWalls()
        {
            return RegisteredWalls.ToArray();
        }

        public void ToggleOpen()
        {
            isOpen = !isOpen;
            if (sr != null)
            {
                Color c = sr.color;
                c.a = isOpen ? 0.25f : 1f;
                sr.color = c;
            }

            if (col != null)
            {
                col.enabled = !isOpen;
            }
        }
    }
