using UnityEngine;
using AppsInToss;

public class TossAudioGate : MonoBehaviour
{
    static TossAudioGate instance;
    bool isMuted;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        AITVisibilityHelper.OnVisibilityChanged -= OnVisibilityChanged;
        AITVisibilityHelper.OnVisibilityChanged += OnVisibilityChanged;

        Apply(AITVisibilityHelper.IsVisible);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            AITVisibilityHelper.OnVisibilityChanged -= OnVisibilityChanged;
            instance = null;
        }
    }

    void OnVisibilityChanged(bool isVisible)
    {
        Apply(isVisible);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            Mute();
        else
            Unmute();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            Mute();
        else
            Unmute();
    }

    void Apply(bool isVisible)
    {
        if (!isVisible)
            Mute();
        else
            Unmute();
    }

    void Mute()
    {
        if (isMuted) return;
        isMuted = true;

        AudioListener.pause = true;
        AudioListener.volume = 0f;
    }

    void Unmute()
    {
        if (!isMuted) return;
        isMuted = false;

        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }
}