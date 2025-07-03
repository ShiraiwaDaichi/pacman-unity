# 課題03: 衝突検出システムの実装

## ?? 学習目標
- Collider コンポーネントの理解と使用
- Trigger と Collision の違いを学ぶ
- OnTriggerEnter/Exit イベントの実装
- タグシステムを使ったオブジェクト分類
- 衝突時の処理とゲームロジック

## ? 推定所要時間
約 60-75 分

## ?? 前提知識
- 課題02の完了
- GameObject と Component の概念
- メソッドと引数の理解

## ?? 課題内容

### ステップ1: 衝突検出用オブジェクトの準備

#### 1.1 プレイヤーの設定
1. 課題02で作成したPlayerオブジェクトを選択
2. Box Collider コンポーネントを追加
3. Is Trigger にチェックを入れる
4. Tag を "Player" に設定

#### 1.2 収集アイテムの作成
1. Create → 3D Object → Sphere で球体を作成
2. 名前を "CollectibleItem" に変更
3. 位置を (3, 0, 0) に設定
4. Sphere Collider の Is Trigger にチェック
5. 新しいタグ "Collectible" を作成して設定

#### 1.3 障害物の作成
1. Create → 3D Object → Cube で立方体を作成
2. 名前を "Obstacle" に変更
3. 位置を (-3, 0, 0) に設定
4. Box Collider の Is Trigger は **チェックしない**
5. 新しいタグ "Obstacle" を作成して設定

### ステップ2: 衝突検出スクリプトの作成

以下のスクリプトを **自分で入力** してください：

```csharp
using UnityEngine;

// 衝突検出と処理を行うスクリプト
public class CollisionDetector : MonoBehaviour
{
    // ===============================
    // パブリック変数（設定可能）
    // ===============================
    
    [Header("衝突検出設定")]
    public bool enableCollisionDetection = true;    // 衝突検出を有効にするか
    public bool showDebugMessages = true;           // デバッグメッセージを表示するか
    public float collisionCooldown = 1.0f;          // 衝突検出のクールダウン時間
    
    [Header("ゲーム設定")]
    public int score = 0;                          // プレイヤーのスコア
    public int health = 100;                       // プレイヤーのヘルス
    public int maxHealth = 100;                    // 最大ヘルス
    
    [Header("アイテム設定")]
    public int itemValue = 10;                     // アイテムの価値
    public int healthRecovery = 20;                // ヘルス回復量
    public int obstacleDamage = 25;                // 障害物のダメージ
    
    [Header("エフェクト設定")]
    public Color normalColor = Color.white;         // 通常時の色
    public Color damageColor = Color.red;           // ダメージ時の色
    public Color healColor = Color.green;           // 回復時の色
    
    // ===============================
    // プライベート変数（内部処理用）
    // ===============================
    
    private float lastCollisionTime = 0f;           // 最後の衝突時間
    private int collisionCount = 0;                 // 衝突回数
    private Renderer objectRenderer;                // オブジェクトのRenderer
    private Color originalColor;                    // 元の色
    
    // アイテム収集の統計
    private int totalItemsCollected = 0;
    private int totalDamageTaken = 0;
    private int totalHealthRecovered = 0;
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    void Start()
    {
        // 初期設定
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        
        // 初期状態の表示
        Debug.Log("=== 衝突検出システム開始 ===");
        Debug.Log("初期スコア: " + score);
        Debug.Log("初期ヘルス: " + health + "/" + maxHealth);
        
        // 操作方法を表示
        ShowControlInfo();
    }
    
    void Update()
    {
        // 特殊キーの処理
        HandleSpecialKeys();
        
        // 色の自動復帰
        RestoreOriginalColor();
    }
    
    // ===============================
    // 衝突検出メソッド（Triggerタイプ）
    // ===============================
    
    // 他のオブジェクトがTriggerに入った時
    void OnTriggerEnter(Collider other)
    {
        // 衝突検出が無効な場合は処理しない
        if (!enableCollisionDetection) return;
        
        // クールダウン時間中は処理しない
        if (Time.time - lastCollisionTime < collisionCooldown) return;
        
        // 衝突したオブジェクトのタグによって処理を分岐
        switch (other.tag)
        {
            case "Collectible":
                HandleCollectibleCollision(other);
                break;
            case "Obstacle":
                HandleObstacleCollision(other);
                break;
            case "Player":
                HandlePlayerCollision(other);
                break;
            default:
                HandleUnknownCollision(other);
                break;
        }
        
        // 衝突時間を記録
        lastCollisionTime = Time.time;
        collisionCount++;
        
        // 統計情報を更新
        UpdateStatistics();
    }
    
    // 他のオブジェクトがTriggerから出た時
    void OnTriggerExit(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log("オブジェクト離脱: " + other.name + " (タグ: " + other.tag + ")");
        }
    }
    
    // 他のオブジェクトがTrigger内にいる間（毎フレーム）
    void OnTriggerStay(Collider other)
    {
        // 継続的な効果が必要な場合にここに処理を書く
        // 例：毒ダメージ、回復エリアなど
    }
    
    // ===============================
    // 衝突検出メソッド（Collisionタイプ）
    // ===============================
    
    // 物理的な衝突が発生した時
    void OnCollisionEnter(Collision collision)
    {
        if (showDebugMessages)
        {
            Debug.Log("物理衝突発生: " + collision.gameObject.name);
            Debug.Log("衝突力: " + collision.relativeVelocity.magnitude);
        }
        
        // 衝突の強さに応じた処理
        float impactForce = collision.relativeVelocity.magnitude;
        if (impactForce > 5.0f)
        {
            Debug.Log("強い衝突！ダメージを受けました");
            TakeDamage((int)(impactForce * 2));
        }
    }
    
    // ===============================
    // 個別の衝突処理メソッド
    // ===============================
    
    // 収集アイテムとの衝突処理
    void HandleCollectibleCollision(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log("アイテム収集: " + other.name);
        }
        
        // スコアを増加
        score += itemValue;
        totalItemsCollected++;
        
        // ヘルスを回復
        RecoverHealth(healthRecovery);
        
        // 視覚的フィードバック
        ChangeColorTemporarily(healColor);
        
        // アイテムを削除
        Destroy(other.gameObject);
        
        Debug.Log("アイテム収集！ スコア: " + score + " (+" + itemValue + ")");
    }
    
    // 障害物との衝突処理
    void HandleObstacleCollision(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log("障害物に衝突: " + other.name);
        }
        
        // ダメージを受ける
        TakeDamage(obstacleDamage);
        
        // 視覚的フィードバック
        ChangeColorTemporarily(damageColor);
        
        Debug.Log("障害物に衝突！ダメージ: " + obstacleDamage);
    }
    
    // プレイヤーとの衝突処理
    void HandlePlayerCollision(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log("プレイヤーと接触: " + other.name);
        }
        
        // プレイヤー同士の特殊処理があればここに記述
    }
    
    // 不明なオブジェクトとの衝突処理
    void HandleUnknownCollision(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log("不明なオブジェクトと衝突: " + other.name + " (タグ: " + other.tag + ")");
        }
    }
    
    // ===============================
    // ゲームロジック処理メソッド
    // ===============================
    
    // ダメージを受ける処理
    void TakeDamage(int damage)
    {
        health -= damage;
        totalDamageTaken += damage;
        
        // ヘルスを0以下にならないようにする
        health = Mathf.Max(0, health);
        
        Debug.Log("ダメージ: " + damage + " (残りヘルス: " + health + ")");
        
        // ヘルスが0になったら
        if (health <= 0)
        {
            HandleGameOver();
        }
    }
    
    // ヘルスを回復する処理
    void RecoverHealth(int recovery)
    {
        health += recovery;
        totalHealthRecovered += recovery;
        
        // 最大ヘルスを超えないようにする
        health = Mathf.Min(maxHealth, health);
        
        Debug.Log("ヘルス回復: " + recovery + " (現在ヘルス: " + health + ")");
    }
    
    // ゲームオーバー処理
    void HandleGameOver()
    {
        Debug.Log("=== ゲームオーバー ===");
        Debug.Log("最終スコア: " + score);
        Debug.Log("収集アイテム数: " + totalItemsCollected);
        Debug.Log("総ダメージ: " + totalDamageTaken);
        
        // ゲームオーバー時の処理
        enableCollisionDetection = false;
        
        // 色を変更
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.black;
        }
    }
    
    // ===============================
    // 視覚的フィードバック処理
    // ===============================
    
    // 一時的に色を変更
    void ChangeColorTemporarily(Color newColor)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = newColor;
        }
    }
    
    // 元の色に戻す
    void RestoreOriginalColor()
    {
        if (objectRenderer != null && Time.time - lastCollisionTime > 0.5f)
        {
            objectRenderer.material.color = originalColor;
        }
    }
    
    // ===============================
    // 統計と情報表示
    // ===============================
    
    // 統計情報を更新
    void UpdateStatistics()
    {
        if (collisionCount % 5 == 0)  // 5回衝突ごとに表示
        {
            ShowStatistics();
        }
    }
    
    // 統計情報を表示
    void ShowStatistics()
    {
        Debug.Log("=== 統計情報 ===");
        Debug.Log("衝突回数: " + collisionCount);
        Debug.Log("現在スコア: " + score);
        Debug.Log("現在ヘルス: " + health + "/" + maxHealth);
        Debug.Log("アイテム収集数: " + totalItemsCollected);
        Debug.Log("総ダメージ: " + totalDamageTaken);
        Debug.Log("総回復量: " + totalHealthRecovered);
    }
    
    // 操作方法を表示
    void ShowControlInfo()
    {
        Debug.Log("=== 操作方法 ===");
        Debug.Log("移動: WASD");
        Debug.Log("情報表示: I");
        Debug.Log("リセット: R");
        Debug.Log("衝突検出ON/OFF: C");
        Debug.Log("緑の球: アイテム（スコア+回復）");
        Debug.Log("立方体: 障害物（ダメージ）");
    }
    
    // ===============================
    // 特殊キー処理
    // ===============================
    
    void HandleSpecialKeys()
    {
        // I キー: 情報表示
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowStatistics();
        }
        
        // R キー: 統計リセット
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetStatistics();
        }
        
        // C キー: 衝突検出ON/OFF
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCollisionDetection();
        }
        
        // H キー: ヘルプ
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowControlInfo();
        }
    }
    
    // 統計をリセット
    void ResetStatistics()
    {
        score = 0;
        health = maxHealth;
        collisionCount = 0;
        totalItemsCollected = 0;
        totalDamageTaken = 0;
        totalHealthRecovered = 0;
        enableCollisionDetection = true;
        
        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
        
        Debug.Log("=== 統計リセット完了 ===");
    }
    
    // 衝突検出の有効/無効を切り替え
    void ToggleCollisionDetection()
    {
        enableCollisionDetection = !enableCollisionDetection;
        string status = enableCollisionDetection ? "有効" : "無効";
        Debug.Log("衝突検出: " + status);
    }
}
```

### ステップ3: スクリプトの適用とテスト

1. PlayerオブジェクトにCollisionDetectorスクリプトをアタッチ
2. Inspector で設定を確認
3. ゲームを実行してテスト：
   - 緑の球体（アイテム）に近づく
   - 立方体（障害物）に衝突する
   - 各種キーを試す

### ステップ4: 複数のアイテムを作成

1. CollectibleItemを複製して3-4個配置
2. 異なる位置に配置
3. それぞれ異なる色のマテリアルを設定（オプション）

## ?? 実験してみよう

### 実験1: 衝突設定の変更

```csharp
// 異なる衝突設定を試す
public class CollisionExperiment : MonoBehaviour
{
    void Start()
    {
        // Is Trigger の ON/OFF を切り替えて動作を確認
        Collider col = GetComponent<Collider>();
        
        // 実験1: Triggerとして使用
        col.isTrigger = true;
        
        // 実験2: 物理衝突として使用
        col.isTrigger = false;
    }
}
```

### 実験2: 異なるアイテムタイプ

```csharp
// アイテムタイプに応じた処理
void HandleCollectibleCollision(Collider other)
{
    // アイテムの名前によって処理を変える
    switch (other.name)
    {
        case "HealthPotion":
            RecoverHealth(50);
            break;
        case "ScoreGem":
            score += 100;
            break;
        case "SpeedBoost":
            // 速度アップ処理
            StartCoroutine(SpeedBoostEffect());
            break;
    }
}

IEnumerator SpeedBoostEffect()
{
    // 5秒間速度アップ
    GetComponent<BasicPlayerMovement>().moveSpeed *= 2;
    yield return new WaitForSeconds(5f);
    GetComponent<BasicPlayerMovement>().moveSpeed /= 2;
}
```

### 実験3: 範囲検出システム

```csharp
// 範囲内のオブジェクトを検出
void DetectObjectsInRange()
{
    Collider[] objectsInRange = Physics.OverlapSphere(transform.position, 5.0f);
    
    foreach (Collider obj in objectsInRange)
    {
        if (obj.CompareTag("Collectible"))
        {
            Debug.Log("範囲内にアイテム発見: " + obj.name);
        }
    }
}
```

## ? よくあるエラー

### エラー1: 衝突が検出されない
**原因**: 
- Collider コンポーネントが不足
- タグが正しく設定されていない
- Is Trigger の設定ミス

**解決方法**:
- 両方のオブジェクトにColliderがあるか確認
- タグの正確な設定を確認
- OnTriggerEnter使用時はIs Triggerにチェック

### エラー2: 複数回衝突してしまう
**原因**: OnTriggerStay が連続で呼ばれる
**解決方法**: クールダウン時間を設定

### エラー3: 物理衝突が機能しない
**原因**: Rigidbody コンポーネントが不足
**解決方法**: 少なくとも片方のオブジェクトにRigidbodyを追加

## ?? 学習ポイント

### 重要な概念
1. **Collider**: 衝突検出の境界
2. **Trigger vs Collision**: 検出方法の違い
3. **Tag System**: オブジェクトの分類
4. **Event-Driven Programming**: イベントベースの処理
5. **Object State Management**: オブジェクトの状態管理

### 衝突検出の種類
- **OnTriggerEnter**: Trigger領域に入った時
- **OnTriggerExit**: Trigger領域から出た時
- **OnTriggerStay**: Trigger領域内にいる間
- **OnCollisionEnter**: 物理的衝突発生時
- **OnCollisionExit**: 物理的衝突終了時
- **OnCollisionStay**: 物理的衝突継続中

## ?? 次のステップ

この課題完了後：
1. **理解度チェック**: 各衝突イベントの使い分けを説明できるか
2. **実験**: 異なる衝突設定での動作確認
3. **次の課題**: 課題04「ゲーム状態管理システム」

## ?? 追加チャレンジ

### チャレンジ1: 複雑なアイテムシステム
```csharp
[System.Serializable]
public class Item
{
    public string name;
    public int value;
    public int healthEffect;
    public float duration;
    public Color effectColor;
}

public Item[] availableItems;
```

### チャレンジ2: パーティクルエフェクト
```csharp
public ParticleSystem collectEffect;
public ParticleSystem damageEffect;

void PlayCollectEffect()
{
    if (collectEffect != null)
    {
        collectEffect.Play();
    }
}
```

### チャレンジ3: 音響効果
```csharp
public AudioClip collectSound;
public AudioClip damageSound;
private AudioSource audioSource;

void PlaySound(AudioClip clip)
{
    if (audioSource != null && clip != null)
    {
        audioSource.PlayOneShot(clip);
    }
}
```

完璧です！?? 
衝突検出システムをマスターしました！