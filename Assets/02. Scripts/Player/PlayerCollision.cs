using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    // UI 및 플레이어 상태 표시 관련 변수
    public TextMeshProUGUI scoreTextUI;
    public PlayerUI playerUI;
    public WorldSpaceScoreText playerScoreLabel;

    // 사망 사운드 중복 재생 방지용 플래그
    private bool hasPlayedDeathSound = false;

    // 산소 부족 경고 아이콘 및 점멸 처리
    public GameObject[] warningIcons;
    public float blinkInterval = 0.5f;
    private Coroutine blinkRoutine;

    // 점수 및 연출 팝업 프리팹
    public GameObject worldScorePopupPrefab;
    public GameObject boneIconPopupPrefab;
    public GameObject deathIconPopupPrefab;

    // 피버타임 관련 설정
    public int fishTargetCount = 10;
    public float feverDuration = 10f;

    // 산소 시스템 설정
    public float oxygen = 1f;
    public float oxygenDecreaseRate = 0.05f;
    public float oxygenRechargeRate = 0.1f;

    // 플레이어 상태 변수
    private bool isFeverMode = false;
    private bool isDead = false;
    private float feverTimer;
    private int playerScore = 1;
    private int fishEatCount;

    // 플레이어 초기 상태 정보
    private Vector3 initialPosition;
    private PlayerFeverEffect feverEffect;

    // 플레이어 크기 성장 관련 변수
    [SerializeField] private float sizeIncreasePerScore_Player = 0.005f;
    private Vector3 originalPlayerScale;

    // 점수 라벨 위치 보정
    [SerializeField] private float labelYOffset = 0.6f;

    // 외부 접근용 값
    [HideInInspector] public Vector3 originalPlayerScale_Public;
    [HideInInspector] public float sizeIncreasePerScore_Player_Public;

    // 플레이어 이동 방향 감지용 변수
    private float lastX;
    private bool hasMoved = false;

    // 초기 값 세팅 및 기본 상태 저장
    private void Awake()
    {
        originalPlayerScale = transform.localScale;
        originalPlayerScale_Public = originalPlayerScale;
        sizeIncreasePerScore_Player_Public = sizeIncreasePerScore_Player;
        initialPosition = transform.position;
    }

    // UI 라벨 및 피버 이펙트 초기화
    private void Start()
    {
        feverEffect = GetComponent<PlayerFeverEffect>();

        if (playerScoreLabel != null)
        {
            var tmp = playerScoreLabel.GetComponentInChildren<TextMeshProUGUI>();
            playerScoreLabel.Initialize(transform, LabelOwnerType.Player, new Vector3(0.1f, labelYOffset, 0), tmp);
            playerScoreLabel.SetScore(playerScore);
        }

        lastX = transform.position.x;
        UpdateScoreText();
    }

    // 플레이어 상태를 지속적으로 갱신하는 메인 루프
    private void Update()
    {
        HandleOxygenSystem();
        HandleWarningIcons();
        HandleLabelDirection();
        HandleFeverTimer();
    }

    // 산소 감소 및 사망 판정 처리
    private void HandleOxygenSystem()
    {
        if (!isDead && !isFeverMode)
        {
            oxygen -= oxygenDecreaseRate * Time.deltaTime;
            oxygen = Mathf.Clamp01(oxygen);
            playerUI?.UpdateOxygen(oxygen);
            if (oxygen <= 0f)
                OnPlayerDeath(transform.position);
        }
    }

    // 산소 부족 시 경고 아이콘 점멸 처리
    private void HandleWarningIcons()
    {
        if (oxygen <= 0.3f)
        {
            if (blinkRoutine == null)
                blinkRoutine = StartCoroutine(BlinkWarnings());
        }
        else
        {
            if (blinkRoutine != null)
            {
                StopCoroutine(blinkRoutine);
                blinkRoutine = null;
            }
            foreach (var icon in warningIcons)
                if (icon != null) icon.SetActive(false);
        }
    }

    // 플레이어 이동 방향에 따른 점수 라벨 위치 보정
    private void HandleLabelDirection()
    {
        float deltaX = transform.position.x - lastX;
        if (Mathf.Abs(deltaX) > 0.001f) hasMoved = true;

        if (hasMoved)
        {
            if (deltaX > 0.001f)
                playerScoreLabel?.SetHorizontalOffset(0.1f);
            else if (deltaX < -0.001f)
                playerScoreLabel?.SetHorizontalOffset(-0.1f);
        }
        lastX = transform.position.x;
    }

    // 피버타임 지속 시간 관리 및 종료 처리
    private void HandleFeverTimer()
    {
        if (!isFeverMode) return;

        feverTimer += Time.deltaTime;
        playerUI?.UpdateFeverFill(Mathf.Max(0f, feverDuration - feverTimer));

        if (feverTimer >= feverDuration)
            EndFeverMode();
    }

    // 경고 아이콘 점멸 코루틴
    private IEnumerator BlinkWarnings()
    {
        bool isOn = false;
        while (true)
        {
            isOn = !isOn;
            foreach (var icon in warningIcons)
                if (icon != null) icon.SetActive(isOn);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    // 산소 회복 처리
    public void RechargeOxygen(float amount)
    {
        if (isDead) return;
        oxygen += amount;
        oxygen = Mathf.Clamp01(oxygen);
        playerUI?.UpdateOxygen(oxygen);
    }

    // 물고기 충돌 처리 및 점수 판정
    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (!other.CompareTag("Fish")) return;

        var fs = other.GetComponent<FishManager.FishRuntimeScore>();
        if (fs == null) return;

        bool canEat = isFeverMode || fs.scoreValue <= playerScore;

        if (canEat)
            HandleEatFish(other, fs);
        else
            OnPlayerDeath(other.transform.position);
    }

    // 물고기 섭취 시 처리 로직
    private void HandleEatFish(Collider other, FishManager.FishRuntimeScore fs)
    {
        int addAmount = GetAddScoreByFishValue(fs.scoreValue);
        AddScore(addAmount);

        SoundManager.Instance?.PlayEatEffect();

        if (!isFeverMode)
            HandleFeverCharge();

        ShowWorldScorePopup(other.transform.position, addAmount);
        ShowBoneIconPopup(other.transform.position);

        var label = other.GetComponentInChildren<WorldSpaceScoreText>();
        if (label != null) Destroy(label.gameObject);

        Destroy(other.transform.root.gameObject);
    }

    // 피버 게이지 누적 처리
    private void HandleFeverCharge()
    {
        fishEatCount++;
        playerUI?.UpdateFishEatCount(fishEatCount, fishTargetCount);
        playerUI?.UpdateNormalFill((float)fishEatCount / fishTargetCount);

        if (fishEatCount >= fishTargetCount)
            StartFeverMode();
    }

    // 플레이어 사망 처리
    private void OnPlayerDeath(Vector3 deathPosition)
    {
        if (isDead) return;
        isDead = true;

        if (!hasPlayedDeathSound)
        {
            SoundManager.Instance?.PlayDeathEffect();
            hasPlayedDeathSound = true;
        }

        ShowDeathIconPopup(deathPosition);

        var labels = Object.FindObjectsByType<WorldSpaceScoreText>(FindObjectsSortMode.None);
        foreach (var label in labels)
            label.ToggleScoreVisible(false);

        StartCoroutine(DisablePlayerAfterSound());
        GameManager.Instance?.StartCoroutine(GameManager.Instance.DelayGameOver(1f));
    }

    // 사망 연출 후 플레이어 비활성화
    private IEnumerator DisablePlayerAfterSound()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        playerUI?.Hide();
        scoreTextUI?.gameObject.SetActive(false);
        playerScoreLabel?.ToggleScoreVisible(false);
    }

    // 점수에 따른 추가 점수 계산
    private int GetAddScoreByFishValue(int v)
    {
        if (v >= 500) return 10;
        if (v >= 400) return 9;
        if (v >= 300) return 8;
        if (v >= 200) return 7;
        if (v >= 100) return 6;
        if (v >= 40) return 5;
        if (v >= 30) return 4;
        if (v >= 20) return 3;
        if (v >= 10) return 2;
        return 1;
    }

    // 점수 증가 및 플레이어 크기 갱신
    private void AddScore(int amt)
    {
        playerScore += amt;
        UpdateScoreText();
        UpdatePlayerScale();
    }

    // 점수 기반 플레이어 크기 조절
    public void UpdatePlayerScale()
    {
        float scaleFactor = 1f + (playerScore * sizeIncreasePerScore_Player);
        transform.localScale = originalPlayerScale * scaleFactor;
        UpdateLabelOffset();
    }

    // 라벨 위치 보정 갱신
    private void UpdateLabelOffset()
    {
        playerScoreLabel?.ApplyScaleOffset(transform.localScale);
    }

    // 피버타임 시작 처리
    private void StartFeverMode()
    {
        isFeverMode = true;
        feverTimer = 0f;

        feverEffect?.StartFever(feverDuration);
        playerUI?.SetFeverMode(true, feverDuration);
        playerScoreLabel?.ToggleScoreVisible(false);

        SoundManager.Instance?.PlayFeverBgm();
    }

    // 피버타임 종료 처리
    private void EndFeverMode()
    {
        isFeverMode = false;
        fishEatCount = 0;

        feverEffect?.EndFever();
        UpdatePlayerScale();

        playerUI?.UpdateFishEatCount(fishEatCount, fishTargetCount);
        playerUI?.SetFeverMode(false);
        playerScoreLabel?.ToggleScoreVisible(true);

        SoundManager.Instance?.PlayUnderwaterBgm();
    }

    // 점수 UI 갱신
    public void UpdateScoreText()
    {
        playerScoreLabel?.SetScore(playerScore);
        playerUI?.UpdateScore(playerScore);
    }

    // 게임 재시작 시 상태 초기화
    public void ResetScore()
    {
        isDead = false;
        hasPlayedDeathSound = false;

        playerScore = 1;
        isFeverMode = false;
        feverTimer = 0f;
        fishEatCount = 0;
        oxygen = 1f;

        hasMoved = true;
        lastX = transform.position.x;
        playerScoreLabel?.SetHorizontalOffset(0.1f);

        UpdatePlayerScale();
        UpdateScoreText();
        feverEffect?.EndFever();

        playerScoreLabel?.ToggleScoreVisible(true);
        playerUI?.UpdateOxygen(oxygen);
    }

    // 점수 팝업 생성
    private void ShowWorldScorePopup(Vector3 fishWorldPos, int amt)
    {
        if (!worldScorePopupPrefab) return;

        var popupObj = Instantiate(worldScorePopupPrefab, playerUI.transform.parent);
        var popup = popupObj.GetComponent<WorldScorePopup>();
        if (popup != null) popup.Setup(amt, fishWorldPos);
    }

    // 뼈 아이콘 팝업 생성
    private void ShowBoneIconPopup(Vector3 pos)
    {
        if (!boneIconPopupPrefab) return;

        Quaternion randomRot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        GameObject bone = Instantiate(boneIconPopupPrefab, pos + Vector3.up, randomRot);

        Rigidbody rb = bone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomDir = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1.5f),
                0f
            ).normalized;

            rb.AddForce(randomDir * Random.Range(2f, 5f), ForceMode.Impulse);
            rb.AddTorque(Vector3.forward * Random.Range(-5f, 5f), ForceMode.Impulse);
        }
    }

    // 사망 아이콘 팝업 생성
    private void ShowDeathIconPopup(Vector3 pos)
    {
        if (!deathIconPopupPrefab) return;
        Instantiate(deathIconPopupPrefab, pos + Vector3.up, Quaternion.identity);
    }

    // 외부 접근용 점수 반환
    public int GetPlayerScore()
    {
        return playerScore;
    }
}