using UnityEngine;

namespace ElephantVsMouse.Gameplay.Interactions
{
    [CreateAssetMenu(menuName = "Elephant vs Mouse/Interaction Effects/Bird Attack")]
    public sealed class BirdAttackEffect : InteractionEffect
    {
        public override void Apply()
        {
            var bootstrap = FindAnyObjectByType<ElephantVsMouse.Gameplay.Core.PrototypeBootstrapper>();
            Transform parent = bootstrap != null ? bootstrap.transform : null;

            GameObject birdObject = new GameObject("BirdAttack");
            if (parent != null)
            {
                birdObject.transform.SetParent(parent, false);
            }

            var actor = birdObject.AddComponent<BirdAttackActor>();

            if (bootstrap != null)
            {
                var elephant = bootstrap.transform.Find("Elephant");
                if (elephant != null)
                {
                    actor.targetElephant = elephant;
                }
            }
        }
    }
}
