using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "Game"; 

    private string _log = "";
    


    public void StartGame()
    {
        Time.timeScale = 1f; // 중요: 게임 시작 전 시간 속도 초기화
        _log = $"Attempting to load: {gameSceneName}";
        Debug.Log($"[LobbyManager] Request to load: {gameSceneName}");

        try
        {
            SceneManager.LoadScene(gameSceneName);
        }
        catch (System.Exception e)
        {
            _log = $"Error: {e.Message}";
            Debug.LogError($"[LobbyManager] LoadScene Failed: {e}");
        }
    }

    // [DEBUG] GUI 제거
    // void OnGUI() { ... }

    public void QuitGame()
    {
        // 어플리케이션 종료
        _log = "Quitting...";
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Game Quit"); 
    }
}
