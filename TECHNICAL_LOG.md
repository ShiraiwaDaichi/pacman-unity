# �Z�p�I�ύX���O

## ?? �T�v
���̃h�L�������g�́AUnity Pacman Learning Project�̋Z�p�I�ȕύX�_�Ǝ����ڍׂ��L�^�������̂ł��B

---

## ?? �����ς݋@�\

### 1. MazeGenerator.cs - ���H�����V�X�e��

#### ��v�@�\
```csharp
// ���H���C�A�E�g��2�����z���`
private int[,] mazeLayout = new int[,] {
    // 0=��, 1=��, 2=�h�b�g, 3=�p���[�y���b�g, 4=�v���C���[, 5=�S�[�X�g
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,1},
    // ... 21�s�̏ڍג�`
};
```

#### �Z�p�I����
- **���I����**: Instantiate�ɂ��v���n�u����̓��I�I�u�W�F�N�g����
- **�K�w�Ǘ�**: �����I�u�W�F�N�g�̐e�q�֌W�ɂ�鐮��
- **�������Ǘ�**: �Đ������̊����I�u�W�F�N�g�N���[���A�b�v
- **�G�f�B�^�[�A�g**: ContextMenu�ɂ��蓮���s�@�\

#### �����p�^�[��
```csharp
// �I�u�W�F�N�g�����̊�{�p�^�[��
GameObject obj = Instantiate(prefab, position, Quaternion.identity);
obj.transform.SetParent(transform);
obj.name = $"ObjectType_{x}_{y}";
```

### 2. PacmanController.cs - �v���C���[����

#### ���͏����V�X�e��
```csharp
// ���͂̌��o�Ɛ��K��
float horizontal = Input.GetAxisRaw("Horizontal");
float vertical = Input.GetAxisRaw("Vertical");
Vector3 direction = new Vector3(horizontal, 0, vertical);
```

#### �ړ�����A���S���Y��
- **���ԃx�[�X�ړ�**: `Time.deltaTime`�ɂ���葬�x�ړ�
- **�Փ˗\��**: Raycast�ɂ�鎖�O��Q�����o
- **�X���[�Y�ړ�**: ��Ԃ��g�p�����Ȃ߂炩�Ȉړ�

#### �œK���Z�p
```csharp
// ��������ɂ�������
if (direction.magnitude > 0.1f) {
    // �ړ������͎��ۂɓ��͂�����ꍇ�̂ݎ��s
    transform.Translate(direction * speed * Time.deltaTime);
}
```

### 3. GhostController.cs - AI����V�X�e��

#### ��ԊǗ��V�X�e��
```csharp
public enum GhostState {
    Chase,   // �ǐ�: �v���C���[�Ɍ������Ĉړ�
    Scatter, // �U��: �����_���ȕ����Ɉړ�
    Scared,  // ���|: �v���C���[���瓦��
    Eaten    // ���A: �X�^�[�g�n�_�ɖ߂�
}
```

#### AI �A���S���Y������

##### �ǐՃA���S���Y��
```csharp
Vector2 GetChaseDirection(Vector2[] directions) {
    Vector2 playerDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
    Vector2 bestDirection = currentDirection;
    float bestDot = -1f;
    
    foreach (Vector2 dir in directions) {
        if (CanMove(dir)) {
            float dot = Vector2.Dot(dir, playerDirection);
            if (dot > bestDot) {
                bestDot = dot;
                bestDirection = dir;
            }
        }
    }
    return bestDirection;
}
```

##### �����A���S���Y��
```csharp
Vector2 GetScaredDirection(Vector2[] directions) {
    // �v���C���[����ł��������������I��
    Vector2 playerDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
    Vector2 bestDirection = currentDirection;
    float bestDot = 1f;  // �ŏ��l��T��
    
    foreach (Vector2 dir in directions) {
        if (CanMove(dir)) {
            float dot = Vector2.Dot(dir, playerDirection);
            if (dot < bestDot) {
                bestDot = dot;
                bestDirection = dir;
            }
        }
    }
    return bestDirection;
}
```

#### �R���[�`���ɂ���Ԑ���
```csharp
IEnumerator StateController() {
    while (true) {
        if (currentState == GhostState.Chase) {
            yield return new WaitForSeconds(Random.Range(5f, 10f));
            if (currentState == GhostState.Chase) {
                SetState(GhostState.Scatter);
            }
        }
        // ���̏�ԏ���...
    }
}
```

### 4. GameManager.cs - �Q�[����ԊǗ�

#### �V���O���g���p�^�[������
```csharp
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
```

#### �X�R�A�V�X�e��
```csharp
public void AddScore(int points) {
    score += points;
    OnScoreChanged?.Invoke(score);
    
    // �{�[�i�X���C�t����
    if (score >= nextLifeScore) {
        GainLife();
        nextLifeScore += lifeScoreInterval;
    }
}
```

#### �Q�[���I������
```csharp
void CheckGameEnd() {
    // ��������: �S�h�b�g���W
    if (GameObject.FindGameObjectsWithTag("Dot").Length == 0) {
        GameWin();
    }
    // �s�k����: ���C�t�؂�
    if (lives <= 0) {
        GameOver();
    }
}
```

### 5. AudioManager.cs - �����V�X�e��

#### �����A�[�L�e�N�`��
```csharp
[System.Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
    public float volume = 1f;
    public float pitch = 1f;
    public bool loop = false;
    [HideInInspector] public AudioSource source;
}
```

#### 3D��������
```csharp
public void PlaySoundAtPosition(string soundName, Vector3 position) {
    Sound sound = Array.Find(sounds, s => s.name == soundName);
    if (sound != null) {
        AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume);
    }
}
```

#### �����t�F�[�h����
```csharp
IEnumerator FadeAudio(AudioSource source, float targetVolume, float duration) {
    float startVolume = source.volume;
    float elapsedTime = 0f;
    
    while (elapsedTime < duration) {
        elapsedTime += Time.deltaTime;
        source.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
        yield return null;
    }
    source.volume = targetVolume;
}
```

### 6. UIManager.cs - ���[�U�[�C���^�[�t�F�[�X

#### UI�X�V�V�X�e��
```csharp
public void UpdateScore(int score) {
    if (scoreText != null) {
        scoreText.text = "Score: " + score.ToString();
    }
}
```

#### �|�[�Y�@�\����
```csharp
public void TogglePause() {
    isPaused = !isPaused;
    
    if (isPaused) {
        Time.timeScale = 0f;  // �Q�[�����Ԃ��~
        pausePanel.SetActive(true);
    } else {
        Time.timeScale = 1f;  // �Q�[�����Ԃ��ĊJ
        pausePanel.SetActive(false);
    }
}
```

#### �C�x���g�h���u��UI
```csharp
void Start() {
    // �{�^���C�x���g�̐ݒ�
    if (restartButton != null) {
        restartButton.onClick.AddListener(RestartGame);
    }
    if (pauseButton != null) {
        pauseButton.onClick.AddListener(TogglePause);
    }
}
```

### 7. CameraController.cs - �J��������

#### �X���[�Y�Ǐ]�V�X�e��
```csharp
void LateUpdate() {
    if (target != null) {
        Vector3 targetPosition = target.position + offset;
        
        // ���E���ɐ���
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        
        // �X���[�Y�Ȉړ�
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
```

#### ���I�Y�[������
```csharp
void UpdateZoom() {
    float targetSize = baseSize;
    
    // �v���C���[�̑��x�ɉ����ăY�[������
    if (player != null) {
        float playerSpeed = player.GetComponent<Rigidbody>().velocity.magnitude;
        targetSize = baseSize + (playerSpeed * zoomSpeedMultiplier);
    }
    
    Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
}
```

### 8. PowerPellet.cs & Dot.cs - �A�C�e���V�X�e��

#### �A�j���[�V��������
```csharp
void Update() {
    if (animateScale) {
        float scale = 1f + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
        transform.localScale = originalScale * scale;
    }
    
    if (blinkingEffect) {
        Color color = originalColor;
        color.a = 0.5f + Mathf.Sin(Time.time * blinkSpeed) * 0.5f;
        spriteRenderer.color = color;
    }
}
```

#### ���W����
```csharp
void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
        // �X�R�A���Z
        if (GameManager.Instance != null) {
            GameManager.Instance.AddScore(pointValue);
        }
        
        // ���ʉ��Đ�
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySound("CollectDot");
        }
        
        // �I�u�W�F�N�g�j��
        Destroy(gameObject);
    }
}
```

---

## ?? �ŐV�̒ǉ��@�\

### SimpleMazeGenerator.cs - �J���x���c�[��

#### �ړI
�v���n�u���g�p�����Ɋ�{�I�Ȗ��H�𓮓I�������A�w�K�҂̓���m�F���x��

#### �Z�p�I����
```csharp
void CreateWall(int x, int y) {
    // �v���~�e�B�u���瓮�I����
    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
    wall.transform.position = new Vector3(x, 0, -y);
    wall.transform.SetParent(transform);
    wall.name = $"Wall_{x}_{y}";
    wall.tag = "Wall";
    
    // �}�e���A���K�p
    if (wallMaterial != null) {
        wall.GetComponent<Renderer>().material = wallMaterial;
    }
}
```

#### �w�K�Ҍ����@�\
- **�����̓���m�F**: �v���n�u�쐬�s�v
- **�ȒP�Z�b�g�A�b�v**: 3��Material�̂ݐݒ�
- **�i�K�I�w�K**: �{�i�V�X�e���ւ̈ڍs����

---

## ?? �݌v�p�^�[���̎���

### 1. �V���O���g���p�^�[��
```csharp
// ������: GameManager
public static GameManager Instance { get; private set; }

void Awake() {
    if (Instance == null) {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    } else {
        Destroy(gameObject);
    }
}
```

**�g�p���R**: �Q�[���S�̂ň�̃}�l�[�W���[�C���X�^���X���K�v

### 2. �I�u�U�[�o�[�p�^�[��
```csharp
// �C�x���g�V�X�e��
public static System.Action<int> OnScoreChanged;
public static System.Action<GameState> OnStateChanged;

// ����
OnScoreChanged?.Invoke(newScore);

// �w��
void Start() {
    GameManager.OnScoreChanged += UpdateScoreDisplay;
}

void OnDestroy() {
    GameManager.OnScoreChanged -= UpdateScoreDisplay;
}
```

**�g�p���R**: UI�X�V�Ȃǂ̑a�����ȘA�g������

### 3. �X�e�[�g�}�V���p�^�[��
```csharp
// ��Ԃ̒�`
public enum GameState {
    MainMenu, Playing, Paused, GameOver, Victory
}

// ��ԑJ��
public void ChangeState(GameState newState) {
    ExitState(currentState);
    currentState = newState;
    EnterState(newState);
}

// ��ԕʂ̏���
void EnterState(GameState state) {
    switch (state) {
        case GameState.Playing:
            Time.timeScale = 1f;
            break;
        case GameState.Paused:
            Time.timeScale = 0f;
            break;
    }
}
```

**�g�p���R**: ���G�ȏ�ԊǗ��𐮗����Ď���

### 4. �I�u�W�F�N�g�v�[���p�^�[���i���������j
```csharp
// ���ʉ��̓����Đ�����
public class AudioManager : MonoBehaviour {
    public int maxSimultaneousSounds = 10;
    private Queue<AudioSource> audioSourcePool;
    
    void InitializePool() {
        audioSourcePool = new Queue<AudioSource>();
        for (int i = 0; i < maxSimultaneousSounds; i++) {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            audioSourcePool.Enqueue(source);
        }
    }
}
```

**�g�p���R**: �����������ƃp�t�H�[�}���X�̌���

---

## ?? �p�t�H�[�}���X�œK��

### 1. �X�V�p�x�̍œK��
```csharp
// UI�X�V��K�v���̂ݎ��s
private int lastScore = -1;
void Update() {
    if (GameManager.Instance.score != lastScore) {
        UpdateScoreDisplay();
        lastScore = GameManager.Instance.score;
    }
}
```

### 2. �I�u�W�F�N�g�����̍œK��
```csharp
// ���H�������̌�����
public void GenerateMaze() {
    ClearMaze();  // �����I�u�W�F�N�g�N���A
    
    // �ꊇ�����ŏ������ׂ��y��
    for (int y = 0; y < mazeHeight; y++) {
        for (int x = 0; x < mazeWidth; x++) {
            CreateMazeElement(x, y);
        }
    }
}
```

### 3. ���������[�N�΍�
```csharp
// �C�x���g�w�ǂ̓K�؂ȉ���
void OnDestroy() {
    // �S�ẴC�x���g�w�ǂ�����
    GameManager.OnScoreChanged -= UpdateScoreDisplay;
    GameManager.OnStateChanged -= UpdateGameState;
}
```

---

## ?? �Z�p�I�ȉۑ�Ɖ�����

### �ۑ�1: �v���n�u�ˑ��̉���
**���**: �w�K�҂��v���n�u�쐬�Ɏ��Ԃ�v����  
**����**: SimpleMazeGenerator�ɂ�铮�I�����V�X�e��

### �ۑ�2: ���G�ȏ�ԊǗ�
**���**: �Q�[����Ԃ̊Ǘ������G  
**����**: �X�e�[�g�}�V���p�^�[���ɂ�鐮��

### �ۑ�3: UI�X�V�̌�����
**���**: ���t���[��UI�X�V�ɂ�镉��  
**����**: �ύX���o�ɂ������t���X�V

### �ۑ�4: �����V�X�e���̍œK��
**���**: �����Đ��ɂ�鉹���i���̗�  
**����**: ���������ƃv�[���V�X�e��

---

## ?? �R�[�h���g���N�X

### �t�@�C���ʍs��
- **MazeGenerator.cs**: 324�s
- **PacmanController.cs**: 298�s
- **GhostController.cs**: 456�s
- **GameManager.cs**: 387�s
- **UIManager.cs**: 213�s
- **AudioManager.cs**: 267�s
- **CameraController.cs**: 189�s
- **PowerPellet.cs**: 134�s
- **Dot.cs**: 89�s
- **SimpleMazeGenerator.cs**: 156�s

### ���G�x���g���N�X
- **���σ��\�b�h��**: 15�s
- **�ő�l�X�g�[�x**: 4���x��
- **�N���X�����x**: ��i�a�����݌v�j
- **�R�����g���x**: ��40%

---

## ?? �w�K�Ҍ��������K�C�h

### 1. ��{�I�Ȏ�������
1. **GameObject�쐬** �� ��{�I�ȃI�u�W�F�N�g�z�u
2. **�X�N���v�g����** �� �@�\�̒i�K�I�ǉ�
3. **�A�g�V�X�e��** �� �R���|�[�l���g�Ԃ̋���
4. **�œK��** �� �p�t�H�[�}���X���P

### 2. �悭����G���[�Ƃ��̑Ώ�
```csharp
// NullReferenceException�΍�
if (gameObject != null && gameObject.activeInHierarchy) {
    // ���S�ȏ���
}

// �d�����s�h�~
if (isProcessing) return;
isProcessing = true;
```

### 3. �f�o�b�O�x���@�\
```csharp
[System.Diagnostics.Conditional("UNITY_EDITOR")]
public void DebugLog(string message) {
    Debug.Log($"[{GetType().Name}] {message}");
}
```

---

## ?? ����̋Z�p�I���P�\��

### 1. �A�[�L�e�N�`���̉��P
- **�ˑ�������**: ���_��ȃR���|�[�l���g�A�g
- **SOLID����**: ���ێ琫�̍����݌v
- **�e�X�g�쓮�J��**: �P�̃e�X�g�̎���

### 2. �p�t�H�[�}���X�̌���
- **�I�u�W�F�N�g�v�[��**: ���S�Ȏ���
- **LOD�V�X�e��**: �����ɉ������ڍדx����
- **�v���t�@�C�����O**: ����I�Ȑ��\����

### 3. �@�\�̊g��
- **���x���G�f�B�^�[**: ���H�̓��I�ҏW
- **AI���P**: ��荂�x�ȓGAI
- **�l�b�g���[�N**: �}���`�v���C���[�Ή�

---

���̃e�N�j�J�����O�́A�J���҂Ɗw�K�҂̗����Ɍ������Z�p�I�ȎQ�l�����Ƃ��Čp���I�ɍX�V����܂��B ???