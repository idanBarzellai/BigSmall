using System.Collections;
using UnityEngine;

public enum GameSound
{
    GameStart,
    MouseReady,
    ElephantReady,
    Countdown,
    ElephantOuch,
    ElephantJump,
    Earthquake,
    MouseBirdCall,
    BirdFlying,
    EggHit,
    FinishLine,
    ElephantWin,
    MouseWin,
    MatchWin
}

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.6f;

    [Header("Start, Ready and Countdown")]
    [SerializeField] private AudioClip gameStartSfx;
    [SerializeField, Range(0f, 1f)] private float gameStartVolume = 1f;
    [SerializeField, Min(0f)] private float gameStartFadeDuration = 1f;
    [SerializeField] private AudioClip mouseReadySfx;
    [SerializeField] private AudioClip elephantReadySfx;
    [SerializeField] private AudioClip countdownSfx;

    [Header("Elephant")]
    [SerializeField] private AudioClip elephantOuchSfx;
    [SerializeField] private AudioClip elephantJumpSfx;
    [SerializeField] private AudioClip earthquakeSfx;

    [Header("Bird Attack")]
    [SerializeField] private AudioClip mouseBirdCallSfx;
    [SerializeField] private AudioClip birdFlyingSfx;
    [SerializeField] private AudioClip eggHitSfx;

    [Header("Race")]
    [SerializeField] private AudioClip finishLineSfx;
    [SerializeField] private AudioClip elephantWinSfx;
    [SerializeField] private AudioClip mouseWinSfx;
    [SerializeField] private AudioClip matchWinSfx;

    [Header("SFX Settings")]
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource gameStartSource;
    private AudioSource sfxSource;
    private bool gameStartPlayed;
    private Coroutine gameStartFadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;

        gameStartSource = gameObject.AddComponent<AudioSource>();
        gameStartSource.playOnAwake = false;
        gameStartSource.loop = false;
        gameStartSource.spatialBlend = 0f;
        gameStartSource.volume = gameStartVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;
        sfxSource.volume = sfxVolume;
    }

    public static void StartBackgroundMusic()
    {
        if (Instance == null || Instance.backgroundMusic == null)
            return;

        Instance.musicSource.Stop();
        Instance.musicSource.clip = Instance.backgroundMusic;
        Instance.musicSource.time = 0f;
        Instance.musicSource.Play();
    }

    public static void StopBackgroundMusic()
    {
        if (Instance == null)
            return;

        Instance.musicSource.Stop();
        Instance.musicSource.time = 0f;
    }

    public static void ResetForNewMatch()
    {
        if (Instance == null)
            return;

        Instance.sfxSource.Stop();

        PrepareForReadyPhase();
    }

    public static void PrepareForReadyPhase()
    {
        if (Instance == null)
            return;

        StopBackgroundMusic();

        if (Instance.gameStartFadeRoutine != null)
        {
            Instance.StopCoroutine(Instance.gameStartFadeRoutine);
            Instance.gameStartFadeRoutine = null;
        }

        Instance.gameStartSource.Stop();
        Instance.gameStartSource.volume = Instance.gameStartVolume;
        Instance.gameStartPlayed = false;
    }

    public static float Play(GameSound sound)
    {
        if (Instance == null)
            return 0f;

        if (sound == GameSound.GameStart)
            return Instance.PlayGameStartOnce();

        AudioClip clip = Instance.GetClip(sound);

        if (clip == null)
            return 0f;

        Instance.sfxSource.PlayOneShot(clip);
        return clip.length;
    }

    private float PlayGameStartOnce()
    {
        if (gameStartPlayed || gameStartSfx == null)
            return 0f;

        gameStartPlayed = true;

        gameStartSource.clip = gameStartSfx;
        gameStartSource.volume = gameStartVolume;
        gameStartSource.Play();

        return 0f;
    }

    public static void FadeOutGameStart()
    {
        if (Instance == null || !Instance.gameStartSource.isPlaying)
            return;

        if (Instance.gameStartFadeRoutine != null)
            Instance.StopCoroutine(Instance.gameStartFadeRoutine);

        Instance.gameStartFadeRoutine =
            Instance.StartCoroutine(Instance.FadeOutGameStartRoutine());
    }

    private IEnumerator FadeOutGameStartRoutine()
    {
        float startVolume = gameStartSource.volume;
        float elapsed = 0f;

        if (gameStartFadeDuration > 0f)
        {
            while (elapsed < gameStartFadeDuration)
            {
                elapsed += Time.deltaTime;
                gameStartSource.volume = Mathf.Lerp(
                    startVolume,
                    0f,
                    elapsed / gameStartFadeDuration
                );
                yield return null;
            }
        }

        gameStartSource.Stop();
        gameStartSource.volume = gameStartVolume;
        gameStartFadeRoutine = null;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private AudioClip GetClip(GameSound sound)
    {
        switch (sound)
        {
            case GameSound.GameStart: return gameStartSfx;
            case GameSound.MouseReady: return mouseReadySfx;
            case GameSound.ElephantReady: return elephantReadySfx;
            case GameSound.Countdown: return countdownSfx;
            case GameSound.ElephantOuch: return elephantOuchSfx;
            case GameSound.ElephantJump: return elephantJumpSfx;
            case GameSound.Earthquake: return earthquakeSfx;
            case GameSound.MouseBirdCall: return mouseBirdCallSfx;
            case GameSound.BirdFlying: return birdFlyingSfx;
            case GameSound.EggHit: return eggHitSfx;
            case GameSound.FinishLine: return finishLineSfx;
            case GameSound.ElephantWin: return elephantWinSfx;
            case GameSound.MouseWin: return mouseWinSfx;
            case GameSound.MatchWin: return matchWinSfx;
            default: return null;
        }
    }
}
