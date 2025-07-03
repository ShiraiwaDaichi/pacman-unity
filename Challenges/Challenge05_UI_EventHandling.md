# 課題05: UI作成とイベント処理

## ?? 学習目標
- Unity UI システムの理解と実装
- Button、Slider、Toggle などのUI要素の使用
- イベント処理とコールバック関数
- データバインディングとUI更新システム
- レスポンシブUIデザインの基礎

## ? 推定所要時間
約 60-75 分

## ?? 前提知識
- 課題04の完了
- UI Canvas の基本概念
- イベント処理の理解

## ?? 課題内容

### ステップ1: 高度なUIシステムの設計

#### 1.1 Canvas構造の設計
1. Main Canvas → Screen Space - Overlay
2. 子キャンバス：
   - GameUI (ゲーム中の表示)
   - MenuUI (メニュー画面)
   - SettingsUI (設定画面)
   - HudUI (常時表示情報)

### ステップ2: UI管理スクリプトの作成

以下のスクリプトを **自分で入力** してください：

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 統合UI管理システム
public class UIManager : MonoBehaviour
{
    // ===============================
    // UI要素の参照
    // ===============================
    
    [Header("Canvas参照")]
    public Canvas gameUICanvas;        // ゲーム中UI
    public Canvas menuUICanvas;        // メニューUI
    public Canvas settingsUICanvas;    // 設定UI
    public Canvas hudUICanvas;         // HUD UI
    
    [Header("ゲーム中UI要素")]
    public Text scoreDisplay;          // スコア表示
    public Text livesDisplay;          // ライフ表示
    public Text timerDisplay;          // タイマー表示
    public Slider healthBar;           // ヘルスバー
    public Image powerModeIndicator;   // パワーモード表示
    
    [Header("メニューUI要素")]
    public Button startGameButton;     // ゲーム開始ボタン
    public Button settingsButton;      // 設定ボタン
    public Button quitButton;          // 終了ボタン
    public Text highScoreText;         // ハイスコア表示
    
    [Header("設定UI要素")]
    public Slider musicVolumeSlider;   // 音楽音量スライダー
    public Slider sfxVolumeSlider;     // 効果音音量スライダー
    public Toggle fullscreenToggle;    // フルスクリーン切り替え
    public Toggle showFPSToggle;       // FPS表示切り替え
    public Button backButton;          // 戻るボタン
    
    [Header("HUD要素")]
    public Text fpsDisplay;            // FPS表示
    public Text debugInfo;             // デバッグ情報
    public Image connectionStatus;     // 接続状態表示
    
    [Header("アニメーション設定")]
    public float fadeSpeed = 2.0f;     // フェード速度
    public float scaleAnimSpeed = 5.0f; // スケールアニメーション速度
    
    [Header("色設定")]
    public Color normalButtonColor = Color.white;
    public Color hoverButtonColor = Color.yellow;
    public Color pressedButtonColor = Color.red;
    public Color disabledButtonColor = Color.gray;
    
    // ===============================
    // プライベート変数
    // ===============================
    
    private Dictionary<string, GameObject> uiPanels;  // UI パネル管理
    private float currentMusicVolume = 1.0f;
    private float currentSfxVolume = 1.0f;
    private bool showFPS = false;
    private float fps = 0f;
    private float frameTime = 0f;
    
    // アニメーション用
    private bool isAnimating = false;
    private Vector3 originalScale;
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    void Start()
    {
        InitializeUI();
        SetupEventListeners();
        LoadUISettings();
        
        Debug.Log("=== UI管理システム開始 ===");
    }
    
    void Update()
    {
        UpdateFPS();
        UpdateGameUI();
        UpdateAnimations();
        HandleKeyboardInput();
    }
    
    // ===============================
    // 初期化メソッド
    // ===============================
    
    void InitializeUI()
    {
        // UI パネル辞書の初期化
        uiPanels = new Dictionary<string, GameObject>
        {
            {"Game", gameUICanvas?.gameObject},
            {"Menu", menuUICanvas?.gameObject},
            {"Settings", settingsUICanvas?.gameObject},
            {"HUD", hudUICanvas?.gameObject}
        };
        
        // 初期状態の設定
        ShowPanel("Menu");
        
        // 初期値の設定
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = 100;
            healthBar.value = 100;
        }
        
        // FPS表示の初期設定
        if (fpsDisplay != null)
        {
            fpsDisplay.gameObject.SetActive(showFPS);
        }
    }
    
    void SetupEventListeners()
    {
        // ボタンイベントの設定
        SetupButton(startGameButton, StartGameClicked);
        SetupButton(settingsButton, SettingsClicked);
        SetupButton(quitButton, QuitClicked);
        SetupButton(backButton, BackClicked);
        
        // スライダーイベントの設定
        SetupSlider(musicVolumeSlider, OnMusicVolumeChanged);
        SetupSlider(sfxVolumeSlider, OnSfxVolumeChanged);
        
        // トグルイベントの設定
        SetupToggle(fullscreenToggle, OnFullscreenToggled);
        SetupToggle(showFPSToggle, OnShowFPSToggled);
        
        // ゲーム状態イベントを購読
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnStateChanged += OnGameStateChanged;
            GameStateManager.OnScoreChanged += OnScoreChanged;
            GameStateManager.OnLivesChanged += OnLivesChanged;
        }
    }
    
    // ===============================
    // UI要素セットアップヘルパー
    // ===============================
    
    void SetupButton(Button button, System.Action callback)
    {
        if (button != null)
        {
            button.onClick.AddListener(() => callback?.Invoke());
            
            // ホバーエフェクトの追加
            AddButtonHoverEffect(button);
        }
    }
    
    void SetupSlider(Slider slider, System.Action<float> callback)
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(callback.Invoke);
        }
    }
    
    void SetupToggle(Toggle toggle, System.Action<bool> callback)
    {
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(callback.Invoke);
        }
    }
    
    void AddButtonHoverEffect(Button button)
    {
        // EventTrigger を使用したホバーエフェクト
        var eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // マウスエンター
        var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => { OnButtonHover(button, true); });
        eventTrigger.triggers.Add(pointerEnter);
        
        // マウス離脱
        var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => { OnButtonHover(button, false); });
        eventTrigger.triggers.Add(pointerExit);
    }
    
    // ===============================
    // パネル管理メソッド
    // ===============================
    
    public void ShowPanel(string panelName)
    {
        foreach (var panel in uiPanels)
        {
            if (panel.Value != null)
            {
                bool shouldShow = panel.Key == panelName;
                panel.Value.SetActive(shouldShow);
                
                if (shouldShow)
                {
                    Debug.Log($"パネル表示: {panelName}");
                    AnimatePanelEntry(panel.Value);
                }
            }
        }
    }
    
    public void HidePanel(string panelName)
    {
        if (uiPanels.ContainsKey(panelName) && uiPanels[panelName] != null)
        {
            uiPanels[panelName].SetActive(false);
            Debug.Log($"パネル非表示: {panelName}");
        }
    }
    
    public void TogglePanel(string panelName)
    {
        if (uiPanels.ContainsKey(panelName) && uiPanels[panelName] != null)
        {
            bool isActive = uiPanels[panelName].activeSelf;
            uiPanels[panelName].SetActive(!isActive);
            
            if (!isActive)
            {
                AnimatePanelEntry(uiPanels[panelName]);
            }
        }
    }
    
    // ===============================
    // ボタンイベント処理
    // ===============================
    
    void StartGameClicked()
    {
        Debug.Log("ゲーム開始ボタンクリック");
        PlayButtonSound();
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.StartGame();
        }
        
        ShowPanel("Game");
    }
    
    void SettingsClicked()
    {
        Debug.Log("設定ボタンクリック");
        PlayButtonSound();
        ShowPanel("Settings");
    }
    
    void QuitClicked()
    {
        Debug.Log("終了ボタンクリック");
        PlayButtonSound();
        
        // 確認ダイアログを表示（実装例）
        if (ShowConfirmDialog("ゲームを終了しますか？"))
        {
            Application.Quit();
        }
    }
    
    void BackClicked()
    {
        Debug.Log("戻るボタンクリック");
        PlayButtonSound();
        ShowPanel("Menu");
    }
    
    // ===============================
    // スライダーイベント処理
    // ===============================
    
    void OnMusicVolumeChanged(float value)
    {
        currentMusicVolume = value;
        Debug.Log($"音楽音量変更: {value:F2}");
        
        // AudioManager がある場合の処理
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
        
        // 設定を保存
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    
    void OnSfxVolumeChanged(float value)
    {
        currentSfxVolume = value;
        Debug.Log($"効果音音量変更: {value:F2}");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
        
        PlayerPrefs.SetFloat("SfxVolume", value);
    }
    
    // ===============================
    // トグルイベント処理
    // ===============================
    
    void OnFullscreenToggled(bool isFullscreen)
    {
        Debug.Log($"フルスクリーン切り替え: {isFullscreen}");
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
    
    void OnShowFPSToggled(bool showFPSValue)
    {
        showFPS = showFPSValue;
        Debug.Log($"FPS表示切り替え: {showFPS}");
        
        if (fpsDisplay != null)
        {
            fpsDisplay.gameObject.SetActive(showFPS);
        }
        
        PlayerPrefs.SetInt("ShowFPS", showFPS ? 1 : 0);
    }
    
    // ===============================
    // ゲーム状態連携
    // ===============================
    
    void OnGameStateChanged(GameStateManager.GameState newState)
    {
        Debug.Log($"UI: 状態変更検知 {newState}");
        
        switch (newState)
        {
            case GameStateManager.GameState.MainMenu:
                ShowPanel("Menu");
                break;
            case GameStateManager.GameState.Playing:
                ShowPanel("Game");
                break;
            case GameStateManager.GameState.Paused:
                // ポーズメニューの表示
                break;
            case GameStateManager.GameState.GameOver:
            case GameStateManager.GameState.Victory:
                ShowPanel("Menu");
                break;
        }
    }
    
    void OnScoreChanged(int newScore)
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.text = $"Score: {newScore:N0}";
            
            // スコア変更時のアニメーション
            AnimateScoreChange();
        }
    }
    
    void OnLivesChanged(int newLives)
    {
        if (livesDisplay != null)
        {
            livesDisplay.text = $"Lives: {newLives}";
            
            // ライフが減った時の警告表示
            if (newLives <= 1)
            {
                livesDisplay.color = Color.red;
            }
            else
            {
                livesDisplay.color = Color.white;
            }
        }
    }
    
    // ===============================
    // UI更新メソッド
    // ===============================
    
    void UpdateGameUI()
    {
        if (GameStateManager.Instance == null) return;
        
        // タイマー更新
        if (timerDisplay != null)
        {
            float time = GameStateManager.Instance.GetRemainingTime();
            if (time >= 0)
            {
                timerDisplay.text = $"Time: {time:F1}s";
                
                // 残り時間が少ない時の警告表示
                if (time <= 10f)
                {
                    timerDisplay.color = Color.red;
                }
                else
                {
                    timerDisplay.color = Color.white;
                }
            }
        }
        
        // ハイスコア更新
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {GameStateManager.Instance.GetHighScore():N0}";
        }
    }
    
    void UpdateFPS()
    {
        if (showFPS && fpsDisplay != null)
        {
            frameTime += (Time.unscaledDeltaTime - frameTime) * 0.1f;
            fps = 1.0f / frameTime;
            fpsDisplay.text = $"FPS: {fps:F1}";
        }
    }
    
    // ===============================
    // アニメーション処理
    // ===============================
    
    void UpdateAnimations()
    {
        // パワーモードインジケーターの点滅
        if (powerModeIndicator != null)
        {
            bool isPowerMode = false; // GameManagerから取得
            if (isPowerMode)
            {
                float alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * 5f);
                Color color = powerModeIndicator.color;
                color.a = alpha;
                powerModeIndicator.color = color;
            }
        }
    }
    
    void AnimatePanelEntry(GameObject panel)
    {
        if (panel != null && !isAnimating)
        {
            StartCoroutine(AnimatePanelEntryCoroutine(panel));
        }
    }
    
    System.Collections.IEnumerator AnimatePanelEntryCoroutine(GameObject panel)
    {
        isAnimating = true;
        originalScale = panel.transform.localScale;
        panel.transform.localScale = Vector3.zero;
        
        float elapsed = 0f;
        while (elapsed < 1f / scaleAnimSpeed)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed * scaleAnimSpeed;
            panel.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, progress);
            yield return null;
        }
        
        panel.transform.localScale = originalScale;
        isAnimating = false;
    }
    
    void AnimateScoreChange()
    {
        if (scoreDisplay != null)
        {
            StartCoroutine(ScoreChangeAnimation());
        }
    }
    
    System.Collections.IEnumerator ScoreChangeAnimation()
    {
        Vector3 originalScale = scoreDisplay.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        
        // 拡大
        float elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.unscaledDeltaTime;
            scoreDisplay.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / 0.1f);
            yield return null;
        }
        
        // 縮小
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.unscaledDeltaTime;
            scoreDisplay.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / 0.1f);
            yield return null;
        }
        
        scoreDisplay.transform.localScale = originalScale;
    }
    
    // ===============================
    // ユーティリティメソッド
    // ===============================
    
    void OnButtonHover(Button button, bool isHovering)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = isHovering ? hoverButtonColor : normalButtonColor;
        }
    }
    
    void PlayButtonSound()
    {
        // ボタンクリック音の再生
        if (AudioManager.Instance != null)
        {
            // AudioManager.Instance.PlayButtonSound();
        }
    }
    
    bool ShowConfirmDialog(string message)
    {
        // 簡単な確認ダイアログ（実際のプロジェクトではより洗練されたものを使用）
        return true; // 常にtrue（実装例のため）
    }
    
    void LoadUISettings()
    {
        // 保存された設定を読み込み
        currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        currentSfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1.0f);
        showFPS = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        
        // UI要素に反映
        if (musicVolumeSlider != null) musicVolumeSlider.value = currentMusicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = currentSfxVolume;
        if (showFPSToggle != null) showFPSToggle.isOn = showFPS;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;
        
        Screen.fullScreen = fullscreen;
    }
    
    void HandleKeyboardInput()
    {
        // キーボードショートカット
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel("HUD");
        }
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            OnShowFPSToggled(!showFPS);
            if (showFPSToggle != null) showFPSToggle.isOn = showFPS;
        }
    }
    
    // ===============================
    // パブリックメソッド
    // ===============================
    
    public void UpdateHealthBar(float healthPercent)
    {
        if (healthBar != null)
        {
            healthBar.value = healthPercent;
            
            // ヘルス低下時の色変更
            Image fill = healthBar.fillRect?.GetComponent<Image>();
            if (fill != null)
            {
                if (healthPercent > 0.6f)
                    fill.color = Color.green;
                else if (healthPercent > 0.3f)
                    fill.color = Color.yellow;
                else
                    fill.color = Color.red;
            }
        }
    }
    
    public void ShowNotification(string message, float duration = 3f)
    {
        Debug.Log($"通知: {message}");
        // 実際の通知表示システムをここに実装
    }
    
    public void SetButtonEnabled(string buttonName, bool enabled)
    {
        // 特定のボタンの有効/無効を切り替え
        Button button = null;
        switch (buttonName)
        {
            case "Start": button = startGameButton; break;
            case "Settings": button = settingsButton; break;
            case "Quit": button = quitButton; break;
        }
        
        if (button != null)
        {
            button.interactable = enabled;
            button.GetComponent<Image>().color = enabled ? normalButtonColor : disabledButtonColor;
        }
    }
}
```

### ステップ3: カスタムUI要素の作成

```csharp
using UnityEngine;
using UnityEngine.UI;

// カスタムプログレスバー
public class CustomProgressBar : MonoBehaviour
{
    [Header("プログレスバー設定")]
    public Image fillImage;
    public Text progressText;
    public bool showPercentage = true;
    public bool animateChanges = true;
    public float animationSpeed = 2f;
    
    [Header("色設定")]
    public Gradient progressGradient;
    
    private float currentValue = 0f;
    private float targetValue = 0f;
    private float maxValue = 100f;
    
    void Update()
    {
        if (animateChanges)
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, animationSpeed * Time.deltaTime);
        }
        else
        {
            currentValue = targetValue;
        }
        
        UpdateVisuals();
    }
    
    public void SetValue(float value)
    {
        targetValue = Mathf.Clamp(value, 0f, maxValue);
    }
    
    public void SetMaxValue(float max)
    {
        maxValue = max;
        targetValue = Mathf.Clamp(targetValue, 0f, maxValue);
    }
    
    void UpdateVisuals()
    {
        float fillAmount = currentValue / maxValue;
        
        if (fillImage != null)
        {
            fillImage.fillAmount = fillAmount;
            fillImage.color = progressGradient.Evaluate(fillAmount);
        }
        
        if (progressText != null)
        {
            if (showPercentage)
            {
                progressText.text = $"{(fillAmount * 100):F0}%";
            }
            else
            {
                progressText.text = $"{currentValue:F0}/{maxValue:F0}";
            }
        }
    }
}
```

### ステップ4: UI要素の配置とテスト

1. Canvas に UI要素を配置
2. UIManager スクリプトをアタッチ
3. Inspector で参照を設定
4. ゲームを実行してテスト

## ?? 実験してみよう

### 実験1: 動的UI生成

```csharp
public void CreateDynamicButton(string text, System.Action callback)
{
    GameObject buttonGO = Instantiate(buttonPrefab, parentPanel);
    Button button = buttonGO.GetComponent<Button>();
    Text buttonText = buttonGO.GetComponentInChildren<Text>();
    
    buttonText.text = text;
    button.onClick.AddListener(() => callback?.Invoke());
}
```

### 実験2: UI データバインディング

```csharp
public class DataBinding : MonoBehaviour
{
    public Text targetText;
    public string propertyName;
    
    void Update()
    {
        if (GameStateManager.Instance != null)
        {
            var value = GetPropertyValue(GameStateManager.Instance, propertyName);
            targetText.text = value?.ToString() ?? "";
        }
    }
    
    object GetPropertyValue(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }
}
```

### 実験3: アクセシビリティ対応

```csharp
public class AccessibilityUI : MonoBehaviour
{
    public KeyCode shortcutKey;
    public Button targetButton;
    
    void Update()
    {
        if (Input.GetKeyDown(shortcutKey) && targetButton.interactable)
        {
            targetButton.onClick.Invoke();
        }
    }
}
```

## ? よくあるエラー

### エラー1: UI要素が反応しない
**原因**: Canvas の Graphic Raycaster 設定
**解決方法**: Canvas に Graphic Raycaster があるか確認

### エラー2: 参照エラー
**原因**: Inspector での参照設定忘れ
**解決方法**: null チェックの追加と適切な参照設定

### エラー3: UI が画面からはみ出る
**原因**: Anchor 設定の間違い
**解決方法**: Rect Transform の Anchor を適切に設定

## ?? 学習ポイント

### 重要な概念
1. **Canvas System**: UI描画システム
2. **Event System**: UI イベント処理
3. **Layout Groups**: 自動レイアウト
4. **Anchoring**: 画面サイズ対応
5. **Data Binding**: データとUIの連携

### UIベストプラクティス
- **Responsive Design**: 異なる画面サイズへの対応
- **Accessibility**: アクセシビリティの考慮
- **Performance**: UI パフォーマンスの最適化
- **User Experience**: ユーザビリティの向上

## ?? 次のステップ

この課題完了後：
1. **理解度チェック**: UI システムの動作説明
2. **実験**: カスタムUI要素の作成
3. **次の課題**: 課題06「音響システムの実装」

## ?? 追加チャレンジ

### チャレンジ1: 設定保存システム
```csharp
[System.Serializable]
public class UISettings
{
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public bool fullscreen = true;
    public bool showFPS = false;
}

public void SaveSettings()
{
    string json = JsonUtility.ToJson(uiSettings);
    PlayerPrefs.SetString("UISettings", json);
}
```

### チャレンジ2: ローカライゼーション
```csharp
public class LocalizationManager : MonoBehaviour
{
    public enum Language { English, Japanese, Korean }
    public Language currentLanguage = Language.English;
    
    private Dictionary<string, Dictionary<Language, string>> textDatabase;
    
    public string GetLocalizedText(string key)
    {
        if (textDatabase.ContainsKey(key))
        {
            return textDatabase[key][currentLanguage];
        }
        return key;
    }
}
```

### チャレンジ3: UI モジュール化
```csharp
public abstract class UIPanel : MonoBehaviour
{
    public abstract void Show();
    public abstract void Hide();
    public virtual void Initialize() { }
    public virtual void UpdatePanel() { }
}

public class MenuPanel : UIPanel
{
    public override void Show()
    {
        gameObject.SetActive(true);
        // アニメーション処理
    }
    
    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
```

優秀です！?? 
総合UI管理システムを完成させました！