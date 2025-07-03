// ===============================
// カメラコントローラー
// ===============================
// このスクリプトは、ゲーム内のカメラの動作を制御します。
// プレイヤー（パックマン）を滑らかに追従し、ズーム機能を提供し、
// カメラの移動範囲を制限する機能も含んでいます。

using UnityEngine;

// MonoBehaviourを継承することで、UnityのGameObjectにアタッチできるコンポーネントになります
public class CameraController : MonoBehaviour
{
    // ===============================
    // カメラ設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Camera Settings")]
    public Transform target;                    // 追従する対象（通常はパックマン）
    public float smoothSpeed = 0.125f;          // カメラの移動の滑らかさ（0～1、小さいほど滑らか）
    public Vector3 offset = new Vector3(0, 0, -10);  // カメラと対象との距離（オフセット）
    
    // ===============================
    // 移動範囲制限設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Bounds")]
    public bool useBounds = true;               // 移動範囲制限を使用するかどうか
    public float minX = -10f;                   // X軸の最小値
    public float maxX = 10f;                    // X軸の最大値
    public float minY = -10f;                   // Y軸の最小値
    public float maxY = 10f;                    // Y軸の最大値
    
    // ===============================
    // ズーム設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Zoom")]
    public float zoomSpeed = 2f;                // ズームの速度
    public float minZoom = 3f;                  // 最小ズーム値（最も近い）
    public float maxZoom = 10f;                 // 最大ズーム値（最も遠い）
    
    // ===============================
    // プライベート変数（内部状態管理）
    // ===============================
    