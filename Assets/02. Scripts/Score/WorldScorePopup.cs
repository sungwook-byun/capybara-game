using TMPro;
using UnityEngine;

public class WorldScorePopup : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public float moveUpSpeed = 0.5f;  // 월드 좌표 기준 상승 속도
    public float lifeTime = 1f;

    private float timer;
    private Vector3 worldPosition; // 현재 월드 좌표
    private Color originalColor;
    private Canvas parentCanvas;

    public void Setup(int score, Vector3 startWorldPos)
    {
        if (scoreText != null)
        {
            scoreText.text = "+" + score.ToString();
            scoreText.color = Color.green;
            originalColor = scoreText.color;
        }

        worldPosition = startWorldPos;
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        // 월드 좌표에서 위로 이동
        worldPosition += Vector3.up * moveUpSpeed * Time.deltaTime;

        // 월드 → 스크린 → 로컬 좌표 변환
        if (parentCanvas != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                screenPos,
                parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
                out Vector2 localPos
            );

            (transform as RectTransform).localPosition = localPos;
        }

        // 투명도 조절
        timer += Time.deltaTime;
        float alpha = Mathf.Clamp01(1f - (timer / lifeTime));
        scoreText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}
