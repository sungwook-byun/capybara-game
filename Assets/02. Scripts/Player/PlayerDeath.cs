using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public float moveUpSpeed = 1f;     // 위로 이동 속도
    public float lifeTime = 1f;        // 전체 생존 시간
    public float fadeOutTime = 0.5f;   // 투명해지기 시작 시간

    private float timer = 0f;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 위로 이동
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        // 투명도 점진적으로 줄이기
        if (spriteRenderer != null && timer > (lifeTime - fadeOutTime))
        {
            float alpha = Mathf.Lerp(1f, 0f, (timer - (lifeTime - fadeOutTime)) / fadeOutTime);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        // 종료 시간 지나면 삭제
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
