// ===============================
// ドット
// ===============================
// このスクリプトは、パックマンが収集するドットの動作を制御します。
// ドットは視覚的な効果（アニメーション）を提供し、
// パックマンがドットに触れた時の検出も行います。

using UnityEngine;

// MonoBehaviourを継承することで、UnityのGameObjectにアタッチできるコンポーネントになります
public class Dot : MonoBehaviour
{
    // ===============================
    // アニメーション設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Animation")]
    public bool animateScale = true;        // スケールアニメーションを有効にするかどうか
    public float animationSpeed = 2f;       // アニメーションの速度
    public float scaleRange = 0.2f;         // スケール変化の範囲（0～1の値）
    
    // ===============================
    // プライベート変数（内部状態管理）
    // ===============================
    
    private Vector3 originalScale;          // 元のスケール（大きさ）を保存
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    // Start()は、オブジェクトが作成された最初のフレームで一度だけ呼び出されます
    void Start()
    {
        // オブジェクトの元のスケールを記録
        // transform.localScaleは、オブジェクトの現在の大きさを表します
        originalScale = transform.localScale;
    }
    
    // Update()は、毎フレーム（通常1秒間に60回）呼び出されます
    void Update()
    {
        // アニメーションが有効な場合のみ実行
        if (animateScale)
        {
            // 時間を基にした周期的な変化を計算
            // Time.timeは、ゲーム開始からの経過時間（秒）
            // Mathf.Sin()は、サイン関数を使用して-1から1の間で滑らかに変化する値を生成
            // これにより、ドットが大きくなったり小さくなったりする効果を作成
            float scale = 1f + Mathf.Sin(Time.time * animationSpeed) * scaleRange;
            
            // 計算したスケール値を実際のオブジェクトに適用
            // originalScale * scaleで、元のサイズを基準に変化させる
            transform.localScale = originalScale * scale;
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
            // 実際のドット収集処理は、PacmanControllerスクリプトで行います
            // このスクリプトでは、衝突の検出のみを行い、
            // 具体的な処理（音の再生、スコア加算、オブジェクトの削除など）は
            // PacmanControllerのCollectDot()メソッドで実行されます
            
            // この設計により、ゲームロジックを一箇所に集約し、
            // 管理しやすいコードを作成できます
        }
    }
}