// ===============================
// ゴーストコントローラー
// ===============================
// このスクリプトは、ゴーストキャラクターのAI（人工知能）を制御します。
// ゴーストは4つの状態を持ち、それぞれ異なる行動を取ります：
// - Chase（追跡）：パックマンを追いかける
// - Scatter（散乱）：ランダムに移動する
// - Scared（恐怖）：パックマンから逃げる
// - Eaten（食べられた）：スタート地点に戻る

using UnityEngine;
using System.Collections;  // コルーチンを使用するために必要

// MonoBehaviourを継承することで、UnityのGameObjectにアタッチできるコンポーネントになります
public class GhostController : MonoBehaviour
{
    // ===============================
    // 移動設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Movement Settings")]
    public float moveSpeed = 3f;            // 通常の移動速度
    public float scaredSpeed = 1.5f;        // 恐怖状態時の移動速度（遅くなる）
    public LayerMask obstacleLayer;         // 障害物のレイヤー（壁など）
    
    // ===============================
    // AI設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("AI Settings")]
    public float chaseDistance = 5f;              // 追跡を開始する距離
    public float directionChangeInterval = 2f;    // 方向を変更する間隔（秒）
    
    // ===============================
    // 状態設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("States")]
    public GhostState currentState = GhostState.Chase;  // 現在の状態
    public Color normalColor = Color.red;               // 通常時の色
    public Color scaredColor = Color.blue;              // 恐怖状態時の色
    public Color eatenColor = Color.white;              // 食べられた時の色
    
    // ===============================
    // プライベート変数（内部状態管理）
    // ===============================
    
    private Vector2 currentDirection = Vector2.right;   // 現在の移動方向
    private Vector2 targetDirection;                    // 目標方向
    private Transform player;                           // プレイヤー（パックマン）への参照
    private Vector2 startPosition;                      // 開始位置
    private SpriteRenderer spriteRenderer;              // スプライトレンダラー（色を変更するため）
    private GameManager gameManager;                    // ゲームマネージャーへの参照
    
    private float directionTimer = 0f;                  // 方向変更タイマー
    private bool isScared = false;                      // 恐怖状態かどうか
    
    // ===============================
    // 列挙型（enum）- ゴーストの状態を定義
    // ===============================
    // enumは、関連する定数をグループ化するためのデータ型です
    public enum GhostState
    {
        Chase,   // 追跡状態：パックマンを追いかける
        Scatter, // 散乱状態：ランダムに移動する
        Scared,  // 恐怖状態：パックマンから逃げる
        Eaten    // 食べられた状態：スタート地点に戻る
    }
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    // Start()は、オブジェクトが作成された最初のフレームで一度だけ呼び出されます
    void Start()
    {
        // プレイヤーオブジェクトを探して参照を取得
        // ?. 演算子は、オブジェクトがnullでない場合のみプロパティにアクセスします
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // 必要なコンポーネントの参照を取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        
        // 開始位置を記録
        startPosition = transform.position;
        
        // 状態制御のコルーチンを開始
        StartCoroutine(StateController());
    }
    
    // Update()は、毎フレーム呼び出されます
    void Update()
    {
        Move();  // 移動処理
        
        // 方向変更タイマーを更新
        directionTimer += Time.deltaTime;
        
        // 一定時間経過したら方向を変更
        if (directionTimer >= directionChangeInterval)
        {
            ChooseDirection();
            directionTimer = 0f;  // タイマーをリセット
        }
    }
    
    // ===============================
    // 移動処理メソッド
    // ===============================
    
    // ゴーストの移動を処理します
    void Move()
    {
        float currentSpeed = GetCurrentSpeed();  // 現在の速度を取得
        
        // 現在の方向に移動可能かチェック
        if (CanMove(currentDirection))
        {
            // 移動を実行
            transform.Translate(currentDirection * currentSpeed * Time.deltaTime);
        }
        else
        {
            // 移動できない場合は新しい方向を選択
            ChooseDirection();
        }
    }
    
    // 現在の状態に応じた移動速度を取得します
    float GetCurrentSpeed()
    {
        // switch式を使用して、状態に応じた速度を返す
        return currentState switch
        {
            GhostState.Scared => scaredSpeed,        // 恐怖状態時は遅い
            GhostState.Eaten => moveSpeed * 2f,      // 食べられた状態時は速い
            _ => moveSpeed                           // その他の状態では通常速度
        };
    }
    
    // 指定された方向に移動可能かどうかを判定します
    bool CanMove(Vector2 direction)
    {
        // レイキャストを使用して、移動方向に障害物があるかチェック
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.6f, obstacleLayer);
        return hit.collider == null;  // 何にもぶつからない場合は移動可能
    }
    
    // ===============================
    // 方向選択メソッド
    // ===============================
    
    // 現在の状態に応じて移動方向を選択します
    void ChooseDirection()
    {
        // 可能な移動方向を定義
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 bestDirection = currentDirection;  // デフォルトは現在の方向
        
        // 現在の状態に応じて最適な方向を選択
        switch (currentState)
        {
            case GhostState.Chase:
                bestDirection = GetChaseDirection(directions);      // 追跡方向を取得
                break;
            case GhostState.Scatter:
                bestDirection = GetRandomDirection(directions);     // ランダム方向を取得
                break;
            case GhostState.Scared:
                bestDirection = GetScaredDirection(directions);     // 逃走方向を取得
                break;
            case GhostState.Eaten:
                bestDirection = GetReturnDirection(directions);     // 帰還方向を取得
                break;
        }
        
        // 選択した方向に移動可能な場合のみ、方向を更新
        if (CanMove(bestDirection))
        {
            currentDirection = bestDirection;
        }
    }
    
    // パックマンを追跡する方向を取得します
    Vector2 GetChaseDirection(Vector2[] directions)
    {
        // プレイヤーが存在しない場合はランダム方向を返す
        if (player == null) return GetRandomDirection(directions);
        
        // プレイヤーへの方向ベクトルを計算
        Vector2 playerDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        
        Vector2 bestDirection = currentDirection;
        float bestDot = -1f;  // 内積の最大値（-1～1の範囲）
        
        // 各方向について、プレイヤーへの方向との内積を計算
        foreach (Vector2 dir in directions)
        {
            if (CanMove(dir))
            {
                // Vector2.Dotは内積を計算（方向の類似度を測る）
                float dot = Vector2.Dot(dir, playerDirection);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDirection = dir;
                }
            }
        }
        
        return bestDirection;
    }
    
    // パックマンから逃げる方向を取得します
    Vector2 GetScaredDirection(Vector2[] directions)
    {
        // プレイヤーが存在しない場合はランダム方向を返す
        if (player == null) return GetRandomDirection(directions);
        
        // プレイヤーへの方向ベクトルを計算
        Vector2 playerDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        
        Vector2 bestDirection = currentDirection;
        float bestDot = 1f;  // 内積の最小値を探す
        
        // 各方向について、プレイヤーへの方向との内積を計算
        foreach (Vector2 dir in directions)
        {
            if (CanMove(dir))
            {
                float dot = Vector2.Dot(dir, playerDirection);
                // 内積が小さい（逆方向に近い）方向を選択
                if (dot < bestDot)
                {
                    bestDot = dot;
                    bestDirection = dir;
                }
            }
        }
        
        return bestDirection;
    }
    
    // スタート地点に戻る方向を取得します
    Vector2 GetReturnDirection(Vector2[] directions)
    {
        // スタート地点への方向ベクトルを計算
        Vector2 homeDirection = (startPosition - (Vector2)transform.position).normalized;
        
        Vector2 bestDirection = currentDirection;
        float bestDot = -1f;
        
        // 各方向について、スタート地点への方向との内積を計算
        foreach (Vector2 dir in directions)
        {
            if (CanMove(dir))
            {
                float dot = Vector2.Dot(dir, homeDirection);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDirection = dir;
                }
            }
        }
        
        return bestDirection;
    }
    
    // ランダムな方向を取得します
    Vector2 GetRandomDirection(Vector2[] directions)
    {
        // 移動可能な方向のみを抽出
        Vector2[] availableDirections = System.Array.FindAll(directions, CanMove);
        
        // 移動可能な方向がない場合は現在の方向を維持
        if (availableDirections.Length == 0) return currentDirection;
        
        // ランダムに方向を選択
        return availableDirections[Random.Range(0, availableDirections.Length)];
    }
    
    // ===============================
    // 状態管理メソッド
    // ===============================
    
    // ゴーストの状態を自動的に切り替えるコルーチン
    IEnumerator StateController()
    {
        // 無限ループで状態を管理
        while (true)
        {
            if (currentState == GhostState.Chase)
            {
                // 追跡状態を5～10秒間維持
                yield return new WaitForSeconds(Random.Range(5f, 10f));
                if (currentState == GhostState.Chase)
                {
                    SetState(GhostState.Scatter);  // 散乱状態に変更
                }
            }
            else if (currentState == GhostState.Scatter)
            {
                // 散乱状態を3～7秒間維持
                yield return new WaitForSeconds(Random.Range(3f, 7f));
                if (currentState == GhostState.Scatter)
                {
                    SetState(GhostState.Chase);  // 追跡状態に変更
                }
            }
            else
            {
                // その他の状態では1秒待機
                yield return new WaitForSeconds(1f);
            }
        }
    }
    
    // ゴーストの状態を設定します（パブリックメソッド）
    public void SetState(GhostState newState)
    {
        currentState = newState;
        UpdateVisual();  // 見た目を更新
    }
    
    // 現在の状態に応じて見た目を更新します
    void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            // 状態に応じて色を変更
            spriteRenderer.color = currentState switch
            {
                GhostState.Scared => scaredColor,  // 恐怖状態では青色
                GhostState.Eaten => eatenColor,    // 食べられた状態では白色
                _ => normalColor                   // その他では通常色
            };
        }
    }
    
    // ===============================
    // パブリックメソッド（他のスクリプトから呼び出し可能）
    // ===============================
    
    // ゴーストが食べられた時の処理
    public void GetEaten()
    {
        SetState(GhostState.Eaten);           // 食べられた状態に変更
        StartCoroutine(ReturnToStart());      // スタート地点に戻るコルーチンを開始
    }
    
    // スタート地点に戻る処理のコルーチン
    IEnumerator ReturnToStart()
    {
        // スタート地点に到着するまで待機
        while (Vector2.Distance(transform.position, startPosition) > 0.5f)
        {
            yield return null;  // 1フレーム待機
        }
        
        // スタート地点に正確に配置
        transform.position = startPosition;
        
        // 3秒待機してから通常状態に戻る
        yield return new WaitForSeconds(3f);
        SetState(GhostState.Chase);
    }
    
    // 恐怖状態の設定
    public void SetScared(bool scared)
    {
        isScared = scared;
        
        if (scared && currentState != GhostState.Eaten)
        {
            // 恐怖状態にする（ただし、食べられた状態でない場合のみ）
            SetState(GhostState.Scared);
        }
        else if (!scared && currentState == GhostState.Scared)
        {
            // 恐怖状態を解除して追跡状態に戻る
            SetState(GhostState.Chase);
        }
    }
    
    // ゴーストの位置と状態をリセット
    public void ResetPosition()
    {
        transform.position = startPosition;     // スタート位置に戻す
        currentDirection = Vector2.right;       // 移動方向をリセット
        SetState(GhostState.Chase);            // 追跡状態にリセット
    }
}