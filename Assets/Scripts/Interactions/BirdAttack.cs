using System.Collections;
using UnityEngine;


    public sealed class BirdAttackActor : MonoBehaviour
    {
        public Transform targetElephant;
        public float speed = 8f;
        public float swoopHeight = 6f;
        public float eggDropOffsetY = 0.5f;
        public float coverDuration = 3f;

        private SpriteRenderer spriteRenderer;
        private static Sprite cachedSprite;
        private static Sprite cachedEggSprite;

        private void Start()
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            if (cachedSprite == null)
            {
                cachedSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            }
            spriteRenderer.sprite = cachedSprite;
            spriteRenderer.color = Color.white;
            transform.localScale = new Vector3(0.4f, 0.4f, 1f);

            StartCoroutine(SwoopRoutine());
        }

        private IEnumerator SwoopRoutine()
        {
            // Determine target positions
            Vector3 targetPos = targetElephant != null ? targetElephant.position : Vector3.zero;
            float startX = (targetPos.x) - 10f;
            float endX = (targetPos.x) + 10f;
            float y = (targetPos.y) + swoopHeight;

            transform.position = new Vector3(startX, y, 0f);

            bool eggDropped = false;

            while (Mathf.Abs(transform.position.x - endX) > 0.1f)
            {
                float step = speed * Time.deltaTime;
                Vector3 next = Vector3.MoveTowards(transform.position, new Vector3(endX, y, 0f), step);
                transform.position = next;

                if (!eggDropped && targetElephant != null && Mathf.Abs(transform.position.x - targetElephant.position.x) < 1.2f)
                {
                    SpawnEgg();
                    eggDropped = true;
                }

                yield return null;
            }

            Destroy(gameObject);
        }

        private void SpawnEgg()
        {
            GameObject egg = new GameObject("Egg");
            egg.transform.SetParent(transform.parent, false);
            egg.transform.position = new Vector3(transform.position.x, transform.position.y - eggDropOffsetY, 0f);
            egg.transform.localScale = Vector3.one * 0.25f;

            SpriteRenderer sr = egg.AddComponent<SpriteRenderer>();
            if (cachedEggSprite == null)
            {
                cachedEggSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            }
            sr.sprite = cachedEggSprite;
            sr.color = Color.white;

            Rigidbody2D rb = egg.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3f;

            CircleCollider2D cc = egg.AddComponent<CircleCollider2D>();
            cc.radius = 0.15f;
            cc.isTrigger = false;

            // egg.AddComponent<EggBehaviour>().coverDuration = coverDuration;
        }
    }
