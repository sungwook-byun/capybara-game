using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("BGM, Effect AudioClips")]
    public AudioClip underwaterBgmClip;  // 일반 BGM
    public AudioClip feverBgmClip;       // 피버타임 BGM
    public AudioClip eatEffectClip;      // 먹기 효과음
    public AudioClip deathEffectClip;    // 죽음 효과음

    [Header("Sound Settings")]
    [Tooltip("먹는 사운드 효과 켜기 끄기")]
    public bool enableEatEffect = true;

    [Tooltip("죽는 사운드 효과 켜기 끄기")]
    public bool enableDeathEffect = true;

    private AudioSource bgmSource;       // BGM 전용 AudioSource
    private AudioSource effectSource;    // 효과음 전용 AudioSource

    private bool isPlayingDeathEffect = false;
    private bool isBgmEnabled = true;    // BGM 활성화 여부
    private bool isEffectEnabled = true; // 효과음 활성화 여부
    private bool isFeverBgm = false;

    private bool blockPlayRequests = false; // 백그라운드 재생 차단

    private enum BgmRequest
    {
        None,
        Underwater,
        Fever
    }

    private BgmRequest pendingBgmRequest = BgmRequest.None;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.volume = 0.5f;

            effectSource = gameObject.AddComponent<AudioSource>();
            effectSource.loop = false;
            effectSource.playOnAwake = false;
            effectSource.volume = 0.5f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (blockPlayRequests) return;
        ApplyPendingBgm();
    }

    private bool CanPlaySound()
    {
        if (blockPlayRequests) return false;
        if (AudioListener.pause) return false;
        return true;
    }

    public void PlayUnderwaterBgm()
    {
        pendingBgmRequest = BgmRequest.Underwater;
    }

    public void PlayFeverBgm()
    {
        pendingBgmRequest = BgmRequest.Fever;
    }

    private void ApplyPendingBgm()
    {
        if (pendingBgmRequest == BgmRequest.None) return;
        if (!CanPlaySound()) return;

        if (pendingBgmRequest == BgmRequest.Underwater)
            PlayUnderwaterBgm_Internal();
        else if (pendingBgmRequest == BgmRequest.Fever)
            PlayFeverBgm_Internal();

        pendingBgmRequest = BgmRequest.None;
    }

    private void PlayUnderwaterBgm_Internal()
    {
        if (!isBgmEnabled || underwaterBgmClip == null) return;

        if (bgmSource.clip != underwaterBgmClip)
            bgmSource.clip = underwaterBgmClip;

        isFeverBgm = false;
        bgmSource.Play();
    }

    private void PlayFeverBgm_Internal()
    {
        if (!isBgmEnabled || feverBgmClip == null) return;

        if (bgmSource.clip != feverBgmClip)
            bgmSource.clip = feverBgmClip;

        isFeverBgm = true;
        bgmSource.Play();
    }

    public void StopBgm()
    {
        bgmSource.Stop();
    }

    public void PlayEatEffect()
    {
        if (!CanPlaySound()) return;
        if (!enableEatEffect) return;
        if (!isEffectEnabled || eatEffectClip == null) return;

        effectSource.Stop();
        effectSource.PlayOneShot(eatEffectClip);
    }

    public void PlayDeathEffect()
    {
        if (!CanPlaySound()) return;
        if (!enableDeathEffect) return;
        if (!isEffectEnabled || deathEffectClip == null) return;
        if (isPlayingDeathEffect) return;

        StartCoroutine(PlayDeathCoroutine());
    }

    private IEnumerator PlayDeathCoroutine()
    {
        isPlayingDeathEffect = true;

        effectSource.Stop();
        effectSource.PlayOneShot(deathEffectClip);

        yield return new WaitForSeconds(deathEffectClip.length);
        isPlayingDeathEffect = false;
    }

    public void SetBgmState(bool enabled)
    {
        isBgmEnabled = enabled;

        if (!isBgmEnabled)
        {
            StopBgm();
        }
        else
        {
            if (isFeverBgm) PlayFeverBgm();
            else PlayUnderwaterBgm();
        }
    }

    public void SetEffectState(bool enabled)
    {
        isEffectEnabled = enabled;
    }

    private void OnApplicationPause(bool pause)
    {
        blockPlayRequests = pause;

        if (pause)
        {
            bgmSource.Stop();
            effectSource.Stop();
        }
    }

    public bool IsBgmEnabled => isBgmEnabled;
    public bool IsEffectEnabled => isEffectEnabled;
}