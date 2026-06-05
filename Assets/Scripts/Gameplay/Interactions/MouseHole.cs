using UnityEngine;

namespace ElephantVsMouse.Gameplay.Interactions
{
    public sealed class MouseHole : MonoBehaviour
    {
        [SerializeField] private InteractionEffect effect;
        [SerializeField] private float cooldown = 2f;
        private float lastActivated = -100f;

        public void Activate()
        {
            effect?.Apply();
        }

        public void SetEffect(InteractionEffect e)
        {
            effect = e;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null) return;

            var mouse = other.GetComponent<ElephantVsMouse.Gameplay.Players.MouseController>();
            if (mouse == null) return;

            if (Time.time - lastActivated < cooldown) return;
            lastActivated = Time.time;
            Activate();
        }
    }
}