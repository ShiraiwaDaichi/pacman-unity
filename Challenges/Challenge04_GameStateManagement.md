# �ۑ�04: �Q�[����ԊǗ��V�X�e��

## ?? �w�K�ڕW
- enum�i�񋓌^�j���g������ԊǗ�
- �V���O���g���p�^�[���̊�b����
- �Q�[�����[�v�ƃX�e�[�g�}�V��
- UI �Ƃ̘A�g�V�X�e��
- �C�x���g�V�X�e���̎���

## ? ���菊�v����
�� 75-90 ��

## ?? �O��m��
- �ۑ�03�̊���
- enum �̊�{�T�O
- static �L�[���[�h�̗���
- public/private �̎g������

## ?? �ۑ���e

### �X�e�b�v1: UI�L�����o�X�̏���

#### 1.1 UI�̍쐬
1. Hierarchy �ŉE�N���b�N �� UI �� Canvas ���쐬
2. Canvas ���� UI �� Text ��3�쐬�F
   - "ScoreText"
   - "StatusText" 
   - "TimerText"
3. ��ʏ�ɓK�؂ɔz�u

#### 1.2 �{�^���̒ǉ�
1. Canvas ���� UI �� Button ��2�쐬�F
   - "StartButton"
   - "ResetButton"

### �X�e�b�v2: �Q�[����ԊǗ��X�N���v�g�̍쐬

�ȉ��̃X�N���v�g�� **�����œ���** ���Ă��������F

```csharp
using UnityEngine;
using UnityEngine.UI;

// �Q�[���S�̂̏�Ԃ��Ǘ�����N���X
public class GameStateManager : MonoBehaviour
{
    // ===============================
    // �Q�[����Ԃ̒�`�ienum�j
    // ===============================
    
    // enum�͊֘A����萔���O���[�v�����邽�߂̃f�[�^�^�ł�
    public enum GameState
    {
        MainMenu,    // ���C�����j���[���
        Playing,     // �Q�[����
        Paused,      // �|�[�Y��
        GameOver,    // �Q�[���I�[�o�[
        Victory      // ����
    }
    
    // ===============================
    // �V���O���g���p�^�[���̎���
    // ===============================
    
    // �V���O���g���p�^�[���F���̃N���X�̃C���X�^���X��1�����ɐ���
    public static GameStateManager Instance { get; private set; }
    
    // ===============================
    // �p�u���b�N�ϐ��i�ݒ�\�j
    // ===============================
    
    [Header("�Q�[���ݒ�")]
    public GameState currentState = GameState.MainMenu;    // ���݂̏��
    public float gameTimeLimit = 60.0f;                   // �Q�[�����Ԑ����i�b�j
    public int targetScore = 100;                         // �ڕW�X�R�A
    public bool useTimeLimit = true;                      // ���Ԑ������g�p���邩
    
    [Header("UI�Q��")]
    public Text scoreText;                                // �X�R�A�\���p�e�L�X�g
    public Text statusText;                               // ��ԕ\���p�e�L�X�g
    public Text timerText;                                // �^�C�}�[�\���p�e�L�X�g
    public Button startButton;                            // �X�^�[�g�{�^��
    public Button resetButton;                            // ���Z�b�g�{�^��
    
    [Header("�Q�[���f�[�^")]
    public int currentScore = 0;                          // ���݂̃X�R�A
    public int lives = 3;                                 // �c�胉�C�t
    public float gameTimer = 0f;                          // �Q�[���^�C�}�[
    
    // ===============================
    // �v���C�x�[�g�ϐ��i�����Ǘ��j
    // ===============================
    
    private GameState previousState;                      // �O�̏��
    private float stateChangeTime;                        // ��ԕύX����
    private bool isGameActive = false;                    // �Q�[�����A�N�e�B�u��
    
    // ���v�f�[�^
    private float totalPlayTime = 0f;
    private int gamesPlayed = 0;
    private int highScore = 0;
    
    // ===============================
    // �C�x���g�V�X�e���iC#��Action�j
    // ===============================
    
    // ���̃X�N���v�g����ԕω����Ď��ł���悤�ɂ���C�x���g
    public static System.Action<GameState> OnStateChanged;
    public static System.Action<int> OnScoreChanged;
    public static System.Action<int> OnLivesChanged;
    public static System.Action OnGameWon;
    public static System.Action OnGameLost;
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    void Awake()
    {
        // �V���O���g���p�^�[���̎���
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �V�[���ύX�Ŕj������Ȃ��悤�ɂ���
        }
        else
        {
            Destroy(gameObject);  // ���ɃC���X�^���X�����݂���ꍇ�͍폜
            return;
        }
    }
    
    void Start()
    {
        // �����ݒ�
        SetupUI();
        ChangeState(GameState.MainMenu);
        
        Debug.Log("=== �Q�[����ԊǗ��V�X�e���J�n ===");
        Debug.Log("�������: " + currentState);
        
        // �{�^���C�x���g�̐ݒ�
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetGame);
    }
    
    void Update()
    {
        // ���݂̏�Ԃɉ���������
        switch (currentState)
        {
            case GameState.MainMenu:
                UpdateMainMenu();
                break;
            case GameState.Playing:
                UpdatePlaying();
                break;
            case GameState.Paused:
                UpdatePaused();
                break;
            case GameState.GameOver:
                UpdateGameOver();
                break;
            case GameState.Victory:
                UpdateVictory();
                break;
        }
        
        // UI�X�V
        UpdateUI();
        
        // ����L�[����
        HandleInput();
    }
    
    // ===============================
    // ��ԊǗ����\�b�h
    // ===============================
    
    // ��Ԃ�ύX�����v���\�b�h
    public void ChangeState(GameState newState)
    {
        // �O�̏�Ԃ��L�^
        previousState = currentState;
        
        // ���݂̏�Ԃ��I��
        ExitState(currentState);
        
        // �V������ԂɕύX
        currentState = newState;
        stateChangeTime = Time.time;
        
        // �V������Ԃ��J�n
        EnterState(newState);
        
        // �C�x���g�ʒm
        OnStateChanged?.Invoke(newState);
        
        Debug.Log($"��ԕύX: {previousState} �� {currentState}");
    }
    
    // ��Ԃɓ��鎞�̏���
    void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                Debug.Log("���C�����j���[�ɓ���܂���");
                isGameActive = false;
                break;
                
            case GameState.Playing:
                Debug.Log("�Q�[���J�n�I");
                isGameActive = true;
                gameTimer = useTimeLimit ? gameTimeLimit : 0f;
                break;
                
            case GameState.Paused:
                Debug.Log("�Q�[���|�[�Y");
                Time.timeScale = 0f;  // �Q�[�����Ԃ��~
                break;
                
            case GameState.GameOver:
                Debug.Log("�Q�[���I�[�o�[");
                HandleGameEnd(false);
                break;
                
            case GameState.Victory:
                Debug.Log("�����I");
                HandleGameEnd(true);
                break;
        }
    }
    
    // ��Ԃ���o�鎞�̏���
    void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                totalPlayTime += Time.time - stateChangeTime;
                break;
                
            case GameState.Paused:
                Time.timeScale = 1f;  // �Q�[�����Ԃ��ĊJ
                break;
        }
    }
    
    // ===============================
    // �e��Ԃ̍X�V����
    // ===============================
    
    void UpdateMainMenu()
    {
        // ���C�����j���[�ł̏���
        // ��F�^�C�g����ʂ̃A�j���[�V�����A���y�Ȃ�
    }
    
    void UpdatePlaying()
    {
        // �Q�[�����̏���
        
        // �^�C�}�[�X�V
        if (useTimeLimit)
        {
            gameTimer -= Time.deltaTime;
            
            // ���Ԑ؂�`�F�b�N
            if (gameTimer <= 0f)
            {
                gameTimer = 0f;
                ChangeState(GameState.GameOver);
                return;
            }
        }
        else
        {
            gameTimer += Time.deltaTime;  // �o�ߎ��Ԃ��J�E���g
        }
        
        // ���������`�F�b�N
        if (currentScore >= targetScore)
        {
            ChangeState(GameState.Victory);
        }
        
        // ���C�t�؂�`�F�b�N
        if (lives <= 0)
        {
            ChangeState(GameState.GameOver);
        }
    }
    
    void UpdatePaused()
    {
        // �|�[�Y���̏���
        // ��F�|�[�Y���j���[�̕\���A�ݒ�ύX�Ȃ�
    }
    
    void UpdateGameOver()
    {
        // �Q�[���I�[�o�[��Ԃ̏���
        // ��F�Q�[���I�[�o�[��ʂ̕\���A�X�R�A�ۑ��Ȃ�
    }
    
    void UpdateVictory()
    {
        // ������Ԃ̏���
        // ��F�������o�A���̃��x���ւ̈ڍs�Ȃ�
    }
    
    // ===============================
    // �Q�[�����W�b�N����
    // ===============================
    
    // �X�R�A��ǉ�
    public void AddScore(int points)
    {
        if (currentState != GameState.Playing) return;
        
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
        
        // �n�C�X�R�A�X�V
        if (currentScore > highScore)
        {
            highScore = currentScore;
            Debug.Log("�V�n�C�X�R�A: " + highScore);
        }
        
        Debug.Log($"�X�R�A�ǉ�: +{points} (���v: {currentScore})");
    }
    
    // ���C�t�����炷
    public void LoseLife()
    {
        if (currentState != GameState.Playing) return;
        
        lives--;
        OnLivesChanged?.Invoke(lives);
        
        Debug.Log($"���C�t���� (�c��: {lives})");
        
        if (lives <= 0)
        {
            ChangeState(GameState.GameOver);
        }
    }
    
    // ���C�t�𑝂₷
    public void GainLife()
    {
        lives++;
        OnLivesChanged?.Invoke(lives);
        Debug.Log($"���C�t�l�� (���v: {lives})");
    }
    
    // �Q�[���J�n
    public void StartGame()
    {
        if (currentState == GameState.MainMenu || currentState == GameState.GameOver || currentState == GameState.Victory)
        {
            ResetGameData();
            ChangeState(GameState.Playing);
            gamesPlayed++;
        }
    }
    
    // �Q�[�����Z�b�g
    public void ResetGame()
    {
        ResetGameData();
        ChangeState(GameState.MainMenu);
        Debug.Log("�Q�[�����Z�b�g����");
    }
    
    // �Q�[���f�[�^�����Z�b�g
    void ResetGameData()
    {
        currentScore = 0;
        lives = 3;
        gameTimer = useTimeLimit ? gameTimeLimit : 0f;
        isGameActive = false;
    }
    
    // �Q�[���I������
    void HandleGameEnd(bool won)
    {
        isGameActive = false;
        
        if (won)
        {
            OnGameWon?.Invoke();
            Debug.Log("=== �����I ===");
        }
        else
        {
            OnGameLost?.Invoke();
            Debug.Log("=== �s�k ===");
        }
        
        Debug.Log($"�ŏI�X�R�A: {currentScore}");
        Debug.Log($"�v���C����: {totalPlayTime:F1}�b");
    }
    
    // ===============================
    // ���͏���
    // ===============================
    
    void HandleInput()
    {
        // ESC�L�[�Ń|�[�Y/�|�[�Y����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }
        
        // Space�L�[�ŃQ�[���J�n/�ĊJ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentState == GameState.MainMenu)
            {
                StartGame();
            }
            else if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }
        
        // R�L�[�Ń��Z�b�g
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
        
        // �f�o�b�O�p�F��ԏ��\��
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowGameInfo();
        }
    }
    
    // ===============================
    // UI�Ǘ�
    // ===============================
    
    void SetupUI()
    {
        // UI�̏����ݒ�
        UpdateUI();
    }
    
    void UpdateUI()
    {
        // �X�R�A�\��
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore} / {targetScore}";
        }
        
        // ��ԕ\��
        if (statusText != null)
        {
            string stateText = GetStateDisplayText();
            statusText.text = $"Status: {stateText} | Lives: {lives}";
        }
        
        // �^�C�}�[�\��
        if (timerText != null)
        {
            if (useTimeLimit)
            {
                timerText.text = $"Time: {gameTimer:F1}s";
            }
            else
            {
                timerText.text = $"Time: {gameTimer:F1}s";
            }
        }
        
        // �{�^���̕\������
        UpdateButtonStates();
    }
    
    // ��Ԃɉ������\���e�L�X�g���擾
    string GetStateDisplayText()
    {
        return currentState switch
        {
            GameState.MainMenu => "���j���[",
            GameState.Playing => "�v���C��",
            GameState.Paused => "�|�[�Y",
            GameState.GameOver => "�Q�[���I�[�o�[",
            GameState.Victory => "�����I",
            _ => "�s��"
        };
    }
    
    // �{�^���̏�Ԃ��X�V
    void UpdateButtonStates()
    {
        if (startButton != null)
        {
            startButton.gameObject.SetActive(
                currentState == GameState.MainMenu || 
                currentState == GameState.GameOver || 
                currentState == GameState.Victory
            );
        }
        
        if (resetButton != null)
        {
            resetButton.gameObject.SetActive(true);  // ��ɕ\��
        }
    }
    
    // ===============================
    // ���\��
    // ===============================
    
    // �Q�[������\��
    void ShowGameInfo()
    {
        Debug.Log("=== �Q�[����� ===");
        Debug.Log($"���ݏ��: {currentState}");
        Debug.Log($"�X�R�A: {currentScore} / {targetScore}");
        Debug.Log($"���C�t: {lives}");
        Debug.Log($"�^�C�}�[: {gameTimer:F1}�b");
        Debug.Log($"�n�C�X�R�A: {highScore}");
        Debug.Log($"�Q�[���v���C��: {gamesPlayed}");
        Debug.Log($"���v���C����: {totalPlayTime:F1}�b");
    }
    
    // ===============================
    // �p�u���b�N���\�b�h�i�O������Ăяo���\�j
    // ===============================
    
    // ���݂̏�Ԃ��擾
    public GameState GetCurrentState()
    {
        return currentState;
    }
    
    // �Q�[�����A�N�e�B�u���`�F�b�N
    public bool IsGameActive()
    {
        return isGameActive;
    }
    
    // �c�莞�Ԃ��擾
    public float GetRemainingTime()
    {
        return useTimeLimit ? gameTimer : -1f;
    }
    
    // �n�C�X�R�A���擾
    public int GetHighScore()
    {
        return highScore;
    }
}
```

### �X�e�b�v3: ��ԘA�g�X�N���v�g�̍쐬

```csharp
using UnityEngine;

// ���̃X�N���v�g���Q�[����Ԃɔ������邽�߂̗�
public class StateReactiveObject : MonoBehaviour
{
    [Header("��Ԕ����ݒ�")]
    public GameObject activeInGame;      // �Q�[�����̂ݕ\��
    public GameObject activeInMenu;      // ���j���[���̂ݕ\��
    
    void Start()
    {
        // ��ԕω��C�x���g���w��
        GameStateManager.OnStateChanged += HandleStateChange;
        GameStateManager.OnScoreChanged += HandleScoreChange;
    }
    
    void OnDestroy()
    {
        // �C�x���g�w�ǂ������i���������[�N�h�~�j
        GameStateManager.OnStateChanged -= HandleStateChange;
        GameStateManager.OnScoreChanged -= HandleScoreChange;
    }
    
    // ��ԕω����̏���
    void HandleStateChange(GameStateManager.GameState newState)
    {
        Debug.Log($"��ԕω������m: {newState}");
        
        // �I�u�W�F�N�g�̕\������
        if (activeInGame != null)
        {
            activeInGame.SetActive(newState == GameStateManager.GameState.Playing);
        }
        
        if (activeInMenu != null)
        {
            activeInMenu.SetActive(newState == GameStateManager.GameState.MainMenu);
        }
    }
    
    // �X�R�A�ω����̏���
    void HandleScoreChange(int newScore)
    {
        Debug.Log($"�X�R�A�ω������m: {newScore}");
        
        // �X�R�A�ɉ���������
        if (newScore >= 50)
        {
            // ���ʂȌ��ʂ𔭓�
            Debug.Log("�n�[�t�E�F�C�{�[�i�X�I");
        }
    }
}
```

### �X�e�b�v4: �����X�N���v�g�Ƃ̘A�g

�ۑ�03��CollisionDetector�X�N���v�g�Ɉȉ���ǉ��F

```csharp
// CollisionDetector�N���X��HandleCollectibleCollision���\�b�h���C��
void HandleCollectibleCollision(Collider other)
{
    // �Q�[�����L���ȏꍇ�̂ݏ���
    if (GameStateManager.Instance != null && GameStateManager.Instance.IsGameActive())
    {
        GameStateManager.Instance.AddScore(itemValue);
        // ���̏���...
    }
}
```

## ?? �������Ă݂悤

### ����1: �J�X�^����Ԃ̒ǉ�

```csharp
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Victory,
    Loading,     // �V�������
    Settings     // �V�������
}
```

### ����2: ��ԑJ�ڂ̐���

```csharp
// ����̏�Ԃ���̂ݑJ�ډ\�ɂ���
bool CanChangeState(GameState from, GameState to)
{
    switch (from)
    {
        case GameState.MainMenu:
            return to == GameState.Playing || to == GameState.Settings;
        case GameState.Playing:
            return to == GameState.Paused || to == GameState.GameOver || to == GameState.Victory;
        // ...
    }
    return false;
}
```

### ����3: �Z�[�u/���[�h�@�\

```csharp
[System.Serializable]
public class GameData
{
    public int highScore;
    public float totalPlayTime;
    public int gamesPlayed;
}

public void SaveGame()
{
    GameData data = new GameData
    {
        highScore = this.highScore,
        totalPlayTime = this.totalPlayTime,
        gamesPlayed = this.gamesPlayed
    };
    
    string json = JsonUtility.ToJson(data);
    PlayerPrefs.SetString("GameData", json);
}
```

## ? �悭����G���[

### �G���[1: �V���O���g�����@�\���Ȃ�
**����**: Awake()�ł̐ݒ�~�X
**�������@**: DontDestroyOnLoad()�̓K�؂Ȏg�p

### �G���[2: �C�x���g�̃��������[�N
**����**: �C�x���g�w�ǂ̉����Y��
**�������@**: OnDestroy()�ł̍w�ǉ���

### �G���[3: UI�Q�ƃG���[
**����**: Inspector �ł̎Q�Ɛݒ�Y��
**�������@**: null �`�F�b�N�̒ǉ�

## ?? �w�K�|�C���g

### �d�v�ȊT�O
1. **enum**: ��Ԃ̒�`�ƊǗ�
2. **Singleton Pattern**: �P��C���X�^���X�̕ۏ�
3. **State Machine**: ��ԑJ�ڃV�X�e��
4. **Event System**: �C�x���g�쓮�v���O���~���O
5. **UI Integration**: UI�ƃ��W�b�N�̘A�g

### �݌v�p�^�[��
- **State Pattern**: ��Ԃɉ����������̐؂�ւ�
- **Observer Pattern**: �C�x���g�V�X�e���̊�b
- **Singleton Pattern**: �O���[�o���A�N�Z�X

## ?? ���̃X�e�b�v

���̉ۑ芮����F
1. **����x�`�F�b�N**: �X�e�[�g�}�V���̊T�O����
2. **����**: �V������Ԃ̒ǉ�
3. **���̉ۑ�**: �ۑ�05�uUI�쐬�ƃC�x���g�����v

## ?? �ǉ��`�������W

### �`�������W1: �A�j���[�V�����A�g
```csharp
public Animator stateAnimator;

void EnterState(GameState state)
{
    if (stateAnimator != null)
    {
        stateAnimator.SetTrigger($"Enter{state}");
    }
}
```

### �`�������W2: �����V�X�e���A�g
```csharp
public AudioClip[] stateMusic;

void PlayStateMusic(GameState state)
{
    int index = (int)state;
    if (index < stateMusic.Length && stateMusic[index] != null)
    {
        // ���y�Đ�����
    }
}
```

### �`�������W3: �ݒ�V�X�e��
```csharp
public class GameSettings
{
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public bool showDebugInfo = false;
    public KeyCode pauseKey = KeyCode.Escape;
}
```

�f���炵���I?? 
�Q�[����ԊǗ��V�X�e�������������܂����I