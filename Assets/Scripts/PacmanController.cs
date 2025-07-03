// ===============================
// パックマンコントローラー
// ===============================
// このスクリプトは、プレイヤーが操作するパックマンキャラクターの動作を制御します。
// 移動、回転、アイテムの収集、ゴーストとの衝突処理などを行います。

using UnityEngine;

// MonoBehaviourを継承することで、UnityのGameObjectにアタッチできるコンポーネントになります
public class PacmanController : MonoBehaviour
{
    // ===============================
    // パブリック変数（Unityエディターで設定可能）
    // ===============================
    
    // [Header] 属性を使用すると、Unityエディターでグループ化して表示されます
    [Header("Movement Settings")]
    public float moveSpeed = 5f;           // パックマンの移動速度（1秒間に移動する距離）
    public LayerMask obstacleLayer;        // 障害物のレイヤー（壁など、移動を妨げるオブジェクト）
    
    [Header("Animation")]
    public Animator animator;              // アニメーションを制御するAnimatorコンポーネント
    
    // ===============================
    // プライベート変数（このスクリプト内でのみ使用）
    // ===============================
    
    // Vector2は2次元のベクトル（x,y座標）を表します
    private Vector2 currentDirection = Vector2.zero;  // 現在の移動方向（0,0は静止状態）
    private Vector2 nextDirection = Vector2.zero;     // 次に移動したい方向
    private Vector2 startPosition;                    // ゲーム開始時の位置
    private bool isMoving = false;                    // 移動中かどうかの状態
    
    [Header("Audio")]
    public AudioSource audioSource;       // 音声を再生するコンポーネント
    public AudioClip dotSound;           // ドットを食べた時の音
    public AudioClip powerPelletSound;   // パワーペレットを食べた時の音
    
    // 他のスクリプトへの参照
    private GameManager gameManager;      // ゲーム全体を管理するスクリプト
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    // Start()は、オブジェクトが作成された最初のフレームで一度だけ呼び出されます
    void Start()
    {
        // 初期設定を行います
        startPosition = transform.position;                    // 開始位置を記録
        gameManager = FindObjectOfType<GameManager>();        // GameManagerスクリプトを探して参照を取得
        
        // AudioSourceコンポーネントが設定されていない場合、自動的に取得を試みます
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
            
        // Animatorコンポーネントが設定されていない場合、自動的に取得を試みます
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    // Update()は、毎フレーム（通常1秒間に60回）呼び出されます
    void Update()
    {
        HandleInput();  // プレイヤーの入力を処理
        Move();         // パックマンの移動を処理
        
        // アニメーターが設定されている場合、移動状態を更新
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }
    }
    
    // ===============================
    // 入力処理メソッド
    // ===============================
    
    // プレイヤーのキーボード入力を処理します
    void HandleInput()
    {
        // Input.GetKeyDown()は、キーが押された瞬間にtrueを返します
        // 矢印キーまたはWASDキーで移動方向を設定
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            nextDirection = Vector2.up;      // 上方向のベクトル（0, 1）
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            nextDirection = Vector2.down;    // 下方向のベクトル（0, -1）
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            nextDirection = Vector2.left;    // 左方向のベクトル（-1, 0）
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            nextDirection = Vector2.right;   // 右方向のベクトル（1, 0）
        }
    }
    
    // ===============================
    // 移動処理メソッド
    // ===============================
    
    // パックマンの移動を処理します
    void Move()
    {
        // 次の方向に移動可能かどうかをチェック
        // Vector2.zeroは（0,0）を表し、方向が設定されていない状態を意味します
        if (nextDirection != Vector2.zero && CanMove(nextDirection))
        {
            currentDirection = nextDirection;     // 現在の方向を更新
            nextDirection = Vector2.zero;         // 次の方向をリセット
        }
        
        // 現在の方向に移動可能かどうかをチェック
        if (currentDirection != Vector2.zero && CanMove(currentDirection))
        {
            // transform.Translateは、オブジェクトの位置を移動させます
            // currentDirection * moveSpeed * Time.deltaTimeで、方向・速度・時間を考慮した移動量を計算
            // Time.deltaTimeは前フレームからの経過時間（通常1/60秒）
            transform.Translate(currentDirection * moveSpeed * Time.deltaTime);
            isMoving = true;
            
            // パックマンの向きを移動方向に合わせて回転
            UpdateRotation();
        }
        else
        {
            isMoving = false;  // 移動できない場合は停止状態に
        }
    }
    
    // 指定された方向に移動可能かどうかを判定します
    bool CanMove(Vector2 direction)
    {
        // Physics2D.Raycastは、指定された方向に向かって「見えない光線」を発射し、
        // 何かにぶつかるかどうかを調べます（レイキャストと呼ばれる技術）
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,  // 光線の開始位置（パックマンの現在位置）
            direction,          // 光線の方向
            0.6f,              // 光線の長さ（0.6ユニット先まで調べる）
            obstacleLayer      // 調べる対象のレイヤー（壁など）
        );
        
        // hit.colliderがnullの場合、何にもぶつからなかったということで移動可能
        return hit.collider == null;
    }
    
    // パックマンの向きを移動方向に合わせて回転させます
    void UpdateRotation()
    {
        // Mathf.Atan2は、ベクトルの角度をラジアンで取得します
        // Mathf.Rad2Degでラジアンを度数に変換
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        
        // Quaternion.AngleAxisで指定された角度の回転を作成し、オブジェクトに適用
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    // ===============================
    // 衝突処理メソッド
    // ===============================
    
    // 他のオブジェクトとの衝突を検出します（トリガーコライダーが必要）
    void OnTriggerEnter2D(Collider2D other)
    {
        // other.CompareTagは、衝突した相手のタグを調べます
        if (other.CompareTag("Dot"))
        {
            CollectDot(other.gameObject);  // ドットを収集
        }
        else if (other.CompareTag("PowerPellet"))
        {
            CollectPowerPellet(other.gameObject);  // パワーペレットを収集
        }
        else if (other.CompareTag("Ghost"))
        {
            // GetComponentで、衝突したゴーストのGhostControllerスクリプトを取得
            HandleGhostCollision(other.GetComponent<GhostController>());
        }
    }
    
    // ドットを収集する処理
    void CollectDot(GameObject dot)
    {
        // 音声が設定されている場合、ドットを食べた音を再生
        if (audioSource != null && dotSound != null)
        {
            audioSource.PlayOneShot(dotSound);  // PlayOneShotは一度だけ音を再生
        }
        
        // Destroyでドットオブジェクトをゲームから削除
        Destroy(dot);
        
        // GameManagerが存在する場合、スコアを追加し勝利条件をチェック
        if (gameManager != null)
        {
            gameManager.AddScore(10);              // 10点追加
            gameManager.CheckWinCondition();      // 勝利条件（全ドット収集）をチェック
        }
    }
    
    // パワーペレットを収集する処理
    void CollectPowerPellet(GameObject powerPellet)
    {
        // 音声が設定されている場合、パワーペレットを食べた音を再生
        if (audioSource != null && powerPelletSound != null)
        {
            audioSource.PlayOneShot(powerPelletSound);
        }
        
        // パワーペレットオブジェクトをゲームから削除
        Destroy(powerPellet);
        
        // GameManagerが存在する場合、スコアを追加しパワーモードを開始
        if (gameManager != null)
        {
            gameManager.AddScore(50);                 // 50点追加
            gameManager.ActivatePowerMode();          // パワーモード開始
        }
    }
    
    // ゴーストとの衝突を処理
    void HandleGhostCollision(GhostController ghost)
    {
        if (gameManager != null)
        {
            // パワーモードが有効な場合
            if (gameManager.IsPowerModeActive())
            {
                // ゴーストを食べることができる
                gameManager.AddScore(200);    // 200点追加
                ghost.GetEaten();            // ゴーストを食べられた状態にする
            }
            else
            {
                // パワーモードでない場合はゲームオーバー
                gameManager.GameOver();
            }
        }
    }
    
    // ===============================
    // パブリックメソッド（他のスクリプトから呼び出し可能）
    // ===============================
    
    // パックマンの位置と状態をリセットします
    public void ResetPosition()
    {
        transform.position = startPosition;        // 開始位置に戻す
        currentDirection = Vector2.zero;           // 移動方向をリセット
        nextDirection = Vector2.zero;              // 次の移動方向をリセット
        isMoving = false;                         // 移動状態をリセット
    }
}