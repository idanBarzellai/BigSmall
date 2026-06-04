using UnityEngine;

namespace ElephantVsMouse.Gameplay.Interactions
{
    public sealed class StompPoint : MonoBehaviour
    {
        [SerializeField] private InteractionEffect effect;

        public void Activate()
        {
            effect?.Apply();
        }
    }
}