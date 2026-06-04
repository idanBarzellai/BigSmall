using UnityEngine;

namespace ElephantVsMouse.Gameplay.Interactions
{
    public abstract class InteractionEffect : ScriptableObject
    {
        public abstract void Apply();
    }
}