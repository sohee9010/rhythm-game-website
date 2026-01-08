using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // [FIX] Added TMPro namespace

public class GameUIBuilder : MonoBehaviour
{
    private GameObject pauseMenuPanel;
    private GameManager gameManager;

    void Start()
    {
        // [FIX] 로비/Main 씬에서는 게임 UI(타이머 등)를 생성하지 않음
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("Lobby") || sceneName.Contains("Main")) return;

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

        // [FIX] 0-1. Score Text (Top Right)
        // Created with specific styling requirements
         RectTransform scRt;
        // [FIX] SCORE UI WITH BACKGROUND BAR
        // Parent container with dark background
        GameObject scoreContainer = new GameObject("ScoreContainer");
        scoreContainer.transform.SetParent(parent, false);
        scRt = scoreContainer.AddComponent<RectTransform>();
        scRt.anchorMin = new Vector2(0.25f, 0.88f); // Centered Top
        scRt.anchorMax = new Vector2(0.75f, 0.98f);
        scRt.offsetMin = Vector2.zero; scRt.offsetMax = Vector2.zero;

        // Add dark background bar like result screen
        Image scoreBg = scoreContainer.AddComponent<Image>();
        
        // Use Unity's built-in white sprite
        scoreBg.sprite = Resources.Load<Sprite>("UI/Skin/Background");
        if (scoreBg.sprite == null)
        {
            // Fallback: create minimal sprite
            scoreBg.sprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f),
                1f
            );
        }
        
        scoreBg.type = Image.Type.Sliced; // Allow stretching
        scoreBg.color = new Color(0.1f, 0.1f, 0.15f, 0.9f); // Dark gray/black
        
        // Add outline for depth
        var scoreOutline = scoreContainer.AddComponent<UnityEngine.UI.Outline>();
        scoreOutline.effectColor = new Color(0.3f, 0.3f, 0.4f, 0.6f);
        scoreOutline.effectDistance = new Vector2(2, -2);

        // Child text object
        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.SetParent(scoreContainer.transform, false);
        RectTransform txtRt = scoreTextObj.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero; txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = Vector2.zero; txtRt.offsetMax = Vector2.zero;

        TextMeshProUGUI scoreTxt = scoreTextObj.AddComponent<TextMeshProUGUI>();
        scoreTxt.text = "SCORE 0";
        scoreTxt.fontSize = 90; // Much larger and bolder
        scoreTxt.fontStyle = FontStyles.Bold | FontStyles.Italic;
        scoreTxt.alignment = TextAlignmentOptions.Center;
        scoreTxt.outlineWidth = 0.2f; // Add outline for thickness
        scoreTxt.outlineColor = new Color(0, 0, 0, 0.8f); // Black outline
        
        // Base Gradient (Silver) - Will be used for Number
        scoreTxt.enableVertexGradient = true;
        scoreTxt.colorGradient = new VertexGradient(
            new Color(1f, 1f, 1f), 
            new Color(1f, 1f, 1f), 
            new Color(0.7f, 0.7f, 0.8f), 
            new Color(0.5f, 0.5f, 0.6f)
        );
        scoreTxt.color = Color.white;

        if (gameManager != null) {
            gameManager.scoreText = scoreTxt;
        }

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

        // 3. [NEW] In-Game Rank Gauge (HUD)
        CreateInGameRankGauge(parent);
    }

    void CreateInGameRankGauge(Transform parent)
    {
        // Container
        GameObject gaugeObj = new GameObject("InGameRankGauge");
        gaugeObj.transform.SetParent(parent, false);
        
        RectTransform rt = gaugeObj.AddComponent<RectTransform>();
        // Position: Bottom Right
        rt.anchorMin = new Vector2(1f, 0f); 
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(1f, 0f);
        rt.anchoredPosition = new Vector2(-50, 50); // Padding from corner
        rt.sizeDelta = new Vector2(300, 30); // Wider and visible

        // Background
        Image bg = gaugeObj.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Darker background

        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(gaugeObj.transform, false);
        Text lbl = labelObj.AddComponent<Text>();
        lbl.text = "RANK ACCURACY";
        lbl.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        lbl.fontSize = 18;
        lbl.color = Color.white;
        lbl.alignment = TextAnchor.MiddleRight;
        
        RectTransform lblRt = labelObj.GetComponent<RectTransform>();
        lblRt.anchorMin = new Vector2(0, 1); lblRt.anchorMax = new Vector2(1, 1);
        lblRt.pivot = new Vector2(0.5f, 0);
        lblRt.anchoredPosition = new Vector2(0, 5); // Above bar
        lblRt.sizeDelta = new Vector2(0, 30);

        // Fill Image
        GameObject fillObj = new GameObject("InGameRankGaugeFill"); 
        fillObj.transform.SetParent(gaugeObj.transform, false);
        
        Image fill = fillObj.AddComponent<Image>();
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.color = Color.gray; 
        
        RectTransform fillRt = fillObj.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero; fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero; fillRt.offsetMax = Vector2.zero;

        // [FIX] Explicitly link to GameManager
        if (gameManager != null)
        {
            gameManager.inGameRankGaugeFill = fill;
            Debug.Log("[GameUIBuilder] Assigned InGameRankGaugeFill to GameManager");
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.inGameRankGaugeFill = fill;
            Debug.Log("[GameUIBuilder] Assigned InGameRankGaugeFill to GameManager.Instance");
        }

        // Threshold Markers Helper
        void CreateHudMarker(float percent, string markLabel)
        {
            GameObject m = new GameObject("Marker_" + markLabel);
            m.transform.SetParent(gaugeObj.transform, false);
            
            Image img = m.AddComponent<Image>();
            img.color = new Color(1, 1, 1, 0.5f);
            
            RectTransform mrt = m.GetComponent<RectTransform>();
            mrt.anchorMin = new Vector2(percent, 0);
            mrt.anchorMax = new Vector2(percent, 1);
            mrt.sizeDelta = new Vector2(2, 0);
            mrt.anchoredPosition = Vector2.zero;
            
            // Marker Label (S, A, B, C)
            GameObject txt = new GameObject("Txt");
            txt.transform.SetParent(m.transform, false);
            Text t = txt.AddComponent<Text>();
            t.text = markLabel;
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.fontSize = 14;
            t.color = Color.yellow;
            t.alignment = TextAnchor.MiddleCenter;
            
            RectTransform trt = txt.GetComponent<RectTransform>();
            trt.anchoredPosition = new Vector2(0, -25); // Below marker
        }

        CreateHudMarker(0.50f, "C"); 
        CreateHudMarker(0.70f, "B");
        CreateHudMarker(0.85f, "A");
        CreateHudMarker(0.95f, "S");
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
