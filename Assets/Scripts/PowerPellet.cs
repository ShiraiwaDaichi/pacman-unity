// ===============================
// パワーペレット
// ===============================
// このスクリプトは、パックマンが収集するパワーペレットの動作を制御します。
// パワーペレットは通常のドットよりも大きく、特別な視覚効果を持ちます。
// パックマンがパワーペレットを食べると、ゴーストを食べられるようになります。

using UnityEngine;

// MonoBehaviourを継承することで、UnityのGameObjectにアタッチできるコンポーネントになります
public class PowerPellet : MonoBehaviour
{
    // ===============================
    // アニメーション設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Animation")]
    public bool animateScale = true;        // スケールアニメーションを有効にするかどうか
    public float animationSpeed = 4f;       // アニメーションの速度（通常のドットより速い）
    public float scaleRange = 0.3f;         // スケール変化の範囲（通常のドットより大きい）
    
    // ===============================
    // 点滅効果設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Blinking")]
    public bool blinkingEffect = true;      // 点滅効果を有効にするかどうか
    public float blinkSpeed = 3f;           // 点滅の速度
    
    // ===============================
    // プライベート変数（内部状態管理）
    // ===============================
    
    private Vector3 originalScale;          // 元のスケール（大きさ）を保存
    private SpriteRenderer spriteRenderer; // スプライト（画像）を表示するコンポーネント
    private Color originalColor;            // 元の色を保存
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    // Start()は、オブジェクトが作成された最初のフレームで一度だけ呼び出されます
    void Start()
    {
        // オブジェクトの元のスケールを記録
        originalScale = transform.localScale;
        
        // SpriteRendererコンポーネントを取得
        // このコンポーネントは、2Dゲームで画像を表示するために使用されます
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // スプライトレンダラーが存在する場合、元の色を記録
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    // Update()は、毎フレーム（通常1秒間に60回）呼び出されます
    void Update()
    {
        // スケールアニメーションの処理
        if (animateScale)
        {
            // 時間を基にした周期的な変化を計算
            // Time.timeは、ゲーム開始からの経過時間（秒）
            // Mathf.Sin()は、サイン関数を使用して-1から1の間で滑らかに変化する値を生成
            // パワーペレットは通常のドットより速く、大きく変化します
            float scale = 1f + Mathf.Sin(Time.time * animationSpeed) * scaleRange;
            
            // 計算したスケール値を実際のオブジェクトに適用
            transform.localScale = originalScale * scale;
        }
        
        // 点滅効果の処理
        if (blinkingEffect && spriteRenderer != null)
        {
            // 透明度（アルファ値）を時間に基づいて変化させる
            // 0.5f + Mathf.Sin() * 0.5fにより、0.0～1.0の範囲で変化
            // これにより、パワーペレットが点滅して見えます
            float alpha = 0.5f + Mathf.Sin(Time.time * blinkSpeed) * 0.5f;
            
            // 新しい色を作成（RGB値は元の色を維持し、アルファ値のみ変更）
            // Color(r, g, b, a)で色を指定
            // r = 赤, g = 緑, b = 青, a = 透明度（0=完全透明, 1=完全不透明）
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
    }
    
    // ===============================
    // 衝突検出メソッド
    // ===============================
    
    // 他のオブジェクトがトリガーコライダーに触れた時に呼び出されます
    // OnTriggerEnter2Dは、2Dゲームでの衝突検出メソッドです
    void OnTriggerEnter2D(Collider2D other)
    {
        // 衝突した相手がプレイヤー（パックマン）かどうかを確認
        if (other.CompareTag("Player"))
        {
            // 実際のパワーペレット収集処理は、PacmanControllerスクリプトで行います
            // このスクリプトでは、衝突の検出のみを行い、
            // 具体的な処理（音の再生、スコア加算、パワーモード開始、オブジェクトの削除など）は
            // PacmanControllerのCollectPowerPellet()メソッドで実行されます
            
            // この設計により、ゲームロジックを一箇所に集約し、
            // 管理しやすいコードを作成できます
            
            // パワーペレットの特別な効果：
            // - 通常のドットより多くのスコアを獲得
            // - パワーモードを開始（ゴーストを食べられるようになる）
            // - 特別な効果音を再生
        }
    }
}