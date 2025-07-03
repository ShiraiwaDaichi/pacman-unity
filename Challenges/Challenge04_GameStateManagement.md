# 課題04: ゲーム状態管理システム

## ?? 学習目標
- enum（列挙型）を使った状態管理
- シングルトンパターンの基礎理解
- ゲームループとステートマシン
- UI との連携システム
- イベントシステムの実装

## ? 推定所要時間
約 75-90 分

## ?? 前提知識
- 課題03の完了
- enum の基本概念
- static キーワードの理解
- public/private の使い分け

## ?? 課題内容

### ステップ1: UIキャンバスの準備

#### 1.1 UIの作成
1. Hierarchy で右クリック → UI → Canvas を作成
2. Canvas 下に UI → Text を3つ作成：
   - "ScoreText"
   - "StatusText" 
   - "TimerText"
3. 画面上に適切に配置

#### 1.2 ボタンの追加
1. Canvas 下に UI → Button を2つ作成：
   - "StartButton"
   - "ResetButton"

### ステップ2: ゲーム状態管理スクリプトの作成

以下のスクリプトを **自分で入力** してください：

```csharp
using UnityEngine;
using UnityEngine.UI;

// ゲーム全体の状態を管理するクラス
public class GameStateManager : MonoBehaviour
{
    // ===============================
    // ゲーム状態の定義（enum）
    // ===============================
    
    // enumは関連する定数をグループ化するためのデータ型です
    public enum GameState
    {
        MainMenu,    // メインメニュー状態
        Playing,     // ゲーム中
        Paused,      // ポーズ中
        GameOver,    // ゲームオーバー
        Victory      // 勝利
    }
    
    // ===============================
    // シングルトンパターンの実装
    // ===============================
    
    // シングルトンパターン：このクラスのインスタンスを1つだけに制限
    public static GameStateManager Instance { get; private set; }
    
    // ===============================
    // パブリック変数（設定可能）
    // ===============================
    
    [Header("ゲーム設定")]
    public GameState currentState = GameState.MainMenu;    // 現在の状態
    public float gameTimeLimit = 60.0f;                   // ゲーム時間制限（秒）
    public int targetScore = 100;                         // 目標スコア
    public bool useTimeLimit = true;                      // 時間制限を使用するか
    
    [Header("UI参照")]
    public Text scoreText;                                // スコア表示用テキスト
    public Text statusText;                               // 状態表示用テキスト
    public Text timerText;                                // タイマー表示用テキスト
    public Button startButton;                            // スタートボタン
    public Button resetButton;                            // リセットボタン
    
    [Header("ゲームデータ")]
    public int currentScore = 0;                          // 現在のスコア
    public int lives = 3;                                 // 残りライフ
    public float gameTimer = 0f;                          // ゲームタイマー
    
    // ===============================
    // プライベート変数（内部管理）
    // ===============================
    
    private GameState previousState;                      // 前の状態
    private float stateChangeTime;                        // 状態変更時刻
    private bool isGameActive = false;                    // ゲームがアクティブか
    
    // 統計データ
    private float totalPlayTime = 0f;
    private int gamesPlayed = 0;
    private int highScore = 0;
    
    // ===============================
    // イベントシステム（C#のAction）
    // ===============================
    
    // 他のスクリプトが状態変化を監視できるようにするイベント
    public static System.Action<GameState> OnStateChanged;
    public static System.Action<int> OnScoreChanged;
    public static System.Action<int> OnLivesChanged;
    public static System.Action OnGameWon;
    public static System.Action OnGameLost;
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // シーン変更で破棄されないようにする
        }
        else
        {
            Destroy(gameObject);  // 既にインスタンスが存在する場合は削除
            return;
        }
    }
    
    void Start()
    {
        // 初期設定
        SetupUI();
        ChangeState(GameState.MainMenu);
        
        Debug.Log("=== ゲーム状態管理システム開始 ===");
        Debug.Log("初期状態: " + currentState);
        
        // ボタンイベントの設定
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetGame);
    }
    
    void Update()
    {
        // 現在の状態に応じた処理
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
        
        // UI更新
        UpdateUI();
        
        // 特殊キー処理
        HandleInput();
    }
    
    // ===============================
    // 状態管理メソッド
    // ===============================
    
    // 状態を変更する主要メソッド
    public void ChangeState(GameState newState)
    {
        // 前の状態を記録
        previousState = currentState;
        
        // 現在の状態を終了
        ExitState(currentState);
        
        // 新しい状態に変更
        currentState = newState;
        stateChangeTime = Time.time;
        
        // 新しい状態を開始
        EnterState(newState);
        
        // イベント通知
        OnStateChanged?.Invoke(newState);
        
        Debug.Log($"状態変更: {previousState} → {currentState}");
    }
    
    // 状態に入る時の処理
    void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                Debug.Log("メインメニューに入りました");
                isGameActive = false;
                break;
                
            case GameState.Playing:
                Debug.Log("ゲーム開始！");
                isGameActive = true;
                gameTimer = useTimeLimit ? gameTimeLimit : 0f;
                break;
                
            case GameState.Paused:
                Debug.Log("ゲームポーズ");
                Time.timeScale = 0f;  // ゲーム時間を停止
                break;
                
            case GameState.GameOver:
                Debug.Log("ゲームオーバー");
                HandleGameEnd(false);
                break;
                
            case GameState.Victory:
                Debug.Log("勝利！");
                HandleGameEnd(true);
                break;
        }
    }
    
    // 状態から出る時の処理
    void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                totalPlayTime += Time.time - stateChangeTime;
                break;
                
            case GameState.Paused:
                Time.timeScale = 1f;  // ゲーム時間を再開
                break;
        }
    }
    
    // ===============================
    // 各状態の更新処理
    // ===============================
    
    void UpdateMainMenu()
    {
        // メインメニューでの処理
        // 例：タイトル画面のアニメーション、音楽など
    }
    
    void UpdatePlaying()
    {
        // ゲーム中の処理
        
        // タイマー更新
        if (useTimeLimit)
        {
            gameTimer -= Time.deltaTime;
            
            // 時間切れチェック
            if (gameTimer <= 0f)
            {
                gameTimer = 0f;
                ChangeState(GameState.GameOver);
                return;
            }
        }
        else
        {
            gameTimer += Time.deltaTime;  // 経過時間をカウント
        }
        
        // 勝利条件チェック
        if (currentScore >= targetScore)
        {
            ChangeState(GameState.Victory);
        }
        
        // ライフ切れチェック
        if (lives <= 0)
        {
            ChangeState(GameState.GameOver);
        }
    }
    
    void UpdatePaused()
    {
        // ポーズ中の処理
        // 例：ポーズメニューの表示、設定変更など
    }
    
    void UpdateGameOver()
    {
        // ゲームオーバー状態の処理
        // 例：ゲームオーバー画面の表示、スコア保存など
    }
    
    void UpdateVictory()
    {
        // 勝利状態の処理
        // 例：勝利演出、次のレベルへの移行など
    }
    
    // ===============================
    // ゲームロジック処理
    // ===============================
    
    // スコアを追加
    public void AddScore(int points)
    {
        if (currentState != GameState.Playing) return;
        
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
        
        // ハイスコア更新
        if (currentScore > highScore)
        {
            highScore = currentScore;
            Debug.Log("新ハイスコア: " + highScore);
        }
        
        Debug.Log($"スコア追加: +{points} (合計: {currentScore})");
    }
    
    // ライフを減らす
    public void LoseLife()
    {
        if (currentState != GameState.Playing) return;
        
        lives--;
        OnLivesChanged?.Invoke(lives);
        
        Debug.Log($"ライフ減少 (残り: {lives})");
        
        if (lives <= 0)
        {
            ChangeState(GameState.GameOver);
        }
    }
    
    // ライフを増やす
    public void GainLife()
    {
        lives++;
        OnLivesChanged?.Invoke(lives);
        Debug.Log($"ライフ獲得 (合計: {lives})");
    }
    
    // ゲーム開始
    public void StartGame()
    {
        if (currentState == GameState.MainMenu || currentState == GameState.GameOver || currentState == GameState.Victory)
        {
            ResetGameData();
            ChangeState(GameState.Playing);
            gamesPlayed++;
        }
    }
    
    // ゲームリセット
    public void ResetGame()
    {
        ResetGameData();
        ChangeState(GameState.MainMenu);
        Debug.Log("ゲームリセット完了");
    }
    
    // ゲームデータをリセット
    void ResetGameData()
    {
        currentScore = 0;
        lives = 3;
        gameTimer = useTimeLimit ? gameTimeLimit : 0f;
        isGameActive = false;
    }
    
    // ゲーム終了処理
    void HandleGameEnd(bool won)
    {
        isGameActive = false;
        
        if (won)
        {
            OnGameWon?.Invoke();
            Debug.Log("=== 勝利！ ===");
        }
        else
        {
            OnGameLost?.Invoke();
            Debug.Log("=== 敗北 ===");
        }
        
        Debug.Log($"最終スコア: {currentScore}");
        Debug.Log($"プレイ時間: {totalPlayTime:F1}秒");
    }
    
    // ===============================
    // 入力処理
    // ===============================
    
    void HandleInput()
    {
        // ESCキーでポーズ/ポーズ解除
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
        
        // Spaceキーでゲーム開始/再開
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
        
        // Rキーでリセット
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
        
        // デバッグ用：状態情報表示
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowGameInfo();
        }
    }
    
    // ===============================
    // UI管理
    // ===============================
    
    void SetupUI()
    {
        // UIの初期設定
        UpdateUI();
    }
    
    void UpdateUI()
    {
        // スコア表示
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore} / {targetScore}";
        }
        
        // 状態表示
        if (statusText != null)
        {
            string stateText = GetStateDisplayText();
            statusText.text = $"Status: {stateText} | Lives: {lives}";
        }
        
        // タイマー表示
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
        
        // ボタンの表示制御
        UpdateButtonStates();
    }
    
    // 状態に応じた表示テキストを取得
    string GetStateDisplayText()
    {
        return currentState switch
        {
            GameState.MainMenu => "メニュー",
            GameState.Playing => "プレイ中",
            GameState.Paused => "ポーズ",
            GameState.GameOver => "ゲームオーバー",
            GameState.Victory => "勝利！",
            _ => "不明"
        };
    }
    
    // ボタンの状態を更新
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
            resetButton.gameObject.SetActive(true);  // 常に表示
        }
    }
    
    // ===============================
    // 情報表示
    // ===============================
    
    // ゲーム情報を表示
    void ShowGameInfo()
    {
        Debug.Log("=== ゲーム情報 ===");
        Debug.Log($"現在状態: {currentState}");
        Debug.Log($"スコア: {currentScore} / {targetScore}");
        Debug.Log($"ライフ: {lives}");
        Debug.Log($"タイマー: {gameTimer:F1}秒");
        Debug.Log($"ハイスコア: {highScore}");
        Debug.Log($"ゲームプレイ回数: {gamesPlayed}");
        Debug.Log($"総プレイ時間: {totalPlayTime:F1}秒");
    }
    
    // ===============================
    // パブリックメソッド（外部から呼び出し可能）
    // ===============================
    
    // 現在の状態を取得
    public GameState GetCurrentState()
    {
        return currentState;
    }
    
    // ゲームがアクティブかチェック
    public bool IsGameActive()
    {
        return isGameActive;
    }
    
    // 残り時間を取得
    public float GetRemainingTime()
    {
        return useTimeLimit ? gameTimer : -1f;
    }
    
    // ハイスコアを取得
    public int GetHighScore()
    {
        return highScore;
    }
}
```

### ステップ3: 状態連携スクリプトの作成

```csharp
using UnityEngine;

// 他のスクリプトがゲーム状態に反応するための例
public class StateReactiveObject : MonoBehaviour
{
    [Header("状態反応設定")]
    public GameObject activeInGame;      // ゲーム中のみ表示
    public GameObject activeInMenu;      // メニュー中のみ表示
    
    void Start()
    {
        // 状態変化イベントを購読
        GameStateManager.OnStateChanged += HandleStateChange;
        GameStateManager.OnScoreChanged += HandleScoreChange;
    }
    
    void OnDestroy()
    {
        // イベント購読を解除（メモリリーク防止）
        GameStateManager.OnStateChanged -= HandleStateChange;
        GameStateManager.OnScoreChanged -= HandleScoreChange;
    }
    
    // 状態変化時の処理
    void HandleStateChange(GameStateManager.GameState newState)
    {
        Debug.Log($"状態変化を検知: {newState}");
        
        // オブジェクトの表示制御
        if (activeInGame != null)
        {
            activeInGame.SetActive(newState == GameStateManager.GameState.Playing);
        }
        
        if (activeInMenu != null)
        {
            activeInMenu.SetActive(newState == GameStateManager.GameState.MainMenu);
        }
    }
    
    // スコア変化時の処理
    void HandleScoreChange(int newScore)
    {
        Debug.Log($"スコア変化を検知: {newScore}");
        
        // スコアに応じた処理
        if (newScore >= 50)
        {
            // 特別な効果を発動
            Debug.Log("ハーフウェイボーナス！");
        }
    }
}
```

### ステップ4: 既存スクリプトとの連携

課題03のCollisionDetectorスクリプトに以下を追加：

```csharp
// CollisionDetectorクラスのHandleCollectibleCollisionメソッドを修正
void HandleCollectibleCollision(Collider other)
{
    // ゲームが有効な場合のみ処理
    if (GameStateManager.Instance != null && GameStateManager.Instance.IsGameActive())
    {
        GameStateManager.Instance.AddScore(itemValue);
        // 他の処理...
    }
}
```

## ?? 実験してみよう

### 実験1: カスタム状態の追加

```csharp
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Victory,
    Loading,     // 新しい状態
    Settings     // 新しい状態
}
```

### 実験2: 状態遷移の制限

```csharp
// 特定の状態からのみ遷移可能にする
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

### 実験3: セーブ/ロード機能

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

## ? よくあるエラー

### エラー1: シングルトンが機能しない
**原因**: Awake()での設定ミス
**解決方法**: DontDestroyOnLoad()の適切な使用

### エラー2: イベントのメモリリーク
**原因**: イベント購読の解除忘れ
**解決方法**: OnDestroy()での購読解除

### エラー3: UI参照エラー
**原因**: Inspector での参照設定忘れ
**解決方法**: null チェックの追加

## ?? 学習ポイント

### 重要な概念
1. **enum**: 状態の定義と管理
2. **Singleton Pattern**: 単一インスタンスの保証
3. **State Machine**: 状態遷移システム
4. **Event System**: イベント駆動プログラミング
5. **UI Integration**: UIとロジックの連携

### 設計パターン
- **State Pattern**: 状態に応じた処理の切り替え
- **Observer Pattern**: イベントシステムの基礎
- **Singleton Pattern**: グローバルアクセス

## ?? 次のステップ

この課題完了後：
1. **理解度チェック**: ステートマシンの概念説明
2. **実験**: 新しい状態の追加
3. **次の課題**: 課題05「UI作成とイベント処理」

## ?? 追加チャレンジ

### チャレンジ1: アニメーション連携
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

### チャレンジ2: 音響システム連携
```csharp
public AudioClip[] stateMusic;

void PlayStateMusic(GameState state)
{
    int index = (int)state;
    if (index < stateMusic.Length && stateMusic[index] != null)
    {
        // 音楽再生処理
    }
}
```

### チャレンジ3: 設定システム
```csharp
public class GameSettings
{
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public bool showDebugInfo = false;
    public KeyCode pauseKey = KeyCode.Escape;
}
```

素晴らしい！?? 
ゲーム状態管理システムを完成させました！