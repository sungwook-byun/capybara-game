using UnityEngine;
using UnityEngine.UI;

public class EffectToggle : MonoBehaviour
{
    [Header("스프라이트 이미지")]
    public Sprite spriteOn;   // 켜졌을 때 아이콘
    public Sprite spriteOff;  // 꺼졌을 때 아이콘

    private bool isOn = true;
    private Image buttonImage;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    private void Start()
    {
        if (SoundManager.Instance != null)
            isOn = SoundManager.Instance.IsEffectEnabled;

        UpdateImage();
    }

    public void Toggle()
    {
        isOn = !isOn;
        UpdateImage();

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetEffectState(isOn);
    }

    private void UpdateImage()
    {
        if (buttonImage != null)
            buttonImage.sprite = isOn ? spriteOn : spriteOff;
    }
}
    