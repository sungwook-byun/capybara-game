using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FishMovement : MonoBehaviour
{
    public float speed = 2f;

    private Vector3 moveDirection = Vector3.right;

    private float minX = 0f;
    private float maxX = 18.94f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // kinematic으로 설정 (물리 힘/중력 영향 안 받게)
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;

        // 물고기 오브젝트가 이동 방향을 향해 바라보도록 회전 조절
        if (moveDirection.x > 0)
            transform.rotation = Quaternion.Euler(90, 0, 0);   // 오른쪽
        else if (moveDirection.x < 0)
            transform.rotation = Quaternion.Euler(90, 180, 0); // 왼쪽
    }

    void FixedUpdate()
    {
        Vector3 newPos = rb.position + moveDirection * speed * Time.fixedDeltaTime;

        // X 범위를 넘어가면 오브젝트 파괴
        if (newPos.x < minX - 1f || newPos.x > maxX + 1f)
        {
            Destroy(gameObject);
            return;
        }

        rb.MovePosition(newPos);
    }
}
