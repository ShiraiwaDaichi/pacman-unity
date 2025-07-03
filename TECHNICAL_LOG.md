# 技術的変更ログ

## ?? 概要
このドキュメントは、Unity Pacman Learning Projectの技術的な変更点と実装詳細を記録したものです。

---

## ?? 実装済み機能

### 1. MazeGenerator.cs - 迷路生成システム

#### 主要機能
```csharp
// 迷路レイアウトの2次元配列定義
private int[,] mazeLayout = new int[,] {
    // 0=空, 1=壁, 2=ドット, 3=パワーペレット, 4=プレイヤー, 5=ゴースト
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,1},
    // ... 21行の詳細定義
};
```

#### 技術的特徴
- **動的生成**: Instantiateによるプレハブからの動的オブジェクト生成
- **階層管理**: 生成オブジェクトの親子関係による整理
- **メモリ管理**: 再生成時の既存オブジェクトクリーンアップ
- **エディター連携**: ContextMenuによる手動実行機能

#### 実装パターン
```csharp
// オブジェクト生成の基本パターン
GameObject obj = Instantiate(prefab, position, Quaternion.identity);
obj.transform.SetParent(transform);
obj.name = $"ObjectType_{x}_{y}";
```

### 2. PacmanController.cs - プレイヤー制御

#### 入力処理システム
```csharp
// 入力の検出と正規化
float horizontal = Input.GetAxisRaw("Horizontal");
float vertical = Input.GetAxisRaw("Vertical");
Vector3 direction = new Vector3(horizontal, 0, vertical);
```

#### 移動制御アルゴリズム
- **時間ベース移動**: `Time.deltaTime`による一定速度移動
- **衝突予測**: Raycastによる事前障害物検出
- **スムーズ移動**: 補間を使用したなめらかな移動

#### 最適化技術
```csharp
// 条件分岐による効率化
if (direction.magnitude > 0.1f) {
    // 移動処理は実際に入力がある場合のみ実行
    transform.Translate(direction * speed * Time.deltaTime);
}
```

### 3. GhostController.cs - AI制御システム

#### 状態管理システム
```csharp
public enum GhostState {
    Chase,   // 追跡: プレイヤーに向かって移動
    Scatter, // 散乱: ランダムな方向に移動
    Scared,  // 恐怖: プレイヤーから逃走
    Eaten    // 復帰: スタート地点に戻る
}
```

#### AI アルゴリズム実装

##### 追跡アルゴリズム
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

##### 逃走アルゴリズム
```csharp
Vector2 GetScaredDirection(Vector2[] directions) {
    // プレイヤーから最も遠ざかる方向を選択
    Vector2 playerDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
    Vector2 bestDirection = currentDirection;
    float bestDot = 1f;  // 最小値を探す
    
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

#### コルーチンによる状態制御
```csharp
IEnumerator StateController() {
    while (true) {
        if (currentState == GhostState.Chase) {
            yield return new WaitForSeconds(Random.Range(5f, 10f));
            if (currentState == GhostState.Chase) {
                SetState(GhostState.Scatter);
            }
        }
        // 他の状態処理...
    }
}
```

### 4. GameManager.cs - ゲーム状態管理

#### シングルトンパターン実装
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

#### スコアシステム
```csharp
public void AddScore(int points) {
    score += points;
    OnScoreChanged?.Invoke(score);
    
    // ボーナスライフ判定
    if (score >= nextLifeScore) {
        GainLife();
        nextLifeScore += lifeScoreInterval;
    }
}
```

#### ゲーム終了条件
```csharp
void CheckGameEnd() {
    // 勝利条件: 全ドット収集
    if (GameObject.FindGameObjectsWithTag("Dot").Length == 0) {
        GameWin();
    }
    // 敗北条件: ライフ切れ
    if (lives <= 0) {
        GameOver();
    }
}
```

### 5. AudioManager.cs - 音響システム

#### 音響アーキテクチャ
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

#### 3D音響実装
```csharp
public void PlaySoundAtPosition(string soundName, Vector3 position) {
    Sound sound = Array.Find(sounds, s => s.name == soundName);
    if (sound != null) {
        AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume);
    }
}
```

#### 音響フェード効果
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

### 6. UIManager.cs - ユーザーインターフェース

#### UI更新システム
```csharp
public void UpdateScore(int score) {
    if (scoreText != null) {
        scoreText.text = "Score: " + score.ToString();
    }
}
```

#### ポーズ機能実装
```csharp
public void TogglePause() {
    isPaused = !isPaused;
    
    if (isPaused) {
        Time.timeScale = 0f;  // ゲーム時間を停止
        pausePanel.SetActive(true);
    } else {
        Time.timeScale = 1f;  // ゲーム時間を再開
        pausePanel.SetActive(false);
    }
}
```

#### イベントドリブンUI
```csharp
void Start() {
    // ボタンイベントの設定
    if (restartButton != null) {
        restartButton.onClick.AddListener(RestartGame);
    }
    if (pauseButton != null) {
        pauseButton.onClick.AddListener(TogglePause);
    }
}
```

### 7. CameraController.cs - カメラ制御

#### スムーズ追従システム
```csharp
void LateUpdate() {
    if (target != null) {
        Vector3 targetPosition = target.position + offset;
        
        // 境界内に制限
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        
        // スムーズな移動
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
```

#### 動的ズーム制御
```csharp
void UpdateZoom() {
    float targetSize = baseSize;
    
    // プレイヤーの速度に応じてズーム調整
    if (player != null) {
        float playerSpeed = player.GetComponent<Rigidbody>().velocity.magnitude;
        targetSize = baseSize + (playerSpeed * zoomSpeedMultiplier);
    }
    
    Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
}
```

### 8. PowerPellet.cs & Dot.cs - アイテムシステム

#### アニメーション効果
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

#### 収集処理
```csharp
void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
        // スコア加算
        if (GameManager.Instance != null) {
            GameManager.Instance.AddScore(pointValue);
        }
        
        // 効果音再生
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySound("CollectDot");
        }
        
        // オブジェクト破棄
        Destroy(gameObject);
    }
}
```

---

## ?? 最新の追加機能

### SimpleMazeGenerator.cs - 開発支援ツール

#### 目的
プレハブを使用せずに基本的な迷路を動的生成し、学習者の動作確認を支援

#### 技術的実装
```csharp
void CreateWall(int x, int y) {
    // プリミティブから動的生成
    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
    wall.transform.position = new Vector3(x, 0, -y);
    wall.transform.SetParent(transform);
    wall.name = $"Wall_{x}_{y}";
    wall.tag = "Wall";
    
    // マテリアル適用
    if (wallMaterial != null) {
        wall.GetComponent<Renderer>().material = wallMaterial;
    }
}
```

#### 学習者向け機能
- **即座の動作確認**: プレハブ作成不要
- **簡単セットアップ**: 3つのMaterialのみ設定
- **段階的学習**: 本格システムへの移行準備

---

## ?? 設計パターンの実装

### 1. シングルトンパターン
```csharp
// 実装例: GameManager
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

**使用理由**: ゲーム全体で一つのマネージャーインスタンスが必要

### 2. オブザーバーパターン
```csharp
// イベントシステム
public static System.Action<int> OnScoreChanged;
public static System.Action<GameState> OnStateChanged;

// 発火
OnScoreChanged?.Invoke(newScore);

// 購読
void Start() {
    GameManager.OnScoreChanged += UpdateScoreDisplay;
}

void OnDestroy() {
    GameManager.OnScoreChanged -= UpdateScoreDisplay;
}
```

**使用理由**: UI更新などの疎結合な連携を実現

### 3. ステートマシンパターン
```csharp
// 状態の定義
public enum GameState {
    MainMenu, Playing, Paused, GameOver, Victory
}

// 状態遷移
public void ChangeState(GameState newState) {
    ExitState(currentState);
    currentState = newState;
    EnterState(newState);
}

// 状態別の処理
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

**使用理由**: 複雑な状態管理を整理して実装

### 4. オブジェクトプールパターン（部分実装）
```csharp
// 効果音の同時再生制御
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

**使用理由**: メモリ効率とパフォーマンスの向上

---

## ?? パフォーマンス最適化

### 1. 更新頻度の最適化
```csharp
// UI更新を必要時のみ実行
private int lastScore = -1;
void Update() {
    if (GameManager.Instance.score != lastScore) {
        UpdateScoreDisplay();
        lastScore = GameManager.Instance.score;
    }
}
```

### 2. オブジェクト生成の最適化
```csharp
// 迷路生成時の効率化
public void GenerateMaze() {
    ClearMaze();  // 既存オブジェクトクリア
    
    // 一括生成で処理負荷を軽減
    for (int y = 0; y < mazeHeight; y++) {
        for (int x = 0; x < mazeWidth; x++) {
            CreateMazeElement(x, y);
        }
    }
}
```

### 3. メモリリーク対策
```csharp
// イベント購読の適切な解除
void OnDestroy() {
    // 全てのイベント購読を解除
    GameManager.OnScoreChanged -= UpdateScoreDisplay;
    GameManager.OnStateChanged -= UpdateGameState;
}
```

---

## ?? 技術的な課題と解決策

### 課題1: プレハブ依存の解決
**問題**: 学習者がプレハブ作成に時間を要する  
**解決**: SimpleMazeGeneratorによる動的生成システム

### 課題2: 複雑な状態管理
**問題**: ゲーム状態の管理が複雑  
**解決**: ステートマシンパターンによる整理

### 課題3: UI更新の効率化
**問題**: 毎フレームUI更新による負荷  
**解決**: 変更検出による条件付き更新

### 課題4: 音響システムの最適化
**問題**: 同時再生による音響品質の劣化  
**解決**: 音源制限とプールシステム

---

## ?? コードメトリクス

### ファイル別行数
- **MazeGenerator.cs**: 324行
- **PacmanController.cs**: 298行
- **GhostController.cs**: 456行
- **GameManager.cs**: 387行
- **UIManager.cs**: 213行
- **AudioManager.cs**: 267行
- **CameraController.cs**: 189行
- **PowerPellet.cs**: 134行
- **Dot.cs**: 89行
- **SimpleMazeGenerator.cs**: 156行

### 複雑度メトリクス
- **平均メソッド長**: 15行
- **最大ネスト深度**: 4レベル
- **クラス結合度**: 低（疎結合設計）
- **コメント密度**: 約40%

---

## ?? 学習者向け実装ガイド

### 1. 基本的な実装順序
1. **GameObject作成** → 基本的なオブジェクト配置
2. **スクリプト実装** → 機能の段階的追加
3. **連携システム** → コンポーネント間の協調
4. **最適化** → パフォーマンス改善

### 2. よくあるエラーとその対処
```csharp
// NullReferenceException対策
if (gameObject != null && gameObject.activeInHierarchy) {
    // 安全な処理
}

// 重複実行防止
if (isProcessing) return;
isProcessing = true;
```

### 3. デバッグ支援機能
```csharp
[System.Diagnostics.Conditional("UNITY_EDITOR")]
public void DebugLog(string message) {
    Debug.Log($"[{GetType().Name}] {message}");
}
```

---

## ?? 今後の技術的改善予定

### 1. アーキテクチャの改善
- **依存性注入**: より柔軟なコンポーネント連携
- **SOLID原則**: より保守性の高い設計
- **テスト駆動開発**: 単体テストの実装

### 2. パフォーマンスの向上
- **オブジェクトプール**: 完全な実装
- **LODシステム**: 距離に応じた詳細度制御
- **プロファイリング**: 定期的な性能測定

### 3. 機能の拡張
- **レベルエディター**: 迷路の動的編集
- **AI改善**: より高度な敵AI
- **ネットワーク**: マルチプレイヤー対応

---

このテクニカルログは、開発者と学習者の両方に向けた技術的な参考資料として継続的に更新されます。 ???