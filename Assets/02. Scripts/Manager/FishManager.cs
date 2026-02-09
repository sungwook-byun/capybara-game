using UnityEngine;
using TMPro;
using static WorldSpaceScoreText; // LabelOwnerType 사용

[System.Serializable]
public class FishInfo
{
    public GameObject prefab;
    public int minScore = 0;
    public int maxScore = 0;
    public bool showScoreText = true;
    public bool keepOriginalScale = false;
}

public class FishManager : MonoBehaviour
{
    public class FishRuntimeScore : MonoBehaviour
    {
        public static System.Collections.Generic.List<FishRuntimeScore> allFishes = new();
        public int scoreValue;

        private void Awake() => allFishes.Add(this);
        private void OnDestroy() => allFishes.Remove(this);
    }

    [Header("물고기 정보")]
    public FishInfo[] fishes;

    [Header("스폰 주기")]
    public float lowSpawnInterval = 1f;
    public float otherSpawnInterval = 2f;

    [Header("스폰 범위")]
    public float minX = 0f;
    public float maxX = 19.4f;
    public float minY = 1.5f;
    public float maxY = 18.3f;

    private float fixedZ = 0f;

    [Header("플레이어 점수")]
    public int playerScore = 1; // GameManager에서 자동 갱신

    [Header("속도 설정")]
    public float baseMinSpeed = 1f;
    public float baseMaxSpeed = 3f;
    public float proportionalIncrease = 0.005f;
    public float maxSpeedLimit = 5f;

    [Header("점수 라벨 Y 오프셋")]
    public float labelYOffset = 0.6f;

    [Header("점수 라벨 스케일 계수")]
    public float labelScaleFactor = 0.3f;

    [Header("UGUI 점수 프리팹")]
    public GameObject scoreLabelPrefab;
    public Transform canvasParent;

    private float lowTimer;
    private float otherTimer;

    private const float sizeIncreasePerScore = 0.005f;

    void Update()
    {
        if (GameManager.Instance != null)
            playerScore = Mathf.Max(GameManager.Instance.CurrentScore, 1);

        lowTimer += Time.deltaTime;
        if (lowTimer >= lowSpawnInterval)
        {
            lowTimer = 0f;
            SpawnCategoryFish("lowOnly");
        }

        otherTimer += Time.deltaTime;
        if (otherTimer >= otherSpawnInterval)
        {
            otherTimer = 0f;
            SpawnCategoryFish("mixed");
        }
    }

    void SpawnCategoryFish(string category)
    {
        if (fishes == null || fishes.Length == 0) return;

        FishInfo info = fishes[Random.Range(0, fishes.Length)];
        int scoreValue;

        bool hasCustomScoreRange = (info.minScore > 0 || info.maxScore > 0);

        if (hasCustomScoreRange)
        {
            scoreValue = Random.Range(info.minScore, info.maxScore + 1);
        }
        else
        {
            scoreValue = GetRandomScoreBasedOnPlayer(category);
        }

        if (!hasCustomScoreRange && scoreValue > playerScore * 2 && CountOverTwoTimes() >= 2)
        {
            scoreValue = Random.Range(playerScore + 1, playerScore * 2 + 1);
        }

        SpawnFishObject(info, scoreValue);
    }

    int GetRandomScoreBasedOnPlayer(string category)
    {
        float r = Random.value;

        if (category == "lowOnly")
            return (r <= 0.70f) ?
                Random.Range(1, playerScore + 1) :
                Random.Range(playerScore + 1, playerScore * 2 + 1);

        if (r <= 0.70f) return Random.Range(1, playerScore + 1);
        if (r <= 0.99f) return Random.Range(playerScore + 1, playerScore * 2 + 1);

        return (CountOverTwoTimes() >= 2) ?
            Random.Range(playerScore + 1, playerScore * 2 + 1) :
            Random.Range(playerScore * 2 + 1, playerScore * 3 + 1);
    }

    int CountOverTwoTimes()
    {
        int count = 0;
        foreach (var fish in FishRuntimeScore.allFishes)
        {
            if (fish.scoreValue > playerScore * 2)
                count++;
        }
        return count;
    }

    void SpawnFishObject(FishInfo info, int scoreValue)
    {
        float spawnX = (Random.value < 0.5f) ? minX : maxX;
        float spawnY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, fixedZ - 0.1f);

        bool moveRight = (spawnX == minX);
        Quaternion rot = Quaternion.Euler(90f, moveRight ? 0f : 180f, 0f);

        GameObject fishObj = Instantiate(info.prefab, spawnPos, rot);

        if (!info.keepOriginalScale)
        {
            float scaleFactor = 1f + (playerScore * sizeIncreasePerScore);
            fishObj.transform.localScale *= scaleFactor;
        }

        FishMovement move = fishObj.GetComponent<FishMovement>();
        if (move != null)
        {
            move.SetDirection(moveRight ? Vector3.right : Vector3.left);

            float minSpd = Mathf.Min(baseMinSpeed + playerScore * proportionalIncrease, maxSpeedLimit);
            float maxSpd = Mathf.Min(baseMaxSpeed + playerScore * proportionalIncrease, maxSpeedLimit);
            move.speed = Random.Range(minSpd, maxSpd);
        }

        var scoreComp = fishObj.AddComponent<FishRuntimeScore>();
        scoreComp.scoreValue = scoreValue;

        if (info.showScoreText && scoreLabelPrefab != null && canvasParent != null)
        {
            GameObject labelObj = Instantiate(scoreLabelPrefab, canvasParent);

            // 항상 맨 앞으로 오도록 설정
            labelObj.transform.SetAsFirstSibling();


            var tmp = labelObj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = scoreValue.ToString();
                tmp.fontSize = 30;
                tmp.color = Color.yellow;
            }

            var labelScript = labelObj.GetComponent<WorldSpaceScoreText>();
            if (labelScript != null)
            {
                float scaleBonus = fishObj.transform.localScale.y * labelScaleFactor;
                Vector3 offset = new Vector3(0, labelYOffset + scaleBonus, 0);

                labelScript.Initialize(fishObj.transform, LabelOwnerType.Fish, offset, tmp);
                labelScript.SetScore(scoreValue);
            }
        }
    }

    public void DespawnFish(GameObject fishObj)
    {
        Collider col = fishObj.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Destroy(fishObj, 0.05f);
    }
}
