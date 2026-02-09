using UnityEngine;

public class WorldBonePopup : MonoBehaviour
{
    public float lifeTime = 1f;         // 생존 시간
    public float fadeOutTime = 0.5f;    // 서서히 사라지는 시간
    public float moveSpeed = 1f;        // 기본 이동 속도

    private float timer = 0f;
    private SpriteRenderer sr;
    private Vector3 randomDir;          // 랜덤 이동 방향

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        // x와 y축 각각 랜덤 값 (예: -1 ~ 1)
        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(0.5f, 1f); // 위쪽으로 살짝 가는 범위
        randomDir = new Vector3(randX, randY, 0f).normalized;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 랜덤 방향으로 이동
        transform.position += randomDir * moveSpeed * Time.deltaTime;

        // 페이드 아웃
        if (sr != null && timer > (lifeTime - fadeOutTime))
        {
            float alpha = Mathf.Lerp(1f, 0f, (timer - (lifeTime - fadeOutTime)) / fadeOutTime);
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}
