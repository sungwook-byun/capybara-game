using UnityEngine;
using AppsInToss;

public class TossAudioGate : MonoBehaviour
{
    static TossAudioGate _instance;
    bool _muted;

    void Awake()
    {
        // 중복 생성 방지
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // 구독
        AITVisibilityHelper.OnVisibilityChanged -= OnVisChanged;
        AITVisibilityHelper.OnVisibilityChanged += OnVisChanged;

        // 시작 상태 반영
        Apply(AITVisibilityHelper.IsVisible);
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            AITVisibilityHelper.OnVisibilityChanged -= OnVisChanged;
            _instance = null;
        }
    }

    void OnVisChanged(bool isVisible) => Apply(isVisible);

    void Apply(bool isVisible)
    {
        // Visible=true면 소리 ON, false면 OFF
        bool shouldMute = !isVisible;
        if (_muted == shouldMute) return;
        _muted = shouldMute;

        if (_muted)
        {
            AudioListener.pause = true;
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.pause = false;
            AudioListener.volume = 1f;
        }

        // Debug.Log($"[TossAudioGate] isVisible={isVisible} => muted={_muted}");
    }
}