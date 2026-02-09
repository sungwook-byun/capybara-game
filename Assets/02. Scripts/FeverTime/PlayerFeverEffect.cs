using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerFeverEffect : MonoBehaviour
{
    public float speedMultiplier = 2f;
    public Vector3 sizeMultiplier = Vector3.one * 3f;
    public GameObject feverEffectPrefab;
    public Vector3 effectOffset = new(0, 0.5f, 0);

    [Header("UI 연동")]
    public WorldSpaceScoreText playerScoreLabel; // 인스펙터에서 플레이어 스코어 UGUI 할당

    private PlayerController controller;
    private PlayerCollision playerCollision;
    private GameObject feverEffectInstance;
    private bool isFever = false;

    private Vector3 targetScale;
    public float scaleLerpSpeed = 5f;
    private float originalSpeed;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        playerCollision = GetComponent<PlayerCollision>();
        originalSpeed = controller.speed;
        targetScale = transform.localScale;
    }

    void Update()
    {
        float currentSign = Mathf.Sign(transform.localScale.x);
        Vector3 absCurrentScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        Vector3 absTargetScale = new Vector3(Mathf.Abs(targetScale.x), targetScale.y, targetScale.z);
        Vector3 newScale = Vector3.Lerp(absCurrentScale, absTargetScale, Time.deltaTime * scaleLerpSpeed);
        transform.localScale = new Vector3(newScale.x * currentSign, newScale.y, newScale.z);

        if (isFever && feverEffectInstance != null)
        {
            feverEffectInstance.transform.position = transform.position + effectOffset;
            feverEffectInstance.transform.rotation = Quaternion.identity;
        }

        // 크기에 따른 스코어 라벨 오프셋 자동 반영
        if (playerScoreLabel != null)
        {
            playerScoreLabel.ApplyScaleOffset(transform.localScale);
        }
    }

    public void StartFever(float duration)
    {
        isFever = true;

        // 피버 시작 시 플레이어 스코어 숨기기
        if (playerScoreLabel != null && playerScoreLabel.ownerType == LabelOwnerType.Player)
            playerScoreLabel.ToggleScoreVisible(false);

        controller.speed = originalSpeed * speedMultiplier;
        targetScale = Vector3.Scale(transform.localScale, sizeMultiplier);

        if (feverEffectPrefab != null && feverEffectInstance == null)
        {
            feverEffectInstance = Instantiate(feverEffectPrefab);
            feverEffectInstance.transform.position = transform.position + effectOffset;
            feverEffectInstance.transform.rotation = Quaternion.identity;
        }

        // 피버타임 BGM
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBgmState(true);
            SoundManager.Instance.PlayFeverBgm();
        }
            
    }

    public void EndFever()
    {
        isFever = false;

        // 피버 끝나면 무조건 다시 보이게
        if (playerScoreLabel != null && playerScoreLabel.ownerType == LabelOwnerType.Player)
            playerScoreLabel.ToggleScoreVisible(true);

        controller.speed = originalSpeed;

        if (playerCollision != null)
        {
            float scaleFactor = 1f + (playerCollision.GetPlayerScore() * playerCollision.sizeIncreasePerScore_Player_Public);
            targetScale = playerCollision.originalPlayerScale_Public * scaleFactor;
        }

        if (feverEffectInstance != null)
        {
            Destroy(feverEffectInstance);
            feverEffectInstance = null;
        }

        // 일반 bgm 복귀 (피버타임 종료시)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUnderwaterBgm();
        }
            
    }
}
