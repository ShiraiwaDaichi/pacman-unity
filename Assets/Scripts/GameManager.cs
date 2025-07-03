// ===============================
// ゲームマネージャー
// ===============================
// このスクリプトは、パックマンゲーム全体の管理を行います。
// スコア、ライフ、パワーモード、勝利条件、敗北条件などを管理し、
// ゲームの状態を制御します。

using UnityEngine;
using UnityEngine.UI;           // UI要素（Text、Buttonなど）を使用するために必要
using System.Collections;       // コルーチン（IEnumerator）を使用するために必要

// MonoBehaviourを継承することで、UnityのGameObjectにアタッチできるコンポーネントになります
public class GameManager : MonoBehaviour
{
    // ===============================
    // ゲーム設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Game Settings")]
    public int score = 0;                    // プレイヤーのスコア
    public int lives = 3;                    // プレイヤーの残りライフ（命）
    public float powerModeTime = 10f;        // パワーモードの持続時間（秒）
    
    // ===============================
    // UI要素への参照（Unityエディターで設定）
    // ===============================
    
    [Header("UI References")]
    public Text scoreText;                   // スコアを表示するテキスト
    public Text livesText;                   // ライフを表示するテキスト
    public Text gameOverText;                // ゲームオーバー時に表示するテキスト
    public Text winText;                     // 勝利時に表示するテキスト
    public Button restartButton;             // リスタートボタン
    
    // ===============================
    // オーディオ設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Audio")]
    public AudioSource gameAudioSource;      // ゲーム音声を再生するコンポーネント
    public AudioClip gameOverSound;          // ゲームオーバー時の音
    public AudioClip winSound;               // 勝利時の音
    public AudioClip powerModeSound;         // パワーモード開始時の音
    
    // ===============================
    // プライベート変数（内部状態管理）
    // ===============================
    
    private bool isPowerModeActive = false;  // パワーモードが有効かどうか
    private bool isGameOver = false;         // ゲームオーバー状態かどうか
    private bool isGameWon = false;          // ゲームに勝利したかどうか
    
    // 他のスクリプトへの参照
    private PacmanController pacmanController;  // パックマンコントローラーへの参照
    private GhostController[] ghosts;          // 全てのゴーストへの参照（配列）
    
    // ドット収集の管理
    private int totalDots;                     // ゲーム内の総ドット数
    private int collectedDots = 0;             // 収集したドット数
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    // Start()は、オブジェクトが作成された最初のフレームで一度だけ呼び出されます
    void Start()
    {
        // 必要なコンポーネントの参照を取得
        pacmanController = FindObjectOfType<PacmanController>();  // PacmanControllerスクリプトを探す
        ghosts = FindObjectsOfType<GhostController>();           // 全てのGhostControllerスクリプトを探す
        
        CountTotalDots();  // ゲーム内の総ドット数を数える
        UpdateUI();        // UI表示を更新
        
        // ゲーム開始時は、ゲームオーバーテキストと勝利テキストを非表示にする
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
            
        if (winText != null)
            winText.gameObject.SetActive(false);
            
        // リスタートボタンの設定
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);           // 最初は非表示
            restartButton.onClick.AddListener(RestartGame);      // ボタンクリック時の処理を設定
        }
    }
    
    // Update()は、毎フレーム呼び出されます
    void Update()
    {
        // Rキーが押されたらゲームをリスタート
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    
    // ===============================
    // ドット管理メソッド
    // ===============================
    
    // ゲーム内の総ドット数を数えます
    void CountTotalDots()
    {
        // GameObject.FindGameObjectsWithTagで、指定されたタグの全オブジェクトを取得
        GameObject[] dots = GameObject.FindGameObjectsWithTag("Dot");
        GameObject[] powerPellets = GameObject.FindGameObjectsWithTag("PowerPellet");
        
        // 通常のドットとパワーペレットの合計数を記録
        totalDots = dots.Length + powerPellets.Length;
    }
    
    // ===============================
    // スコア管理メソッド
    // ===============================
    
    // スコアを追加します（パブリックメソッド - 他のスクリプトから呼び出し可能）
    public void AddScore(int points)
    {
        score += points;  // スコアに点数を追加
        UpdateUI();       // UI表示を更新
    }
    
    // ===============================
    // パワーモード管理メソッド
    // ===============================
    
    // パワーモードを開始します（パブリックメソッド）
    public void ActivatePowerMode()
    {
        // 既にパワーモードが有効な場合は何もしない
        if (isPowerModeActive) return;
        
        // コルーチンを開始（時間経過の処理）
        StartCoroutine(PowerModeCoroutine());
    }
    
    // パワーモードの時間管理を行うコルーチン
    // コルーチンは、処理を途中で一時停止し、後で再開できる特殊なメソッドです
    IEnumerator PowerModeCoroutine()
    {
        isPowerModeActive = true;  // パワーモードを有効にする
        
        // パワーモード開始音を再生
        if (gameAudioSource != null && powerModeSound != null)
        {
            gameAudioSource.PlayOneShot(powerModeSound);
        }
        
        // 全てのゴーストを怖がらせる（逃走状態にする）
        foreach (GhostController ghost in ghosts)
        {
            if (ghost != null)  // ゴーストが存在する場合のみ
            {
                ghost.SetScared(true);  // 怖がった状態にする
            }
        }
        
        // 指定された時間だけ待機（パワーモード持続時間）
        yield return new WaitForSeconds(powerModeTime);
        
        isPowerModeActive = false;  // パワーモードを終了
        
        // 全てのゴーストを通常状態に戻す
        foreach (GhostController ghost in ghosts)
        {
            if (ghost != null)
            {
                ghost.SetScared(false);  // 通常状態に戻す
            }
        }
    }
    
    // パワーモードが有効かどうかを確認します（パブリックメソッド）
    public bool IsPowerModeActive()
    {
        return isPowerModeActive;
    }
    
    // ===============================
    // ゲームオーバー処理メソッド
    // ===============================
    
    // ゲームオーバー処理を行います（パブリックメソッド）
    public void GameOver()
    {
        // 既にゲームオーバー状態の場合は何もしない
        if (isGameOver) return;
        
        isGameOver = true;  // ゲームオーバー状態にする
        lives--;           // ライフを1減らす
        
        // ライフが0以下になった場合
        if (lives <= 0)
        {
            // 完全なゲームオーバー
            if (gameAudioSource != null && gameOverSound != null)
            {
                gameAudioSource.PlayOneShot(gameOverSound);  // ゲームオーバー音を再生
            }
            
            // ゲームオーバーテキストを表示
            if (gameOverText != null)
            {
                gameOverText.gameObject.SetActive(true);
            }
            
            // リスタートボタンを表示
            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }
            
            // ゲームの時間を停止（Time.timeScale = 0で全ての時間処理が止まる）
            Time.timeScale = 0f;
        }
        else
        {
            // ライフが残っている場合は、レベルを再開
            StartCoroutine(RestartLevel());
        }
        
        UpdateUI();  // UI表示を更新
    }
    
    // レベルを再開するコルーチン
    IEnumerator RestartLevel()
    {
        // 2秒待機（プレイヤーが状況を理解する時間を与える）
        yield return new WaitForSeconds(2f);
        
        // パックマンとゴーストの位置をリセット
        if (pacmanController != null)
        {
            pacmanController.ResetPosition();
        }
        
        foreach (GhostController ghost in ghosts)
        {
            if (ghost != null)
            {
                ghost.ResetPosition();
            }
        }
        
        isGameOver = false;  // ゲームオーバー状態を解除
    }
    
    // ===============================
    // 勝利条件チェックメソッド
    // ===============================
    
    // 勝利条件をチェックします（パブリックメソッド）
    public void CheckWinCondition()
    {
        collectedDots++;  // 収集したドット数を増やす
        
        // 全てのドットを収集した場合
        if (collectedDots >= totalDots)
        {
            WinGame();  // 勝利処理を実行
        }
    }
    
    // 勝利処理を行います
    void WinGame()
    {
        // 既に勝利状態の場合は何もしない
        if (isGameWon) return;
        
        isGameWon = true;  // 勝利状態にする
        
        // 勝利音を再生
        if (gameAudioSource != null && winSound != null)
        {
            gameAudioSource.PlayOneShot(winSound);
        }
        
        // 勝利テキストを表示
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
        }
        
        // リスタートボタンを表示
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
        }
        
        // ゲームの時間を停止
        Time.timeScale = 0f;
    }
    
    // ===============================
    // UI更新メソッド
    // ===============================
    
    // UI表示を更新します
    void UpdateUI()
    {
        // スコアテキストを更新
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        
        // ライフテキストを更新
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
    }
    
    // ===============================
    // ゲームリスタートメソッド
    // ===============================
    
    // ゲームを再開します（パブリックメソッド）
    public void RestartGame()
    {
        Time.timeScale = 1f;  // ゲームの時間を正常に戻す
        
        // 現在のシーンを再読み込み（ゲームを最初からやり直す）
        // SceneManagerを使用してシーンをリロード
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}