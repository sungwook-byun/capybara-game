using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject startMenuPanel;
    public GameObject gameOverPanel;
    public PlayerUI playerUI;

    [Header("게임오버 UI")]
    public TextMeshProUGUI finalScoreText;

    [Header("플레이어")]
    public Transform player;
    private readonly Vector3 playerStartPos = new Vector3(2.9f, 4.9f, 0f);
    public MonoBehaviour playerController;

    private bool isGameOver = false;

    public int CurrentScore
    {
        get
        {
            if (player != null)
            {
                var pc = player.GetComponent<PlayerCollision>();
                if (pc != null)
                    return pc.GetPlayerScore();
            }
            return 0;
        }
    }

    void Awake()
    {
        Instance = this;
        Time.timeScale = 0f;

        if (startMenuPanel) startMenuPanel.SetActive(true);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        Physics.SyncTransforms();
    }

    public void StartGame()
    {
        isGameOver = false;

        if (player != null)
        {
            player.position = playerStartPos;
            player.gameObject.SetActive(true);

            var ctrl = player.GetComponent<PlayerController>();
            if (ctrl != null) ctrl.ResetState();

            if (playerUI != null)
            {
                playerUI.ResetUI();
                playerUI.Show();
                playerUI.CloseSettings();

                if (playerUI.settingButton != null)
                {
                    foreach (Transform child in playerUI.settingButton.transform)
                        child.gameObject.SetActive(false);
                }
            }

            var pc = player.GetComponent<PlayerCollision>();
            if (pc != null)
            {
                pc.ResetScore();
                if (pc.scoreTextUI != null)
                    pc.scoreTextUI.gameObject.SetActive(true);
            }
        }

        if (playerController != null)
            playerController.enabled = true;

        foreach (var fish in GameObject.FindGameObjectsWithTag("Fish"))
        {
            Collider col = fish.GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            Destroy(fish, 0.05f);
        }

        if (startMenuPanel) startMenuPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        var labels = Object.FindObjectsByType<WorldSpaceScoreText>(FindObjectsSortMode.None);
        foreach (var label in labels)
            label.ToggleScoreVisible(true);

        Time.timeScale = 1f;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBgmState(true);
            SoundManager.Instance.PlayUnderwaterBgm();
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        var pc = player != null ? player.GetComponent<PlayerCollision>() : null;
        if (pc != null && finalScoreText != null)
        {
            int finalScore = pc.GetPlayerScore();
            finalScoreText.text = $"{finalScore}";
        }

        if (playerUI != null)
            playerUI.Hide();

        if (gameOverPanel) gameOverPanel.SetActive(true);

        SoundManager.Instance?.StopBgm();

        Time.timeScale = 0f;
    }

    public IEnumerator DelayGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameOver();
    }

    public void QuitToMain()
    {
        isGameOver = false;

        if (player != null)
        {
            player.position = playerStartPos;
            player.rotation = Quaternion.identity;
            player.gameObject.SetActive(false);

            var pc = player.GetComponent<PlayerCollision>();
            if (pc != null)
            {
                pc.ResetScore();
                if (pc.scoreTextUI != null)
                    pc.scoreTextUI.gameObject.SetActive(true);
            }
        }

        if (playerUI != null)
        {
            playerUI.ResetUI();
            playerUI.Hide();
        }

        if (playerController != null)
            playerController.enabled = false;

        foreach (var fish in GameObject.FindGameObjectsWithTag("Fish"))
            Destroy(fish);

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (startMenuPanel) startMenuPanel.SetActive(true);

        if (playerUI.settingButton != null)
        {
            foreach (Transform child in playerUI.settingButton.transform)
                child.gameObject.SetActive(false);
        }

        Time.timeScale = 0f;
        SoundManager.Instance?.StopBgm();
    }

    public void PauseGame()
    {
        if (isGameOver) return;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (isGameOver) return;
        Time.timeScale = 1f;
    }
}