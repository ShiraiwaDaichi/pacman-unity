# 課題02: プレイヤーの基本移動システム

## ?? 学習目標
- Transform コンポーネントの理解
- Vector3 を使った座標計算
- Input システムによるキー入力処理
- Time.deltaTime を使った時間ベースの移動
- 基本的な移動制御の実装

## ? 推定所要時間
約 45-60 分

## ?? 前提知識
- 課題01の完了
- Vector3 の基本概念
- Update() メソッドの理解

## ?? 課題内容

### ステップ1: プレイヤーオブジェクトの準備

1. Hierarchy で Create → 3D Object → Cube を作成
2. 名前を「Player」に変更
3. Transform の Position を (0, 0, 0) に設定
4. 見やすくするため、Material を作成して色を変更（オプション）

### ステップ2: 基本移動スクリプトを作成

以下のスクリプトを **自分で入力** してください：

```csharp
using UnityEngine;

// プレイヤーの基本移動を制御するスクリプト
public class BasicPlayerMovement : MonoBehaviour
{
    // ===============================
    // パブリック変数（Unityエディターで調整可能）
    // ===============================
    
    [Header("移動設定")]
    public float moveSpeed = 5.0f;              // 移動速度
    public float rotationSpeed = 100.0f;        // 回転速度
    public bool canMove = true;                 // 移動可能かどうか
    
    [Header("移動制限")]
    public float maxDistance = 10.0f;           // 原点からの最大移動距離
    public bool useMovementBounds = true;       // 移動制限を使用するかどうか
    
    [Header("デバッグ情報")]
    public bool showDebugInfo = true;           // デバッグ情報を表示するかどうか
    
    // ===============================
    // プライベート変数（内部処理用）
    // ===============================
    
    private Vector3 startPosition;              // 開始位置を記録
    private Vector3 lastPosition;               // 前フレームの位置
    private float totalDistance = 0f;           // 移動した総距離
    private int frameCounter = 0;               // フレームカウンター
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    // ゲーム開始時に一度だけ実行
    void Start()
    {
        // 開始位置を記録
        startPosition = transform.position;
        lastPosition = transform.position;
        
        // デバッグ情報を出力
        Debug.Log("=== プレイヤー移動システム開始 ===");
        Debug.Log("開始位置: " + startPosition);
        Debug.Log("移動速度: " + moveSpeed);
        Debug.Log("回転速度: " + rotationSpeed);
        
        // 操作方法をコンソールに表示
        ShowControlInfo();
    }
    
    // 毎フレーム実行される
    void Update()
    {
        // 移動が許可されている場合のみ処理
        if (canMove)
        {
            HandleMovement();      // 移動処理
            HandleRotation();      // 回転処理
            UpdateDebugInfo();     // デバッグ情報更新
        }
        
        // 特殊キーの処理
        HandleSpecialKeys();
        
        frameCounter++;
    }
    
    // ===============================
    // 移動処理メソッド
    // ===============================
    
    // キー入力による移動処理
    void HandleMovement()
    {
        // 移動方向を取得（WASD または矢印キー）
        float horizontal = Input.GetAxis("Horizontal");  // A/D または ←/→
        float vertical = Input.GetAxis("Vertical");      // W/S または ↑/↓
        
        // 移動ベクトルを作成
        // Vector3は3次元の座標やベクトルを表すクラス
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        
        // ベクトルの正規化（長さを1にする）
        // これにより、斜め移動時も速度が一定になります
        moveDirection = moveDirection.normalized;
        
        // 実際の移動量を計算
        // Time.deltaTime を掛けることで、フレームレートに依存しない動きになります
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        
        // 新しい位置を計算
        Vector3 newPosition = transform.position + movement;
        
        // 移動制限のチェック
        if (useMovementBounds)
        {
            newPosition = ApplyMovementBounds(newPosition);
        }
        
        // 位置を更新
        transform.position = newPosition;
        
        // 移動距離を計算して累積
        float distanceThisFrame = Vector3.Distance(lastPosition, transform.position);
        totalDistance += distanceThisFrame;
        lastPosition = transform.position;
    }
    
    // 回転処理
    void HandleRotation()
    {
        // Q/E キーで左右回転
        float rotationInput = 0f;
        
        if (Input.GetKey(KeyCode.Q))
        {
            rotationInput = -1f;  // 左回転
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotationInput = 1f;   // 右回転
        }
        
        // 回転を適用
        if (rotationInput != 0f)
        {
            float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotationAmount, 0);
        }
    }
    
    // 移動制限を適用
    Vector3 ApplyMovementBounds(Vector3 targetPosition)
    {
        // 原点からの距離を計算
        float distanceFromStart = Vector3.Distance(startPosition, targetPosition);
        
        // 最大距離を超える場合
        if (distanceFromStart > maxDistance)
        {
            // 方向ベクトルを取得
            Vector3 direction = (targetPosition - startPosition).normalized;
            
            // 最大距離の位置に制限
            targetPosition = startPosition + direction * maxDistance;
            
            if (showDebugInfo)
            {
                Debug.Log("移動制限に到達！ 最大距離: " + maxDistance);
            }
        }
        
        return targetPosition;
    }
    
    // ===============================
    // 特殊キー処理
    // ===============================
    
    void HandleSpecialKeys()
    {
        // R キー: 位置リセット
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
        
        // T キー: 移動停止/再開
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleMovement();
        }
        
        // I キー: 情報表示
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowPlayerInfo();
        }
        
        // H キー: ヘルプ表示
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowControlInfo();
        }
    }
    
    // ===============================
    // ユーティリティメソッド
    // ===============================
    
    // 位置をリセット
    void ResetPosition()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;  // 回転もリセット
        totalDistance = 0f;
        
        Debug.Log("=== 位置リセット ===");
        Debug.Log("位置: " + startPosition);
    }
    
    // 移動の有効/無効を切り替え
    void ToggleMovement()
    {
        canMove = !canMove;  // true/false を反転
        
        string status = canMove ? "有効" : "無効";
        Debug.Log("移動制御: " + status);
    }
    
    // プレイヤー情報を表示
    void ShowPlayerInfo()
    {
        Debug.Log("=== プレイヤー情報 ===");
        Debug.Log("現在位置: " + transform.position);
        Debug.Log("現在回転: " + transform.rotation.eulerAngles);
        Debug.Log("開始位置からの距離: " + Vector3.Distance(startPosition, transform.position));
        Debug.Log("総移動距離: " + totalDistance.ToString("F2"));
        Debug.Log("実行フレーム数: " + frameCounter);
    }
    
    // 操作方法を表示
    void ShowControlInfo()
    {
        Debug.Log("=== 操作方法 ===");
        Debug.Log("移動: WASD または 矢印キー");
        Debug.Log("回転: Q（左） / E（右）");
        Debug.Log("リセット: R");
        Debug.Log("移動停止/再開: T");
        Debug.Log("情報表示: I");
        Debug.Log("ヘルプ: H");
    }
    
    // デバッグ情報を更新
    void UpdateDebugInfo()
    {
        // 60フレームごとに情報を表示（1秒間隔）
        if (showDebugInfo && frameCounter % 60 == 0)
        {
            float distanceFromStart = Vector3.Distance(startPosition, transform.position);
            Debug.Log($"[{Time.time:F1}s] 位置: {transform.position:F1}, 距離: {distanceFromStart:F1}");
        }
    }
}
```

### ステップ3: スクリプトをアタッチして設定

1. Player オブジェクトに「BasicPlayerMovement」スクリプトをアタッチ
2. Inspector で以下を確認・調整：
   - Move Speed: 5.0
   - Rotation Speed: 100.0
   - Max Distance: 10.0
   - Can Move: チェック
   - Use Movement Bounds: チェック
   - Show Debug Info: チェック

### ステップ4: 動作テスト

ゲームを実行して以下をテストしてください：

#### 基本移動テスト
- **WASD キー**: 前後左右移動
- **矢印キー**: 前後左右移動
- **Q/E キー**: 左右回転

#### 機能テスト
- **R キー**: 位置リセット
- **T キー**: 移動停止/再開
- **I キー**: 情報表示
- **H キー**: ヘルプ表示

## ?? 実験してみよう

### 実験1: パラメータの調整
以下の値を変更して動作の違いを確認してください：

```csharp
// 超高速移動
moveSpeed = 50.0f;

// 超低速移動
moveSpeed = 0.5f;

// 高速回転
rotationSpeed = 500.0f;

// 移動範囲を狭くする
maxDistance = 3.0f;
```

### 実験2: 新しい移動パターン

```csharp
// Update()メソッドに追加
// Shift キーで高速移動
if (Input.GetKey(KeyCode.LeftShift))
{
    moveSpeed = 10.0f;
}
else
{
    moveSpeed = 5.0f;
}

// スペースキーでジャンプ（Y軸移動）
if (Input.GetKeyDown(KeyCode.Space))
{
    Vector3 jumpPosition = transform.position;
    jumpPosition.y += 2.0f;
    transform.position = jumpPosition;
}
```

### 実験3: 視覚的フィードバック

```csharp
// 移動時に軌跡を描画（LineRenderer使用）
// または移動方向にオブジェクトの色を変更
void UpdateMovementVisuals()
{
    // 移動中は色を変更
    Renderer renderer = GetComponent<Renderer>();
    if (renderer != null)
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            renderer.material.color = Color.green;  // 移動中は緑
        }
        else
        {
            renderer.material.color = Color.white;  // 停止中は白
        }
    }
}
```

## ? よくあるエラー

### エラー1: プレイヤーが動かない
**原因**: Input Manager の設定または Can Move のチェック
**解決方法**: 
- Can Move にチェックが入っているか確認
- Game ビューがアクティブか確認

### エラー2: 移動が異常に速い/遅い
**原因**: Time.deltaTime の理解不足
**解決方法**: moveSpeed の値を調整

### エラー3: 斜め移動が速すぎる
**原因**: ベクトルの正規化を忘れている
**解決方法**: `moveDirection.normalized` を使用

## ?? 学習ポイント

### 重要な概念
1. **Transform**: オブジェクトの位置、回転、スケール
2. **Vector3**: 3次元ベクトルの操作
3. **Time.deltaTime**: フレームレート独立の時間計算
4. **Input.GetAxis()**: 滑らかなアナログ入力
5. **Vector3.normalized**: ベクトルの正規化

### 数学的概念
- **ベクトル**: 方向と大きさを持つ量
- **正規化**: ベクトルの長さを1にする操作
- **距離計算**: Vector3.Distance() の使用
- **線形補間**: 滑らかな移動の基礎

## ?? 次のステップ

この課題完了後：
1. **理解度チェック**: 各メソッドの役割を説明できるか
2. **実験**: パラメータを変更して動作を観察
3. **次の課題**: 課題03「衝突検出システムの実装」

## ?? 追加チャレンジ

### チャレンジ1: 慣性システム
```csharp
// 慣性を持った移動システム
private Vector3 velocity = Vector3.zero;
public float acceleration = 10.0f;
public float friction = 5.0f;

void HandleInertiaMovement()
{
    Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    
    // 加速
    velocity += input * acceleration * Time.deltaTime;
    
    // 摩擦
    velocity = Vector3.Lerp(velocity, Vector3.zero, friction * Time.deltaTime);
    
    // 移動
    transform.position += velocity * Time.deltaTime;
}
```

### チャレンジ2: 移動軌跡の記録
```csharp
// 移動経路を記録・表示
private List<Vector3> movementPath = new List<Vector3>();

void RecordMovement()
{
    if (Vector3.Distance(lastRecordedPosition, transform.position) > 0.5f)
    {
        movementPath.Add(transform.position);
        lastRecordedPosition = transform.position;
    }
}
```

### チャレンジ3: カメラ追従システム
```csharp
// メインカメラをプレイヤーに追従させる
void UpdateCameraFollow()
{
    Camera mainCamera = Camera.main;
    if (mainCamera != null)
    {
        Vector3 cameraPosition = transform.position + new Vector3(0, 5, -10);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraPosition, 2.0f * Time.deltaTime);
        mainCamera.transform.LookAt(transform);
    }
}
```

素晴らしい！?? 
プレイヤーの基本移動システムを完成させました！