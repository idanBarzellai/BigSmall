using UnityEngine;

namespace ElephantVsMouse.Gameplay.Interactions
{
    public sealed class MouseHole : MonoBehaviour
    {
        [SerializeField] private InteractionEffect effect;

        public void Activate()
        {
            effect?.Apply();
        }
    }
}