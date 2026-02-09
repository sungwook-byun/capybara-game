using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    [Header("스프라이트 이미지")]
    public Sprite spriteOn;   // 켜졌을 때 아이콘
    public Sprite spriteOff;  // 꺼졌을 때 아이콘

    private bool isOn = true;
    private Image buttonImage;

    private void Awake()
    {
        // 버튼(GameObject)에 붙은 Image 자동으로 가져오기
        buttonImage = GetComponent<Image>();
    }

    private void Start()
    {
        // SoundManager 상태와 동기화
        if (SoundManager.Instance != null)
            isOn = SoundManager.Instance.IsBgmEnabled;

        UpdateImage();
    }

    public void Toggle()
    {
        isOn = !isOn;
        UpdateImage();

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetBgmState(isOn);
    }

    private void UpdateImage()
    {
        if (buttonImage != null)
            buttonImage.sprite = isOn ? spriteOn : spriteOff;
    }
}
