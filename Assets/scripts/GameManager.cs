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
    public Text timeText; // 일반 Text (GameUIBuilder 생성)

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip gameMusic;

    private void Awake()
    {
        Instance = this;

        // UI 자동 연결
        if (scoreText == null) scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        if (comboText == null) comboText = GameObject.Find("ComboText")?.GetComponent<TextMeshProUGUI>();
        if (gameOverPanel == null) gameOverPanel = GameObject.Find("GameOverPanel");
        if (finalScoreText == null) finalScoreText = GameObject.Find("FinalScoreText")?.GetComponent<TextMeshProUGUI>();
        // timeText는 Start에서 GameUIBuilder가 생성 후 연결되거나 직접 찾음
        
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
        
        // UI 빌더 자동 추가
        if (GetComponent<GameUIBuilder>() == null)
        {
            gameObject.AddComponent<GameUIBuilder>();
        }

        // AudioSource 자동 찾기 (If null)
        if (musicSource == null) musicSource = GetComponent<AudioSource>();
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

        // [FIX] 오디오 클립이 없으면 로드 시도
        if (gameMusic == null)
        {
            // 1. AudioSource에 이미 할당되어 있는지 확인
            if (musicSource.clip != null)
            {
                gameMusic = musicSource.clip;
            }
            // 2. Resources 폴더에서 로드 시도 (파일명이 정확해야 함)
            else
            {
                gameMusic = Resources.Load<AudioClip>("Galaxias_ Shor ver.");
                // 만약 실패하면 다른 이름으로도 시도
                if (gameMusic == null) gameMusic = Resources.Load<AudioClip>("Music/Galaxias_ Shor ver.");
            }
        }

        // TimerText 찾기 (UIBuilder가 만든 후)
        if (timeText == null)
        {
            GameObject timerObj = GameObject.Find("TimerText");
            if (timerObj != null) timeText = timerObj.GetComponent<Text>();
        }

        // 테스트를 위해 바로 시작!
        StartGame(); 
        
        // NoteSpawner도 시작! (경고 수정됨)
        NoteSpawner spawner = Object.FindFirstObjectByType<NoteSpawner>();
        if (spawner != null) spawner.StartSpawning();
    }

    private void Update()
    {
        // 디버그용: P 키로 일시정지 테스트
        // 디버그용: P 키 또는 ESC 키로 일시정지
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        if (isPlaying && !isPaused && musicSource != null && musicSource.clip != null)
        {
            // 남은 시간 계산
            float remainingTime = musicSource.clip.length - musicSource.time;
            if (remainingTime < 0) remainingTime = 0;

            // UI 업데이트
            if (timeText == null)
            {
                // 늦게 생성될 수 있으므로 다시 찾기
                GameObject timerObj = GameObject.Find("TimerText");
                if (timerObj != null) timeText = timerObj.GetComponent<Text>();
            }

            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60F);
                int seconds = Mathf.FloorToInt(remainingTime - minutes * 60);
                timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        isPaused = false;
        Time.timeScale = 1f; // 시간 초기화

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

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // 게임 시간 정지
        if (musicSource != null) musicSource.Pause();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // 게임 시간 재개
        if (musicSource != null) musicSource.UnPause();
    }

    public void ReturnToLobby()
    {
        Time.timeScale = 1f; // 씬 이동 전 시간 정상화
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
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

    void OnGUI()
    {
        GUI.color = Color.yellow;
        string musicStatus = musicSource != null ? (musicSource.isPlaying ? "Playing" : "Stopped") : "Null";
        string clipStatus = (musicSource != null && musicSource.clip != null) ? musicSource.clip.name : "Null";
        
        string debugInfo = $"isPlaying: {isPlaying}\n" +
                           $"isPaused: {isPaused}\n" +
                           $"Time.timeScale: {Time.timeScale}\n" +
                           $"Music: {musicStatus} / Clip: {clipStatus}\n" +
                           $"Music Time: {(musicSource != null ? musicSource.time.ToString("F1") : "-")}";

        GUI.Label(new Rect(10, 200, 500, 200), debugInfo);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}