using UnityEngine;

public sealed class EggDropActor : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 8f;
    [SerializeField] private float targetY = 0.8f;

    private ScreenObscurer screenObscurer;
    private float obscureDuration;

    public void Initialize(ScreenObscurer obscurer, float duration)
    {
        screenObscurer = obscurer;
        obscureDuration = duration;
    }

    private void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y <= targetY)
        {
            AudioManager.Play(GameSound.EggHit);

            if (screenObscurer != null)
                screenObscurer.Obscure(obscureDuration);

            Destroy(gameObject);
        }
    }
}
