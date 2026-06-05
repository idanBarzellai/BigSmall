using UnityEngine;

namespace ElephantVsMouse.Gameplay.Interactions
{
    public sealed class EggBehaviour : MonoBehaviour
    {
        public float coverDuration = 3f;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider == null) return;

            var elephant = collision.collider.GetComponent<ElephantVsMouse.Gameplay.Players.ElephantController>();
            if (elephant != null)
            {
                // Find screen obscurer and trigger cover
                var obscurer = FindAnyObjectByType<ScreenObscurer>();
                if (obscurer != null)
                {
                    obscurer.Obscure(coverDuration);
                }

                Destroy(gameObject);
            }
        }
    }
}
