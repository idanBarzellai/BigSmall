using UnityEngine;

namespace ElephantVsMouse.Gameplay.Interactions
{
    [CreateAssetMenu(menuName = "Elephant vs Mouse/Interaction Effects/Maze Change")]
    public sealed class MazeChangeEffect : InteractionEffect
    {
        public override void Apply()
        {
            var bootstrap = FindAnyObjectByType<ElephantVsMouse.Gameplay.Core.PrototypeBootstrapper>();
            if (bootstrap == null) return;
            // Find nearest MazeWallController and toggle its open state (non-destructive)
            var controllers = MazeWallController.GetRegisteredWalls();
            if (controllers == null || controllers.Length == 0) return;

            // Use the mouse position as reference if available
            var mouseTf = bootstrap.transform.Find("Mouse");
            float refX = mouseTf != null ? mouseTf.position.x : 0f;

            MazeWallController best = null;
            float bestDist = float.MaxValue;
            foreach (var c in controllers)
            {
                float d = Mathf.Abs(c.transform.position.x - refX);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = c;
                }
            }

            if (best != null)
            {
                best.ToggleOpen();
            }
        }
    }
}
