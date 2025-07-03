// ===============================
// UIマネージャー
// ===============================
// このスクリプトは、ゲームのユーザーインターフェース（UI）を管理します。
// スコア表示、ライフ表示、ゲームオーバー画面、勝利画面、ポーズ機能などを制御し、
// プレイヤーに必要な情報を提供します。

using UnityEngine;
using UnityEngine.UI;  // UI要素（Text、Button、Panelなど）を使用するために必要

// MonoBehaviourを継承することで、UnityのGameObjectにアタッチできるコンポーネントになります
public class UIManager : MonoBehaviour
{
    // ===============================
    // UI要素への参照（Unityエディターで設定が必要）
    // ===============================
    
    [Header("UI Elements")]
    public Text scoreText;          // スコアを表示するテキスト
    public Text livesText;          // 残りライフを表示するテキスト
    public Text levelText;          // 現在のレベルを表示するテキスト
    public Text gameOverText;       // ゲームオーバー時に表示するテキスト
    public Text winText;            // 勝利時に表示するテキスト
    public Button restartButton;    // ゲーム再開ボタン
    public Button pauseButton;      // ポーズボタン
    
    // ===============================
    // パネル要素への参照（Unityエディターで設定が必要）
    // ===============================
    // パネルは、複数のUI要素をグループ化するためのコンテナです
    
    [Header("Panels")]
    public GameObject gameOverPanel;  // ゲームオーバー画面のパネル
    public GameObject winPanel;       // 勝利画面のパネル
    public GameObject pausePanel;     // ポーズ画面のパネル
    
    // ===============================
    // プライベート変数（内部状態管理）
    // ===============================
    
    private GameManager gameManager;  // ゲームマネージャーへの参照
    private bool isPaused = false;    // ゲームがポーズ中かどうか
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    // Start()は、オブジェクトが作成された最初のフレームで一度だけ呼び出されます
    void Start()
    {
        // GameManagerスクリプトを探して参照を取得
        gameManager = FindObjectOfType<GameManager>();
        
        // リスタートボタンのクリックイベントを設定
        if (restartButton != null)
        {
            // AddListenerで、ボタンがクリックされた時の処理を設定
            restartButton.onClick.AddListener(RestartGame);
        }
        
        // ポーズボタンのクリックイベントを設定
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
        
        // ゲーム開始時は各パネルを非表示にする
        // SetActive(false)で、GameObjectを非アクティブ（非表示）にします
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        if (winPanel != null)
            winPanel.SetActive(false);
            
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
    
    // Update()は、毎フレーム呼び出されます
    void Update()
    {
        // Escapeキーが押された時にポーズを切り替える
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    // ===============================
    // UI更新メソッド（パブリック - 他のスクリプトから呼び出し可能）
    // ===============================
    
    // スコア表示を更新します
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            // ToString()で、数値を文字列に変換
            scoreText.text = "Score: " + score.ToString();
        }
    }
    
    // ライフ表示を更新します
    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives.ToString();
        }
    }
    
    // レベル表示を更新します
    public void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level.ToString();
        }
    }
    
    // ===============================
    // ゲーム状態表示メソッド
    // ===============================
    
    // ゲームオーバー画面を表示します
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);  // パネルを表示
        }
        
        if (gameOverText != null)
        {
            gameOverText.text = "Game Over!";
        }
    }
    
    // 勝利画面を表示します
    public void ShowWin()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);  // パネルを表示
        }
        
        if (winText != null)
        {
            winText.text = "You Win!";
        }
    }
    
    // ===============================
    // ポーズ機能メソッド
    // ===============================
    
    // ポーズ状態を切り替えます
    public void TogglePause()
    {
        // !演算子で、boolean値を反転させる（true → false, false → true）
        isPaused = !isPaused;
        
        if (isPaused)
        {
            // ポーズ状態にする
            Time.timeScale = 0f;  // ゲームの時間を停止（0倍にする）
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);  // ポーズパネルを表示
            }
        }
        else
        {
            // ポーズを解除する
            Time.timeScale = 1f;  // ゲームの時間を通常に戻す（1倍にする）
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);  // ポーズパネルを非表示
            }
        }
    }
    
    // ===============================
    // ゲーム制御メソッド
    // ===============================
    
    // ゲームを再開します（パブリックメソッド）
    public void RestartGame()
    {
        Time.timeScale = 1f;  // 時間スケールを正常に戻す
        
        // GameManagerが存在する場合、リスタート処理を実行
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
    
    // ゲームを終了します（パブリックメソッド）
    public void QuitGame()
    {
        // Application.Quit()で、ゲームアプリケーションを終了
        // 注意：Unityエディターでは動作しません（ビルドされたゲームでのみ有効）
        Application.Quit();
        
        // Unityエディターでテストする場合は、以下のコードを追加できます：
        // #if UNITY_EDITOR
        // UnityEditor.EditorApplication.isPlaying = false;
        // #endif
    }
}