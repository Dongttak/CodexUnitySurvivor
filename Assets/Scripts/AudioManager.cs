using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField, Range(0f, 1f)] private float masterVolume = 0.35f;
    [SerializeField, Range(0f, 1f)] private float shootVolume = 0.32f;
    [SerializeField, Range(0f, 1f)] private float hitVolume = 0.22f;
    [SerializeField, Range(0f, 1f)] private float deathVolume = 0.34f;
    [SerializeField, Range(0f, 1f)] private float xpVolume = 0.25f;
    [SerializeField, Range(0f, 1f)] private float levelUpVolume = 0.42f;
    [SerializeField, Range(0f, 1f)] private float playerDamageVolume = 0.36f;
    [SerializeField, Range(0f, 1f)] private float gameOverVolume = 0.45f;
    [SerializeField] private float hitSoundCooldown = 0.04f;

    private AudioSource audioSource;
    private AudioClip shootClip;
    private AudioClip hitClip;
    private AudioClip deathClip;
    private AudioClip xpClip;
    private AudioClip levelUpClip;
    private AudioClip playerDamageClip;
    private AudioClip gameOverClip;
    private float nextHitSoundTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        BuildClips();
    }

    public void PlayShoot()
    {
        Play(shootClip, shootVolume, 1f);
    }

    public void PlayEnemyHit()
    {
        if (Time.unscaledTime < nextHitSoundTime)
        {
            return;
        }

        nextHitSoundTime = Time.unscaledTime + hitSoundCooldown;
        Play(hitClip, hitVolume, Random.Range(0.94f, 1.06f));
    }

    public void PlayEnemyDeath()
    {
        Play(deathClip, deathVolume, Random.Range(0.96f, 1.04f));
    }

    public void PlayXpPickup()
    {
        Play(xpClip, xpVolume, Random.Range(0.96f, 1.08f));
    }

    public void PlayLevelUp()
    {
        Play(levelUpClip, levelUpVolume, 1f);
    }

    public void PlayPlayerDamage()
    {
        Play(playerDamageClip, playerDamageVolume, 1f);
    }

    public void PlayGameOver()
    {
        Play(gameOverClip, gameOverVolume, 1f);
    }

    private void BuildClips()
    {
        shootClip = CreateTone("SFX Shoot", 720f, 930f, 0.055f, WaveType.Square);
        hitClip = CreateTone("SFX Enemy Hit", 260f, 180f, 0.06f, WaveType.Noise);
        deathClip = CreateTone("SFX Enemy Death", 180f, 70f, 0.16f, WaveType.Saw);
        xpClip = CreateTone("SFX XP Pickup", 920f, 1320f, 0.07f, WaveType.Sine);
        levelUpClip = CreateArpeggio("SFX Level Up", new[] { 520f, 660f, 880f }, 0.24f);
        playerDamageClip = CreateTone("SFX Player Damage", 150f, 90f, 0.14f, WaveType.Square);
        gameOverClip = CreateTone("SFX Game Over", 220f, 55f, 0.36f, WaveType.Sine);
    }

    private void Play(AudioClip clip, float volume, float pitch)
    {
        if (clip == null || audioSource == null || masterVolume <= 0f || volume <= 0f)
        {
            return;
        }

        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, masterVolume * volume);
    }

    private static AudioClip CreateTone(string clipName, float startFrequency, float endFrequency, float duration, WaveType waveType)
    {
        const int sampleRate = 44100;
        int sampleCount = Mathf.Max(1, Mathf.CeilToInt(sampleRate * duration));
        float[] samples = new float[sampleCount];
        float phase = 0f;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleCount;
            float frequency = Mathf.Lerp(startFrequency, endFrequency, t);
            phase += frequency / sampleRate;
            phase -= Mathf.Floor(phase);
            float envelope = Mathf.Sin(Mathf.PI * t) * (1f - t * 0.25f);
            samples[i] = SampleWave(phase, waveType) * envelope * 0.6f;
        }

        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static AudioClip CreateArpeggio(string clipName, float[] frequencies, float duration)
    {
        const int sampleRate = 44100;
        int sampleCount = Mathf.Max(1, Mathf.CeilToInt(sampleRate * duration));
        float[] samples = new float[sampleCount];
        int segmentLength = Mathf.Max(1, sampleCount / frequencies.Length);
        float phase = 0f;

        for (int i = 0; i < sampleCount; i++)
        {
            int segment = Mathf.Min(frequencies.Length - 1, i / segmentLength);
            float t = i / (float)sampleCount;
            phase += frequencies[segment] / sampleRate;
            phase -= Mathf.Floor(phase);
            float envelope = Mathf.Sin(Mathf.PI * t);
            samples[i] = Mathf.Sin(phase * Mathf.PI * 2f) * envelope * 0.55f;
        }

        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static float SampleWave(float phase, WaveType waveType)
    {
        switch (waveType)
        {
            case WaveType.Sine:
                return Mathf.Sin(phase * Mathf.PI * 2f);
            case WaveType.Square:
                return phase < 0.5f ? 1f : -1f;
            case WaveType.Saw:
                return phase * 2f - 1f;
            case WaveType.Noise:
                return Random.Range(-1f, 1f);
            default:
                return 0f;
        }
    }

    private enum WaveType
    {
        Sine,
        Square,
        Saw,
        Noise
    }
}
