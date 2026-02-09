using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 3f;
    public SimpleJoystick joystick;

    public float baseFacingOffsetY = 90f;
    public float fixedXRotation = 26f;
    public float rotationSmoothSpeed = 10f;

    public Transform visual;
    public Transform colliderPivot;

    private Rigidbody rb;
    private bool startPositionSet = false;

    private float targetYRotation;
    private float smoothedYRotation;

    private const float inputDead = 0.1f;

    private readonly Vector3 minPosition = new Vector3(0f, 0f, 0f);
    private readonly Vector3 maxPosition = new Vector3(18.94f, 18.8f, 0f);

    private bool inOceanZone = false;
    private float oceanZoneTiltZ = 0f;

    // 초기 설정
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        targetYRotation = baseFacingOffsetY;
        smoothedYRotation = targetYRotation;

        SetStartPosition();
    }

    // 시작 위치 설정
    private void SetStartPosition()
    {
        Camera cam = Camera.main;
        Vector3 camCenter = cam != null ? cam.transform.position : new Vector3(9.47f, 10.4f, 0f);

        Vector2 half = GetHalfExtentsForClamp();

        float clampedX = Mathf.Clamp(camCenter.x, minPosition.x + half.x, maxPosition.x - half.x);
        float clampedY = Mathf.Clamp(camCenter.y, minPosition.y + half.y, maxPosition.y - half.y);

        rb.position = new Vector3(clampedX, clampedY, 0f);
        startPositionSet = true;
    }

    // 콜라이더 크기 기반 반경 계산
    private Vector2 GetHalfExtentsForClamp()
    {
        if (colliderPivot != null)
        {
            BoxCollider col = colliderPivot.GetComponent<BoxCollider>();
            if (col != null)
            {
                Vector3 ext = col.bounds.extents;
                return new Vector2(ext.x, ext.y);
            }
        }

        return new Vector2(0.5f, 0.5f);
    }

    // 입력에 따른 회전 처리
    private void Update()
    {
        if (!startPositionSet || joystick == null)
            return;

        Vector2 inputDir = joystick.Direction;

        if (inputDir.x > inputDead)
        {
            targetYRotation = baseFacingOffsetY;
            if (inOceanZone) oceanZoneTiltZ = 40f;
        }
        else if (inputDir.x < -inputDead)
        {
            targetYRotation = baseFacingOffsetY + 180f;
            if (inOceanZone) oceanZoneTiltZ = -40f;
        }

        smoothedYRotation = Mathf.LerpAngle(
            smoothedYRotation,
            targetYRotation,
            Time.deltaTime * rotationSmoothSpeed
        );

        if (colliderPivot != null)
            colliderPivot.localRotation = Quaternion.identity;
    }

    // 물리 이동 처리
    private void FixedUpdate()
    {
        if (!startPositionSet || joystick == null)
            return;

        Vector2 inputDir = joystick.Direction;

        if (inputDir.magnitude < inputDead)
            return;

        Vector3 moveDir = new Vector3(inputDir.x, inputDir.y, 0f).normalized;
        Vector3 newPos = rb.position + moveDir * speed * Time.fixedDeltaTime;

        Vector2 half = GetHalfExtentsForClamp();

        newPos.x = Mathf.Clamp(
            newPos.x,
            minPosition.x + half.x,
            maxPosition.x - half.x
        );

        newPos.y = Mathf.Clamp(
            newPos.y,
            minPosition.y + half.y,
            maxPosition.y - half.y
        );

        newPos.z = 0f;

        rb.MovePosition(newPos);

        ApplyVisualRotation();
    }

    // 시각적 회전 적용
    private void ApplyVisualRotation()
    {
        float targetZ = inOceanZone ? oceanZoneTiltZ : 0f;

        float newZ = Mathf.LerpAngle(
            transform.localRotation.eulerAngles.z,
            targetZ,
            Time.deltaTime * rotationSmoothSpeed
        );

        transform.localRotation = Quaternion.Euler(
            transform.localRotation.eulerAngles.x,
            transform.localRotation.eulerAngles.y,
            newZ
        );

        if (visual != null)
        {
            visual.localRotation = Quaternion.Euler(
                fixedXRotation,
                smoothedYRotation,
                0f
            );
        }
    }

    // 오션존 진입 처리
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OceanZone"))
        {
            inOceanZone = true;
        }
    }

    // 오션존 이탈 처리
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("OceanZone"))
        {
            inOceanZone = false;
            oceanZoneTiltZ = 0f;
        }
    }

    // 상태 초기화
    public void ResetState()
    {
        inOceanZone = false;
        oceanZoneTiltZ = 0f;

        Vector3 euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(euler.x, euler.y, 0f);
    }
}
