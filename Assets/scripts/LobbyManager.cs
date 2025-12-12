using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "Game"; // 게임 씬 이름 (저장한 이름과 같아야 함)

    public void StartGame()
    {
        Debug.Log($"[LobbyManager] Trying to load scene: {gameSceneName}");
        // 게임 씬으로 이동
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        // 어플리케이션 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Game Quit"); 
    }
}
