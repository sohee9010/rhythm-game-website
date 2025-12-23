using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIBuilder : MonoBehaviour
{
    private GameObject pauseMenuPanel;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
        
        // Canvas 생성 없으면 만들기
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("GameCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.sortingOrder = 100; // [FIX] 다른 UI보다 위에 오도록 강제 설정
        }

        // EventSystem 확인
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        CreateUI(canvas.transform);
    }

    void CreateUI(Transform parent)
    {
        // 0. 남은 시간 텍스트 (상단 중앙)
        CreateText(parent, "TimerText", "00:00", new Vector2(0, -50), 40, Color.white);

        // 1. 일시정지 버튼 (우측 상단 고정) -> [FIX] uGUI 버튼 클릭 문제로 인해 GameManager의 IMGUI 버튼으로 대체함
        /*
        CreateButton(parent, "PauseButton", "PAUSE", new Vector2(-100, -80), new Vector2(150, 60), () => {
            Debug.Log("[GameUIBuilder] Pause Button Clicked!");
            gameManager.PauseGame();
            pauseMenuPanel.SetActive(true);
        }, true); 
        */

        // 2. 일시정지 메뉴 패널 (초기엔 비활성화)
        CreatePauseMenu(parent);
    }
    
    // 텍스트 생성 헬퍼
    void CreateText(Transform parent, string name, string initialText, Vector2 anchoredPos, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        Text txt = textObj.AddComponent<Text>();
        txt.text = initialText;
        txt.text = initialText;
        // [FIX] Unity 최신 버전에서 Arial.ttf가 없을 수 있음 -> 기본 폰트 사용
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.font = font;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.yellow; // [FIX] 흰색 -> 노란색 (더 잘 보이게)
        txt.fontSize = 60; // [FIX] 40 -> 60 (더 크게)
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f); // 상단 중앙
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(200, 100);
    }

    void CreatePauseMenu(Transform parent)
    {
        pauseMenuPanel = new GameObject("PauseMenuPanel");
        pauseMenuPanel.transform.SetParent(parent, false);
        
        // 반투명 배경
        Image bg = pauseMenuPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.8f);
        RectTransform rect = bg.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        // 메뉴 컨테이너
        GameObject container = new GameObject("Container");
        container.transform.SetParent(pauseMenuPanel.transform, false);
        
        // Resume 버튼
        CreateButton(container.transform, "ResumeButton", "RESUME", new Vector2(0, 50), new Vector2(200, 60), () => {
            gameManager.ResumeGame();
            pauseMenuPanel.SetActive(false);
        });

        // Lobby 버튼
        CreateButton(container.transform, "LobbyButton", "RETURN TO LOBBY", new Vector2(0, -50), new Vector2(200, 60), () => {
            gameManager.ReturnToLobby();
        });

        pauseMenuPanel.SetActive(false);
    }

    // 버튼 생성 헬퍼
    GameObject CreateButton(Transform parent, string name, string text, Vector2 anchoredPos, Vector2 size, UnityEngine.Events.UnityAction onClick, bool isAnchorTopRight = false)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);

        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(onClick);

        RectTransform rect = btnObj.GetComponent<RectTransform>();
        if (isAnchorTopRight)
        {
            rect.anchorMin = Vector2.one; // (1, 1) 우측 상단
            rect.anchorMax = Vector2.one;
            rect.pivot = Vector2.one;
        }
        else
        {
            // 기본값: 중앙
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
        }
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        Text txt = textObj.AddComponent<Text>();
        txt.text = text;
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.font = font;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.fontSize = 24; // 글자 크기 조금 키움
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        // [FIX] 텍스트가 버튼 클릭을 가로채지 않도록 설정
        txt.raycastTarget = false;

        // [FIX] 버튼이 다른 UI 위에 오도록 맨 나중으로 순서 변경
        btnObj.transform.SetAsLastSibling();

        return btnObj;
    }
}
