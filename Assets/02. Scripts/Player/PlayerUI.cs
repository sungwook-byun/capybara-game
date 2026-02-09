using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI 오브젝트")]
    public GameObject uiImage;

    [Header("UI 참조")]
    public Image fillImage;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI fishEatCountText;

    [Header("산소 게이지")]
    public Image oxygenFillImage;
    public Gradient oxygenGradient;

    [Header("피버 게이지")]
    public Gradient fillGradient;

    [Header("설정 화면")]
    public GameObject settingButton;  // [Button] Setting 객체 (Dim, View, Button, Sound가 자식으로 들어있음)

    private bool isFeverMode = false;
    private float feverTotalTime = 0f;
    private float feverRemainingTime = 0f;
    private int fishTargetCount = 10;

    private void Awake()
    {
        if (uiImage != null)
            uiImage.SetActive(false);

        // 시작 시 환경설정 패널 꺼두기
        if (settingButton != null)
        {
            foreach (Transform child in settingButton.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        ResetUI();
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    public void UpdateFishEatCount(int currentCount, int targetCount)
    {
        if (fishEatCountText != null)
            fishEatCountText.text = currentCount + " / " + targetCount;
    }

    public void UpdateNormalFill(float fillRatio)
    {
        if (isFeverMode) return;
        ApplyFill(fillRatio);
    }

    public void UpdateFeverFill(float remainingTime)
    {
        if (!isFeverMode) return;

        feverRemainingTime = remainingTime;
        float ratio = Mathf.Clamp01(feverRemainingTime / feverTotalTime);

        if (fillImage != null)
        {
            fillImage.fillAmount = ratio;
            if (fillGradient != null)
                fillImage.color = fillGradient.Evaluate(ratio);
        }
    }

    private void ApplyFill(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        if (fillImage != null)
        {
            fillImage.fillAmount = ratio;
            if (fillGradient != null)
                fillImage.color = fillGradient.Evaluate(ratio);
        }
    }

    public void SetFeverMode(bool active, float totalTime = 0f)
    {
        isFeverMode = active;
        feverTotalTime = active ? totalTime : 0f;
        feverRemainingTime = feverTotalTime;

        if (active) ApplyFill(1f);
        else ApplyFill(0f);
    }

    public void ResetUI()
    {
        ApplyFill(0f);
        if (scoreText != null)
            scoreText.text = "0";
        if (fishEatCountText != null)
            fishEatCountText.text = "0 / " + fishTargetCount;

        if (oxygenFillImage != null)
        {
            oxygenFillImage.fillAmount = 1f;
            if (oxygenGradient != null)
                oxygenFillImage.color = oxygenGradient.Evaluate(1f);
        }

        isFeverMode = false;
        feverTotalTime = 0f;
        feverRemainingTime = 0f;
    }

    public void Show()
    {
        if (uiImage != null)
            uiImage.SetActive(true);
    }

    public void Hide()
    {
        if (uiImage != null)
            uiImage.SetActive(false);
    }

    public void UpdateOxygen(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        if (oxygenFillImage != null)
        {
            oxygenFillImage.fillAmount = ratio;
            if (oxygenGradient != null)
                oxygenFillImage.color = oxygenGradient.Evaluate(ratio);
        }
    }

    public void OpenSettings()
    {
        if (settingButton != null)
        {
            Transform parent = settingButton.transform.parent;

            // 형제 오브젝트 4개 켜기
            Transform dim = parent.Find("Dim");
            Transform view = parent.Find("View");
            Transform panelButton = parent.Find("Button");
            Transform panelSound = parent.Find("Sound");

            if (dim) dim.gameObject.SetActive(true);
            if (view) view.gameObject.SetActive(true);
            if (panelButton) panelButton.gameObject.SetActive(true);
            if (panelSound) panelSound.gameObject.SetActive(true);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.PauseGame();
    }

    public void CloseSettings()
    {
        if (settingButton != null)
        {
            Transform parent = settingButton.transform.parent;

            Transform dim = parent.Find("Dim");
            Transform view = parent.Find("View");
            Transform panelButton = parent.Find("Button");
            Transform panelSound = parent.Find("Sound");

            if (dim) dim.gameObject.SetActive(false);
            if (view) view.gameObject.SetActive(false);
            if (panelButton) panelButton.gameObject.SetActive(false);
            if (panelSound) panelSound.gameObject.SetActive(false);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
    }
}
