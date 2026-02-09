using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    [SerializeField] private Vector2 minPosition = new Vector2(0f, 0f);
    [SerializeField] private Vector2 maxPosition = new Vector2(18.94f, 21.8f);

    private Camera cam;
    private float halfWidth;
    private float halfHeight;

    // 초기 설정
    private void Start()
    {
        cam = Camera.main;
        UpdateCameraSize();
    }

    // 카메라 추적
    private void LateUpdate()
    {
        if (player == null || cam == null)
            return;

        UpdateCameraSize();

        float x = Mathf.Clamp(
            player.position.x,
            minPosition.x + halfWidth,
            maxPosition.x - halfWidth
        );

        float y = Mathf.Clamp(
            player.position.y,
            minPosition.y + halfHeight,
            maxPosition.y - halfHeight
        );

        transform.position = new Vector3(x, y, transform.position.z);
    }

    // 화면비 대응
    private void UpdateCameraSize()
    {
        float baseAspect = 9f / 16f;
        float aspect = (float)Screen.width / Screen.height;

        float baseSize = 5f;

        if (aspect < baseAspect)
            cam.orthographicSize = baseSize * (baseAspect / aspect);
        else
            cam.orthographicSize = baseSize;

        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * aspect;
    }
}
