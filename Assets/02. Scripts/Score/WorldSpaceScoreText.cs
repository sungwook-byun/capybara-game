using UnityEngine;
using TMPro;

public enum LabelOwnerType
{
    Player,
    Fish
}

public class WorldSpaceScoreText : MonoBehaviour
{
    [Header("기본 설정")]
    public LabelOwnerType ownerType;
    public Vector3 baseOffset = new Vector3(0.1f, 0.6f, 0);
    public float scaleFactor = 0.3f;

    [SerializeField] private TextMeshProUGUI textUI;
    private Transform target;
    private Camera mainCam;
    private Vector3 originalTargetScale;

    private bool forceHidden = false;

    // 초기화
    public void Initialize(Transform followTarget, LabelOwnerType type, Vector3 offset, TextMeshProUGUI uiText)
    {
        target = followTarget;
        ownerType = type;
        baseOffset = offset;
        textUI = uiText;

        originalTargetScale = target.localScale;
        mainCam = Camera.main;
    }

    // 점수 텍스트 업데이트
    public void SetScore(int score)
    {
        if (textUI != null)
            textUI.text = score.ToString();
    }

    // 크기에 따라 오프셋 조정
    public void ApplyScaleOffset(Vector3 currentScale)
    {
        if (target == null) return;
        float scaleDiff = currentScale.y - originalTargetScale.y;
        baseOffset = new Vector3(baseOffset.x, 0.6f + (scaleDiff * scaleFactor), baseOffset.z);
    }

    // 좌우 오프셋 조정
    public void SetHorizontalOffset(float offsetX)
    {
        baseOffset = new Vector3(offsetX, baseOffset.y, baseOffset.z);
    }

    // 점수 라벨 숨김/표시 제어
    public void ToggleScoreVisible(bool isVisible)
    {
        forceHidden = !isVisible;

        if (textUI != null)
            textUI.gameObject.SetActive(isVisible);

        gameObject.SetActive(isVisible);
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        if (textUI == null) return;

        // 강제로 숨김 상태면 꺼두기
        if (forceHidden)
        {
            if (textUI.gameObject.activeSelf)
                textUI.gameObject.SetActive(false);
            return;
        }

        // 표시 상태면 켜기
        if (!textUI.gameObject.activeSelf)
            textUI.gameObject.SetActive(true);

        // 월드 좌표 → 스크린 좌표 변환
        Vector3 worldPos = target.position + baseOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 카메라 뒤쪽에 있으면 숨김
        if (screenPos.z < 0)
        {
            textUI.gameObject.SetActive(false);
            return;
        }

        // 스크린 좌표를 로컬 좌표로 변환
        RectTransform canvasRect = textUI.canvas.GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos))
        {
            // 플레이어 라벨은 픽셀 단위로 보정해서 떨림 최소화
            if (ownerType == LabelOwnerType.Player)
            {
                localPos.x = Mathf.Round(localPos.x);
                localPos.y = Mathf.Round(localPos.y);
            }

            textUI.rectTransform.localPosition = localPos;
        }
    }
}
