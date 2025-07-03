# �ۑ�05: UI�쐬�ƃC�x���g����

## ?? �w�K�ڕW
- Unity UI �V�X�e���̗����Ǝ���
- Button�ASlider�AToggle �Ȃǂ�UI�v�f�̎g�p
- �C�x���g�����ƃR�[���o�b�N�֐�
- �f�[�^�o�C���f�B���O��UI�X�V�V�X�e��
- ���X�|���V�uUI�f�U�C���̊�b

## ? ���菊�v����
�� 60-75 ��

## ?? �O��m��
- �ۑ�04�̊���
- UI Canvas �̊�{�T�O
- �C�x���g�����̗���

## ?? �ۑ���e

### �X�e�b�v1: ���x��UI�V�X�e���̐݌v

#### 1.1 Canvas�\���̐݌v
1. Main Canvas �� Screen Space - Overlay
2. �q�L�����o�X�F
   - GameUI (�Q�[�����̕\��)
   - MenuUI (���j���[���)
   - SettingsUI (�ݒ���)
   - HudUI (�펞�\�����)

### �X�e�b�v2: UI�Ǘ��X�N���v�g�̍쐬

�ȉ��̃X�N���v�g�� **�����œ���** ���Ă��������F

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// ����UI�Ǘ��V�X�e��
public class UIManager : MonoBehaviour
{
    // ===============================
    // UI�v�f�̎Q��
    // ===============================
    
    [Header("Canvas�Q��")]
    public Canvas gameUICanvas;        // �Q�[����UI
    public Canvas menuUICanvas;        // ���j���[UI
    public Canvas settingsUICanvas;    // �ݒ�UI
    public Canvas hudUICanvas;         // HUD UI
    
    [Header("�Q�[����UI�v�f")]
    public Text scoreDisplay;          // �X�R�A�\��
    public Text livesDisplay;          // ���C�t�\��
    public Text timerDisplay;          // �^�C�}�[�\��
    public Slider healthBar;           // �w���X�o�[
    public Image powerModeIndicator;   // �p���[���[�h�\��
    
    [Header("���j���[UI�v�f")]
    public Button startGameButton;     // �Q�[���J�n�{�^��
    public Button settingsButton;      // �ݒ�{�^��
    public Button quitButton;          // �I���{�^��
    public Text highScoreText;         // �n�C�X�R�A�\��
    
    [Header("�ݒ�UI�v�f")]
    public Slider musicVolumeSlider;   // ���y���ʃX���C�_�[
    public Slider sfxVolumeSlider;     // ���ʉ����ʃX���C�_�[
    public Toggle fullscreenToggle;    // �t���X�N���[���؂�ւ�
    public Toggle showFPSToggle;       // FPS�\���؂�ւ�
    public Button backButton;          // �߂�{�^��
    
    [Header("HUD�v�f")]
    public Text fpsDisplay;            // FPS�\��
    public Text debugInfo;             // �f�o�b�O���
    public Image connectionStatus;     // �ڑ���ԕ\��
    
    [Header("�A�j���[�V�����ݒ�")]
    public float fadeSpeed = 2.0f;     // �t�F�[�h���x
    public float scaleAnimSpeed = 5.0f; // �X�P�[���A�j���[�V�������x
    
    [Header("�F�ݒ�")]
    public Color normalButtonColor = Color.white;
    public Color hoverButtonColor = Color.yellow;
    public Color pressedButtonColor = Color.red;
    public Color disabledButtonColor = Color.gray;
    
    // ===============================
    // �v���C�x�[�g�ϐ�
    // ===============================
    
    private Dictionary<string, GameObject> uiPanels;  // UI �p�l���Ǘ�
    private float currentMusicVolume = 1.0f;
    private float currentSfxVolume = 1.0f;
    private bool showFPS = false;
    private float fps = 0f;
    private float frameTime = 0f;
    
    // �A�j���[�V�����p
    private bool isAnimating = false;
    private Vector3 originalScale;
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    void Start()
    {
        InitializeUI();
        SetupEventListeners();
        LoadUISettings();
        
        Debug.Log("=== UI�Ǘ��V�X�e���J�n ===");
    }
    
    void Update()
    {
        UpdateFPS();
        UpdateGameUI();
        UpdateAnimations();
        HandleKeyboardInput();
    }
    
    // ===============================
    // ���������\�b�h
    // ===============================
    
    void InitializeUI()
    {
        // UI �p�l�������̏�����
        uiPanels = new Dictionary<string, GameObject>
        {
            {"Game", gameUICanvas?.gameObject},
            {"Menu", menuUICanvas?.gameObject},
            {"Settings", settingsUICanvas?.gameObject},
            {"HUD", hudUICanvas?.gameObject}
        };
        
        // ������Ԃ̐ݒ�
        ShowPanel("Menu");
        
        // �����l�̐ݒ�
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = 100;
            healthBar.value = 100;
        }
        
        // FPS�\���̏����ݒ�
        if (fpsDisplay != null)
        {
            fpsDisplay.gameObject.SetActive(showFPS);
        }
    }
    
    void SetupEventListeners()
    {
        // �{�^���C�x���g�̐ݒ�
        SetupButton(startGameButton, StartGameClicked);
        SetupButton(settingsButton, SettingsClicked);
        SetupButton(quitButton, QuitClicked);
        SetupButton(backButton, BackClicked);
        
        // �X���C�_�[�C�x���g�̐ݒ�
        SetupSlider(musicVolumeSlider, OnMusicVolumeChanged);
        SetupSlider(sfxVolumeSlider, OnSfxVolumeChanged);
        
        // �g�O���C�x���g�̐ݒ�
        SetupToggle(fullscreenToggle, OnFullscreenToggled);
        SetupToggle(showFPSToggle, OnShowFPSToggled);
        
        // �Q�[����ԃC�x���g���w��
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnStateChanged += OnGameStateChanged;
            GameStateManager.OnScoreChanged += OnScoreChanged;
            GameStateManager.OnLivesChanged += OnLivesChanged;
        }
    }
    
    // ===============================
    // UI�v�f�Z�b�g�A�b�v�w���p�[
    // ===============================
    
    void SetupButton(Button button, System.Action callback)
    {
        if (button != null)
        {
            button.onClick.AddListener(() => callback?.Invoke());
            
            // �z�o�[�G�t�F�N�g�̒ǉ�
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
        // EventTrigger ���g�p�����z�o�[�G�t�F�N�g
        var eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // �}�E�X�G���^�[
        var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => { OnButtonHover(button, true); });
        eventTrigger.triggers.Add(pointerEnter);
        
        // �}�E�X���E
        var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => { OnButtonHover(button, false); });
        eventTrigger.triggers.Add(pointerExit);
    }
    
    // ===============================
    // �p�l���Ǘ����\�b�h
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
                    Debug.Log($"�p�l���\��: {panelName}");
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
            Debug.Log($"�p�l����\��: {panelName}");
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
    // �{�^���C�x���g����
    // ===============================
    
    void StartGameClicked()
    {
        Debug.Log("�Q�[���J�n�{�^���N���b�N");
        PlayButtonSound();
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.StartGame();
        }
        
        ShowPanel("Game");
    }
    
    void SettingsClicked()
    {
        Debug.Log("�ݒ�{�^���N���b�N");
        PlayButtonSound();
        ShowPanel("Settings");
    }
    
    void QuitClicked()
    {
        Debug.Log("�I���{�^���N���b�N");
        PlayButtonSound();
        
        // �m�F�_�C�A���O��\���i������j
        if (ShowConfirmDialog("�Q�[�����I�����܂����H"))
        {
            Application.Quit();
        }
    }
    
    void BackClicked()
    {
        Debug.Log("�߂�{�^���N���b�N");
        PlayButtonSound();
        ShowPanel("Menu");
    }
    
    // ===============================
    // �X���C�_�[�C�x���g����
    // ===============================
    
    void OnMusicVolumeChanged(float value)
    {
        currentMusicVolume = value;
        Debug.Log($"���y���ʕύX: {value:F2}");
        
        // AudioManager ������ꍇ�̏���
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
        
        // �ݒ��ۑ�
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    
    void OnSfxVolumeChanged(float value)
    {
        currentSfxVolume = value;
        Debug.Log($"���ʉ����ʕύX: {value:F2}");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
        
        PlayerPrefs.SetFloat("SfxVolume", value);
    }
    
    // ===============================
    // �g�O���C�x���g����
    // ===============================
    
    void OnFullscreenToggled(bool isFullscreen)
    {
        Debug.Log($"�t���X�N���[���؂�ւ�: {isFullscreen}");
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
    
    void OnShowFPSToggled(bool showFPSValue)
    {
        showFPS = showFPSValue;
        Debug.Log($"FPS�\���؂�ւ�: {showFPS}");
        
        if (fpsDisplay != null)
        {
            fpsDisplay.gameObject.SetActive(showFPS);
        }
        
        PlayerPrefs.SetInt("ShowFPS", showFPS ? 1 : 0);
    }
    
    // ===============================
    // �Q�[����ԘA�g
    // ===============================
    
    void OnGameStateChanged(GameStateManager.GameState newState)
    {
        Debug.Log($"UI: ��ԕύX���m {newState}");
        
        switch (newState)
        {
            case GameStateManager.GameState.MainMenu:
                ShowPanel("Menu");
                break;
            case GameStateManager.GameState.Playing:
                ShowPanel("Game");
                break;
            case GameStateManager.GameState.Paused:
                // �|�[�Y���j���[�̕\��
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
            
            // �X�R�A�ύX���̃A�j���[�V����
            AnimateScoreChange();
        }
    }
    
    void OnLivesChanged(int newLives)
    {
        if (livesDisplay != null)
        {
            livesDisplay.text = $"Lives: {newLives}";
            
            // ���C�t�����������̌x���\��
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
    // UI�X�V���\�b�h
    // ===============================
    
    void UpdateGameUI()
    {
        if (GameStateManager.Instance == null) return;
        
        // �^�C�}�[�X�V
        if (timerDisplay != null)
        {
            float time = GameStateManager.Instance.GetRemainingTime();
            if (time >= 0)
            {
                timerDisplay.text = $"Time: {time:F1}s";
                
                // �c�莞�Ԃ����Ȃ����̌x���\��
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
        
        // �n�C�X�R�A�X�V
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
    // �A�j���[�V��������
    // ===============================
    
    void UpdateAnimations()
    {
        // �p���[���[�h�C���W�P�[�^�[�̓_��
        if (powerModeIndicator != null)
        {
            bool isPowerMode = false; // GameManager����擾
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
        
        // �g��
        float elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.unscaledDeltaTime;
            scoreDisplay.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / 0.1f);
            yield return null;
        }
        
        // �k��
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
    // ���[�e�B���e�B���\�b�h
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
        // �{�^���N���b�N���̍Đ�
        if (AudioManager.Instance != null)
        {
            // AudioManager.Instance.PlayButtonSound();
        }
    }
    
    bool ShowConfirmDialog(string message)
    {
        // �ȒP�Ȋm�F�_�C�A���O�i���ۂ̃v���W�F�N�g�ł͂��������ꂽ���̂��g�p�j
        return true; // ���true�i������̂��߁j
    }
    
    void LoadUISettings()
    {
        // �ۑ����ꂽ�ݒ��ǂݍ���
        currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        currentSfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1.0f);
        showFPS = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        
        // UI�v�f�ɔ��f
        if (musicVolumeSlider != null) musicVolumeSlider.value = currentMusicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = currentSfxVolume;
        if (showFPSToggle != null) showFPSToggle.isOn = showFPS;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;
        
        Screen.fullScreen = fullscreen;
    }
    
    void HandleKeyboardInput()
    {
        // �L�[�{�[�h�V���[�g�J�b�g
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
    // �p�u���b�N���\�b�h
    // ===============================
    
    public void UpdateHealthBar(float healthPercent)
    {
        if (healthBar != null)
        {
            healthBar.value = healthPercent;
            
            // �w���X�ቺ���̐F�ύX
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
        Debug.Log($"�ʒm: {message}");
        // ���ۂ̒ʒm�\���V�X�e���������Ɏ���
    }
    
    public void SetButtonEnabled(string buttonName, bool enabled)
    {
        // ����̃{�^���̗L��/������؂�ւ�
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

### �X�e�b�v3: �J�X�^��UI�v�f�̍쐬

```csharp
using UnityEngine;
using UnityEngine.UI;

// �J�X�^���v���O���X�o�[
public class CustomProgressBar : MonoBehaviour
{
    [Header("�v���O���X�o�[�ݒ�")]
    public Image fillImage;
    public Text progressText;
    public bool showPercentage = true;
    public bool animateChanges = true;
    public float animationSpeed = 2f;
    
    [Header("�F�ݒ�")]
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

### �X�e�b�v4: UI�v�f�̔z�u�ƃe�X�g

1. Canvas �� UI�v�f��z�u
2. UIManager �X�N���v�g���A�^�b�`
3. Inspector �ŎQ�Ƃ�ݒ�
4. �Q�[�������s���ăe�X�g

## ?? �������Ă݂悤

### ����1: ���IUI����

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

### ����2: UI �f�[�^�o�C���f�B���O

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

### ����3: �A�N�Z�V�r���e�B�Ή�

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

## ? �悭����G���[

### �G���[1: UI�v�f���������Ȃ�
**����**: Canvas �� Graphic Raycaster �ݒ�
**�������@**: Canvas �� Graphic Raycaster �����邩�m�F

### �G���[2: �Q�ƃG���[
**����**: Inspector �ł̎Q�Ɛݒ�Y��
**�������@**: null �`�F�b�N�̒ǉ��ƓK�؂ȎQ�Ɛݒ�

### �G���[3: UI ����ʂ���͂ݏo��
**����**: Anchor �ݒ�̊ԈႢ
**�������@**: Rect Transform �� Anchor ��K�؂ɐݒ�

## ?? �w�K�|�C���g

### �d�v�ȊT�O
1. **Canvas System**: UI�`��V�X�e��
2. **Event System**: UI �C�x���g����
3. **Layout Groups**: �������C�A�E�g
4. **Anchoring**: ��ʃT�C�Y�Ή�
5. **Data Binding**: �f�[�^��UI�̘A�g

### UI�x�X�g�v���N�e�B�X
- **Responsive Design**: �قȂ��ʃT�C�Y�ւ̑Ή�
- **Accessibility**: �A�N�Z�V�r���e�B�̍l��
- **Performance**: UI �p�t�H�[�}���X�̍œK��
- **User Experience**: ���[�U�r���e�B�̌���

## ?? ���̃X�e�b�v

���̉ۑ芮����F
1. **����x�`�F�b�N**: UI �V�X�e���̓������
2. **����**: �J�X�^��UI�v�f�̍쐬
3. **���̉ۑ�**: �ۑ�06�u�����V�X�e���̎����v

## ?? �ǉ��`�������W

### �`�������W1: �ݒ�ۑ��V�X�e��
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

### �`�������W2: ���[�J���C�[�[�V����
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

### �`�������W3: UI ���W���[����
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
        // �A�j���[�V��������
    }
    
    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
```

�D�G�ł��I?? 
����UI�Ǘ��V�X�e�������������܂����I