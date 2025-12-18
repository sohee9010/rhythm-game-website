using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(LobbyManager))]
[ExecuteAlways] // 에디터에서도 스크립트가 돌아가게 함
public class LobbyUIBuilder : MonoBehaviour
{
    [Header("Custom UI Images")]
    public Sprite borderSprite; // 게임 테두리 (필요없으면 비워두세요)
    public Sprite titleLogoSprite; // 타이틀 로고 이미지
    public Sprite startButtonSprite; // 게임 시작 버튼 이미지
    public Sprite quitButtonSprite; // 게임 종료 버튼 이미지

    [Header("UI Settings")]
    public Vector2 logoSize = new Vector2(1100, 1100); // 로고 크기
    public Vector2 startButtonSize = new Vector2(1000, 1000); // 시작 버튼 크기
    public Vector2 quitButtonSize = new Vector2(1000, 1000); // 종료 버튼 크기
    
    [Header("UI Positions")]
    public Vector2 logoPosition = Vector2.zero; // 로고 위치
    public Vector2 startButtonPosition = new Vector2(0, -170); // 시작 버튼 위치
    public Vector2 quitButtonPosition = new Vector2(0, -400); // 종료 버튼 위치

    private bool _isDirty = false;

    private void Start()
    {
        if (Application.isPlaying)
        {
            // NetworkManager 없으면 생성
            if (Object.FindFirstObjectByType<NetworkManager>() == null)
            {
                new GameObject("NetworkManager").AddComponent<NetworkManager>();
            }

            BuildUI();

            // 초기 상태: 메인 메뉴 (Start 버튼 표시)
            SetGameReady(true);

            // 이벤트 연결
            NetworkManager net = Object.FindFirstObjectByType<NetworkManager>();
            if (net != null)
            {
                net.OnConnected += OnClientConnected;
            }
        }
    }

    private void OnClientConnected()
    {
        // 연결되면 바로 시작하지 않고 카운트다운 시작
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            GameObject panel = FindChild(canvas.gameObject, "ConnectionPanel");
            if (panel != null && panel.activeSelf)
            {
                StartCoroutine(StartGameCountdown(panel));
            }
        }
    }

    private System.Collections.IEnumerator StartGameCountdown(GameObject panel)
    {
        GameObject card = FindChild(panel, "CardBackground");
        if (card == null) yield break;

        GameObject txtObj = FindChild(card, "InfoText");
        TextMeshProUGUI txt = (txtObj != null) ? txtObj.GetComponent<TextMeshProUGUI>() : null;
        
        // QR 코드 이미지는 숨기기 (깔끔하게)
        GameObject qrObj = FindChild(card, "QRCode");
        if (qrObj != null) qrObj.SetActive(false);

        float duration = 3.0f;
        while (duration > 0)
        {
            if (txt != null) 
                txt.text = $"Connected!\nStarting in {Mathf.CeilToInt(duration)}...";
            
            yield return null;
            duration -= Time.deltaTime;
        }

        if (txt != null) txt.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        GetComponent<LobbyManager>().StartGame();
    }

    public void OnStartButtonClicked()
    {
        NetworkManager net = NetworkManager.Instance;
        if (net != null && net.isConnected)
        {
            // 이미 연결되어 있으면 바로 시작
            GetComponent<LobbyManager>().StartGame();
        }
        else
        {
            // 연결 안 되어 있으면 QR 코드 패널 표시
            SetGameReady(false);
        }
    }

    private void SetGameReady(bool isReady)
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        GameObject startBtn = FindChild(canvas.gameObject, "StartButton");
        if (startBtn != null) startBtn.SetActive(isReady);

        GameObject panel = FindChild(canvas.gameObject, "ConnectionPanel");
        if (panel != null) panel.SetActive(!isReady);
    }

    private void OnValidate()
    {
        // 인스펙터에서 값이 바뀌면 갱신 예약
        _isDirty = true;
    }

    private void Update()
    {
        // 에디터 모드일 때만, 값이 바뀌었으면 UI 다시 그리기
        if (!Application.isPlaying && _isDirty)
        {
            _isDirty = false;
            BuildUI();
        }
    }

    [ContextMenu("Build Lobby UI")]
    public void BuildUI()
    {
        // 1. Canvas 찾기 또는 생성
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<GraphicRaycaster>();
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }

        // 1.5 EventSystem 찾기 또는 생성 (UI 클릭 필수요소)
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 2. 배경 (Panel)
        GameObject bgObj = FindChild(canvas.gameObject, "BackgroundPanel");
        if (bgObj == null)
        {
            bgObj = new GameObject("BackgroundPanel");
            bgObj.transform.SetParent(canvas.transform, false);
            Image img = bgObj.AddComponent<Image>();
            img.color = new Color(0.05f, 0.05f, 0.1f, 0.8f); 
            
            RectTransform rt = bgObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        bgObj.transform.SetAsFirstSibling();

        // [NEW] 게임 테두리 (Border)
        if (borderSprite != null)
        {
            GameObject borderObj = FindChild(canvas.gameObject, "GameBorder");
            if (borderObj == null)
            {
                borderObj = new GameObject("GameBorder");
                borderObj.transform.SetParent(canvas.transform, false);
                Image img = borderObj.AddComponent<Image>();
                img.sprite = borderSprite;
                img.raycastTarget = false; 
                
                RectTransform rt = borderObj.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
        }

        // [NEW] 3D 배경 생성
        Create3DBackground();

#if UNITY_EDITOR
        MakeTextureReadable(startButtonSprite);
        MakeTextureReadable(quitButtonSprite);
#endif

        // 3. 타이틀 (Text or Logo Image)
        GameObject titleObj = FindChild(canvas.gameObject, "TitleText");
        if (titleObj == null)
        {
            titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvas.transform, false);
            
            RectTransform rt = titleObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.7f); 
            rt.anchorMax = new Vector2(0.5f, 0.7f);
            rt.anchoredPosition = Vector2.zero;
        }

        if (titleLogoSprite != null)
        {
            TextMeshProUGUI oldTxt = titleObj.GetComponent<TextMeshProUGUI>();
            if (oldTxt != null) DestroyImmediate(oldTxt);
            
            Image img = titleObj.GetComponent<Image>();
            if (img == null) img = titleObj.AddComponent<Image>();
            
            img.sprite = titleLogoSprite;
            img.preserveAspect = true; 
            // 사이즈 설정 (사용자 지정)
            titleObj.GetComponent<RectTransform>().sizeDelta = logoSize; 
            titleObj.GetComponent<RectTransform>().anchoredPosition = logoPosition;
        }
        else
        {
            Image oldImg = titleObj.GetComponent<Image>();
            if (oldImg != null) DestroyImmediate(oldImg);

            TextMeshProUGUI txt = titleObj.GetComponent<TextMeshProUGUI>();
            if (txt == null) txt = titleObj.AddComponent<TextMeshProUGUI>();
            
            txt.text = "MOTION\nRHYTHM GAME"; 
            txt.fontSize = 80;
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = new Color(0f, 1f, 1f); 
            txt.fontStyle = FontStyles.Bold | FontStyles.Italic;
            txt.characterSpacing = 10;
            
            if (titleObj.GetComponent<Shadow>() == null)
            {
                Shadow shadow = titleObj.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0.5f, 0.5f, 0.5f);
                shadow.effectDistance = new Vector2(5, -5);
            }

            if (titleObj.GetComponent<Outline>() == null)
            {
                Outline outline = titleObj.AddComponent<Outline>();
                outline.effectColor = new Color(0f, 0.2f, 0.2f);
                outline.effectDistance = new Vector2(2, -2);
            }
            
            titleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1500, 300);
            titleObj.GetComponent<RectTransform>().anchoredPosition = logoPosition;
        }

        // [NEW] 연결 대기 화면 (QR 코드)
        CreateConnectionPanel(canvas.transform);

        // 4. 시작 버튼 (이미지 지원, 사이즈/위치 조절 가능)
        CreateButton(canvas.transform, "StartButton", "START GAME", startButtonPosition, new Color(0.2f, 0.2f, 0.2f), new Color(0f, 1f, 0.5f), startButtonSprite, startButtonSize, () => {
            OnStartButtonClicked();
        });

        // 5. 종료 버튼 (이미지 지원, 사이즈/위치 조절 가능)
        CreateButton(canvas.transform, "QuitButton", "EXIT", quitButtonPosition, new Color(0.2f, 0.2f, 0.2f), new Color(1f, 0.2f, 0.5f), quitButtonSprite, quitButtonSize, () => {
            GetComponent<LobbyManager>().QuitGame();
        });
        
        // 카메라 파랄랙스 효과
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            if (mainCam.GetComponent<LobbyCameraEffect>() == null)
            {
                mainCam.gameObject.AddComponent<LobbyCameraEffect>();
            }
        }

        Debug.Log("Lobby UI Built Successfully!");
    }

    private void CreateConnectionPanel(Transform parent)
    {
        GameObject panelObj = FindChild(parent.gameObject, "ConnectionPanel");
        if (panelObj == null)
        {
            panelObj = new GameObject("ConnectionPanel");
            panelObj.transform.SetParent(parent, false);
            
            // 1. 전체 화면 배경 (어두운 반투명)
            Image img = panelObj.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.9f);
            
            RectTransform rt = panelObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            // 2. 카드 배경 (중앙 정렬)
            GameObject cardObj = new GameObject("CardBackground");
            cardObj.transform.SetParent(panelObj.transform, false);
            Image cardImg = cardObj.AddComponent<Image>();
            cardImg.color = new Color(0.15f, 0.15f, 0.2f, 1f); // 다크 블루 그레이
            
            // 둥근 모서리 효과 (Outline 컴포넌트로 대체하거나 스프라이트 필요, 여기선 색상만)
            if (borderSprite != null) cardImg.sprite = borderSprite; // 테두리 스프라이트 재활용 가능하면 사용

            RectTransform cardRt = cardObj.GetComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(800, 900); // 카드 크기
            cardRt.anchoredPosition = Vector2.zero;

            // 3. QR 코드 이미지
            GameObject qrObj = new GameObject("QRCode");
            qrObj.transform.SetParent(cardObj.transform, false);
            RawImage qrImg = qrObj.AddComponent<RawImage>();
            
            Texture2D qrTex = Resources.Load<Texture2D>("qrcode");
            if (qrTex != null)
            {
                qrImg.texture = qrTex;
                qrObj.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 500);
            }
            else
            {
                qrImg.color = Color.white;
                qrObj.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 500);
            }
            qrObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);

            // 4. 안내 텍스트
            GameObject txtObj = new GameObject("InfoText");
            txtObj.transform.SetParent(cardObj.transform, false);
            TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
            txt.text = "Scan with your Phone";
            txt.fontSize = 50;
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = new Color(0.8f, 0.9f, 1f);
            txt.fontStyle = FontStyles.Bold;
            
            RectTransform txtRt = txtObj.GetComponent<RectTransform>();
            txtRt.anchoredPosition = new Vector2(0, 350);
            txtRt.sizeDelta = new Vector2(700, 100);

            // 5. 서브 텍스트
            GameObject subTxtObj = new GameObject("SubText");
            subTxtObj.transform.SetParent(cardObj.transform, false);
            TextMeshProUGUI subTxt = subTxtObj.AddComponent<TextMeshProUGUI>();
            subTxt.text = "Make sure both devices are on the\nSAME Wi-Fi Network";
            subTxt.fontSize = 30;
            subTxt.alignment = TextAlignmentOptions.Center;
            subTxt.color = new Color(0.6f, 0.6f, 0.7f);
            
            RectTransform subTxtRt = subTxtObj.GetComponent<RectTransform>();
            subTxtRt.anchoredPosition = new Vector2(0, -250);
            subTxtRt.sizeDelta = new Vector2(700, 100);

            // 6. 취소 버튼
            CreateButton(cardObj.transform, "CancelButton", "CANCEL", new Vector2(0, -380), new Color(0.3f, 0.3f, 0.3f), Color.white, null, new Vector2(300, 80), () => {
                SetGameReady(true); // 다시 메인으로
            });
        }
        else
        {
            // 이미 존재하면 QR 코드 이미지만 갱신 시도
            Transform cardTrans = panelObj.transform.Find("CardBackground");
            if (cardTrans != null)
            {
                Transform qrTrans = cardTrans.Find("QRCode");
                if (qrTrans != null)
                {
                    RawImage qrImg = qrTrans.GetComponent<RawImage>();
                    Texture2D qrTex = Resources.Load<Texture2D>("qrcode");
                    if (qrTex != null) qrImg.texture = qrTex;
                }
            }
        }
    }

    private void CreateButton(Transform parent, string name, string text, Vector2 position, Color bgColor, Color textColor, Sprite sprite, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = FindChild(parent.gameObject, name);
        if (btnObj == null)
        {
            btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            
            Image img = btnObj.AddComponent<Image>();
            
            // 이미지가 있으면 이미지 사용, 없으면 색상 사용
            if (sprite != null)
            {
                img.sprite = sprite;
                img.color = Color.white; // 이미지가 있으면 흰색(원본색)
                img.preserveAspect = true;
                img.alphaHitTestMinimumThreshold = 0.1f; // [FIX] 투명한 부분 클릭 무시
            }
            else
            {
                img.color = bgColor;
            }

            Button btn = btnObj.AddComponent<Button>();

            // 텍스트 (이미지가 없을 때만 생성)
            GameObject txtObj = FindChild(btnObj, "Text");
            if (sprite == null)
            {
                if (txtObj == null)
                {
                    txtObj = new GameObject("Text");
                    txtObj.transform.SetParent(btnObj.transform, false);
                    TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
                    txt.text = text;
                    txt.fontSize = 40; 
                    txt.alignment = TextAlignmentOptions.Center;
                    txt.color = textColor; 
                    txt.fontStyle = FontStyles.Bold | FontStyles.Italic;
                    
                    RectTransform txtRt = txtObj.GetComponent<RectTransform>();
                    txtRt.anchorMin = Vector2.zero;
                    txtRt.anchorMax = Vector2.one;
                    txtRt.offsetMin = Vector2.zero;
                    txtRt.offsetMax = Vector2.zero;
                }
            }
            else
            {
                // 이미지가 있는데 텍스트 오브젝트가 남아있으면 삭제 (깔끔하게)
                if (txtObj != null) DestroyImmediate(txtObj);
            }

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f); 
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size; // 사용자 지정 사이즈 적용
            rt.anchoredPosition = position;
            
            ColorBlock colors = btn.colors;
            if (sprite == null)
            {
                colors.normalColor = bgColor;
                colors.highlightedColor = bgColor + new Color(0.1f, 0.1f, 0.1f);
                colors.pressedColor = bgColor - new Color(0.1f, 0.1f, 0.1f);
                colors.selectedColor = bgColor;
            }
            else
            {
                colors.normalColor = Color.white;
                colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
                colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
                colors.selectedColor = Color.white;
            }
            btn.colors = colors;
        }
        else
        {
            // 이미 존재할 때 업데이트 로직
            Image img = btnObj.GetComponent<Image>();
            GameObject txtObj = FindChild(btnObj, "Text");

            if (sprite != null)
            {
                img.sprite = sprite;
                img.color = Color.white;
                img.preserveAspect = true;
                img.alphaHitTestMinimumThreshold = 0.1f; // [FIX] 투명한 부분 클릭 무시
                if (txtObj != null) DestroyImmediate(txtObj);
            }
            
            // 사이즈 업데이트
            RectTransform rt = btnObj.GetComponent<RectTransform>();
            if (rt != null) 
            {
                rt.sizeDelta = size;
                rt.anchoredPosition = position; // [FIX] 위치도 같이 업데이트
            }

            // 그림자/아웃라인 제거 (깔끔하게)
            Shadow shadow = btnObj.GetComponent<Shadow>();
            if (shadow != null) DestroyImmediate(shadow);
            Outline outline = btnObj.GetComponent<Outline>();
            if (outline != null) DestroyImmediate(outline);
            
            // 런타임 리스너 재연결
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                // [FIX] 버튼 색상 상태도 같이 업데이트해야 함
                ColorBlock colors = btn.colors;
                if (sprite != null)
                {
                    colors.normalColor = Color.white;
                    colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
                    colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
                    colors.selectedColor = Color.white;
                }
                else
                {
                    // 이미지가 없으면 배경색 사용 (기존 로직 유지 또는 업데이트)
                    // 여기서는 굳이 건드리지 않아도 되지만, 확실하게 하려면 업데이트
                }
                btn.colors = colors;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
            }
        }
        
        // 안전장치
        Button buttonComponent = btnObj.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(action);
        }
    }

    private void Create3DBackground()
    {
        // 기존에 생성된 3D 배경이 있다면 삭제 (학교 모델을 넣기 위해 비워둠)
        GameObject oldBg = GameObject.Find("Background3D");
        if (oldBg != null)
        {
            DestroyImmediate(oldBg);
        }
        
        // 여기에 school.fbx를 배치하시면 됩니다!
    }

    private GameObject FindChild(GameObject parent, string name)
    {
        Transform t = parent.transform.Find(name);
        if (t != null) return t.gameObject;
        return null;
    }

#if UNITY_EDITOR
    private void MakeTextureReadable(Sprite sprite)
    {
        if (sprite == null) return;
        try
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(sprite.texture);
            UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
            if (importer != null && !importer.isReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
                Debug.Log($"[LobbyUIBuilder] Automatically enabled Read/Write for {sprite.name}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to set Read/Write: {e.Message}");
        }
    }
#endif
}
