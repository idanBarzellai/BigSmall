using UnityEngine;

namespace ElephantVsMouse.Gameplay.Interactions
{
    public sealed class StompPoint : MonoBehaviour
    {
        [SerializeField] private InteractionEffect effect;
        [SerializeField] private float cooldown = 1.5f;
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

            // Only react to the Elephant
            var elephant = other.GetComponent<ElephantVsMouse.Gameplay.Players.ElephantController>();
            if (elephant == null) return;

            Rigidbody2D rb = other.attachedRigidbody;
            if (rb == null) return;

            // Activate only when the elephant is landing from above (i.e. falling onto the trigger)
            if (rb.linearVelocity.y <= 0f && other.transform.position.y > transform.position.y)
            {
                if (Time.time - lastActivated < cooldown) return;
                lastActivated = Time.time;
                Activate();
            }
        }
    }
}