using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI 참조")]
    public GameObject mainMenuPanel; // 메인 메뉴 UI 패널

    [Header("게임 오브젝트 참조")]
    public FishManager fishManager;           // 물고기 스폰 스크립트
    public PlayerController player;       // 플레이어 이동 스크립트
    public Camera mainCamera;                 // 메인 카메라
    public CameraFollow cameraFollow;         // 카메라 팔로우 스크립트

    [Header("게임 시작 시 카메라 설정")]
    public Vector3 startCamPosition = new Vector3(0f, 0f, -10f); // 게임 시작 시 이동할 카메라 좌표
    public Vector3 startCamRotation = new Vector3(0f, 0f, 0f);    // 게임 시작 시 카메라 회전
    public float startCamSize = 5f;                               // 게임 시작 시 카메라 orthographicSize

    private void Start()
    {
        // 📌 초기 카메라 사이즈
        if (mainCamera != null)
            mainCamera.orthographicSize = 630f;

        // 게임 시작 전 비활성화
        if (player != null) player.enabled = false;
        if (fishManager != null) fishManager.enabled = false;
        if (cameraFollow != null) cameraFollow.enabled = false;
    }

    public void StartGame()
    {
        // 메인 메뉴 숨기기
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        // 📌 카메라 위치·회전·크기 변경
        if (mainCamera != null)
        {
            mainCamera.transform.position = startCamPosition;
            mainCamera.transform.rotation = Quaternion.Euler(startCamRotation);
            mainCamera.orthographicSize = startCamSize;
        }

        // 카메라 팔로우 켜기
        if (cameraFollow != null)
            cameraFollow.enabled = true;

        // 플레이어 이동/스폰 켜기
        if (player != null) player.enabled = true;
        if (fishManager != null) fishManager.enabled = true;

        // 시간 재개
        Time.timeScale = 1f;
    }
}