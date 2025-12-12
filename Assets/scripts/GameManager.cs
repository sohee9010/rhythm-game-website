using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 게임 전체 흐름을 관리하는 메인 매니저
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool isPlaying = false;
    public bool isPaused = false;

    [Header("Score")]
    public int score = 0;
    public int combo = 0;
    public int maxCombo = 0;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip gameMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // UI 자동 연결
        if (scoreText == null) scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        if (comboText == null) comboText = GameObject.Find("ComboText")?.GetComponent<TextMeshProUGUI>();
        if (gameOverPanel == null) gameOverPanel = GameObject.Find("GameOverPanel");
        if (finalScoreText == null) finalScoreText = GameObject.Find("FinalScoreText")?.GetComponent<TextMeshProUGUI>();

        // NetworkManager 자동 생성 (경고 수정됨)
        if (Object.FindFirstObjectByType<NetworkManager>() == null)
        {
            GameObject netObj = new GameObject("NetworkManager");
            netObj.AddComponent<NetworkManager>();
        }
    }

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateUI();
        
        // 테스트를 위해 바로 시작!
        StartGame(); 
        
        // NoteSpawner도 시작! (경고 수정됨)
        NoteSpawner spawner = Object.FindFirstObjectByType<NoteSpawner>();
        if (spawner != null) spawner.StartSpawning();
    }

    public void StartGame()
    {
        isPlaying = true;
        isPaused = false;
        score = 0;
        combo = 0;
        maxCombo = 0;

        if (musicSource != null && gameMusic != null)
        {
            musicSource.clip = gameMusic;
            musicSource.Play();
        }
        UpdateUI();
    }

    public void EndGame()
    {
        isPlaying = false;
        if (musicSource != null) musicSource.Stop();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = $"Score: {score}\nMax Combo: {maxCombo}";
        }
    }

    public void AddScore(int points)
    {
        score += points;
        combo++;
        if (combo > maxCombo) maxCombo = combo;
        UpdateUI();
    }

    public void ResetCombo()
    {
        combo = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (comboText != null)
        {
            if (combo > 0)
            {
                comboText.text = $"Combo: {combo}";
                comboText.gameObject.SetActive(true);
            }
            else comboText.gameObject.SetActive(false);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}