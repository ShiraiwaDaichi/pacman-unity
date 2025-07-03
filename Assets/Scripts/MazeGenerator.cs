// ===============================
// 迷路生成器
// ===============================
// このスクリプトは、パックマンゲームの迷路を自動的に生成します。
// 事前に定義された迷路レイアウトを使用して、壁、ドット、パワーペレット、
// パックマン、ゴーストなどのオブジェクトを適切な位置に配置します。

using UnityEngine;

// MonoBehaviourを継承することで、UnityのGameObjectにアタッチできるコンポーネントになります
public class MazeGenerator : MonoBehaviour
{
    // ===============================
    // 迷路設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Maze Settings")]
    public int mazeWidth = 19;     // 迷路の幅（セル数）
    public int mazeHeight = 21;    // 迷路の高さ（セル数）
    public float cellSize = 1f;    // 各セルのサイズ（Unity単位）
    
    // ===============================
    // プレハブ設定（Unityエディターで設定が必要）
    // ===============================
    // プレハブとは、事前に作成されたGameObjectのテンプレートです
    
    [Header("Prefabs")]
    public GameObject wallPrefab;         // 壁のプレハブ
    public GameObject dotPrefab;          // ドットのプレハブ
    public GameObject powerPelletPrefab;  // パワーペレットのプレハブ
    public GameObject pacmanPrefab;       // パックマンのプレハブ
    public GameObject ghostPrefab;        // ゴーストのプレハブ
    
    // ===============================
    // 生成設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Generation")]
    public bool generateOnStart = true;   // ゲーム開始時に自動生成するかどうか
    
    // ===============================
    // 迷路レイアウトの定義
    // ===============================
    // 2次元配列を使用して迷路のレイアウトを定義します
    // 各数字は異なるオブジェクトを表します：
    // 0 = 空のスペース
    // 1 = 壁
    // 2 = ドット
    // 3 = パワーペレット
    // 4 = パックマン
    // 5 = ゴースト
    
    private int[,] mazeLayout = new int[,]
    {
        // 行1: 上部の壁
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        // 行2: ドットとパワーペレット
        {1,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,1},
        // 行3: パワーペレットと壁の配置
        {1,3,1,1,1,2,1,1,1,1,1,1,1,2,1,1,1,3,1},
        // 行4: ドットの配置
        {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        // 行5-6: 迷路の中間部分
        {1,2,1,1,1,2,1,2,1,1,1,2,1,2,1,1,1,2,1},
        {1,2,2,2,2,2,1,2,2,1,2,2,1,2,2,2,2,2,1},
        // 行7-8: ゴーストの巣への入り口
        {1,1,1,1,1,2,1,1,0,1,0,1,1,2,1,1,1,1,1},
        {0,0,0,0,1,2,1,0,0,0,0,0,1,2,1,0,0,0,0},
        // 行9-11: ゴーストの巣
        {0,0,0,0,1,2,1,0,1,0,1,0,1,2,1,0,0,0,0},
        {1,1,1,1,1,2,0,0,1,4,1,0,0,2,1,1,1,1,1},  // 4はパックマンの初期位置
        {0,0,0,0,0,2,0,0,1,5,1,0,0,2,0,0,0,0,0},  // 5はゴーストの初期位置
        // 行12-14: ゴーストの巣からの出口
        {1,1,1,1,1,2,1,0,1,1,1,0,1,2,1,1,1,1,1},
        {0,0,0,0,1,2,1,0,0,0,0,0,1,2,1,0,0,0,0},
        {0,0,0,0,1,2,1,1,0,1,0,1,1,2,1,0,0,0,0},
        // 行15-21: 迷路の下部
        {1,1,1,1,1,2,1,2,2,1,2,2,1,2,1,1,1,1,1},
        {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        {1,2,1,1,1,2,1,1,1,1,1,1,1,2,1,1,1,2,1},
        {1,3,2,2,1,2,2,2,2,1,2,2,2,2,1,2,2,3,1},  // 3はパワーペレット
        {1,1,1,2,1,2,1,2,1,1,1,2,1,2,1,2,1,1,1},
        {1,2,2,2,2,2,1,2,2,1,2,2,1,2,2,2,2,2,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}   // 下部の壁
    };
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    // Start()は、オブジェクトが作成された最初のフレームで一度だけ呼び出されます
    void Start()
    {
        // 自動生成が有効な場合、ゲーム開始時に迷路を生成
        if (generateOnStart)
        {
            GenerateMaze();
        }
    }
    
    // ===============================
    // 迷路生成メソッド
    // ===============================
    
    // 迷路を生成します（パブリックメソッド）
    public void GenerateMaze()
    {
        ClearMaze();  // 既存の迷路をクリア
        
        // 2次元配列を使用して迷路を生成
        // 外側のループ（y）は行を、内側のループ（x）は列を処理
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                // 各セルのワールド座標を計算
                // x * cellSizeで水平位置、-y * cellSizeで垂直位置を計算
                // yを負の値にすることで、配列の上から下へと正しく配置
                Vector3 position = new Vector3(x * cellSize, -y * cellSize, 0);
                
                // 現在のセルのタイプを取得
                int cellType = mazeLayout[y, x];
                
                // セルタイプに応じてオブジェクトを生成
                switch (cellType)
                {
                    case 1: // 壁を生成
                        if (wallPrefab != null)
                        {
                            // Instantiateは、プレハブからオブジェクトを生成します
                            GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
                            // 生成したオブジェクトを、この迷路生成器の子オブジェクトに設定
                            wall.transform.SetParent(transform);
                            // オブジェクトに識別しやすい名前を付ける
                            wall.name = $"Wall_{x}_{y}";
                        }
                        break;
                        
                    case 2: // ドットを生成
                        if (dotPrefab != null)
                        {
                            GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity);
                            dot.transform.SetParent(transform);
                            dot.name = $"Dot_{x}_{y}";
                        }
                        break;
                        
                    case 3: // パワーペレットを生成
                        if (powerPelletPrefab != null)
                        {
                            GameObject powerPellet = Instantiate(powerPelletPrefab, position, Quaternion.identity);
                            powerPellet.transform.SetParent(transform);
                            powerPellet.name = $"PowerPellet_{x}_{y}";
                        }
                        break;
                        
                    case 4: // パックマンを生成
                        if (pacmanPrefab != null)
                        {
                            GameObject pacman = Instantiate(pacmanPrefab, position, Quaternion.identity);
                            pacman.name = "Pacman";
                            // パックマンは迷路生成器の子オブジェクトにしない
                            // （独立したオブジェクトとして管理）
                        }
                        break;
                        
                    case 5: // ゴーストを生成
                        if (ghostPrefab != null)
                        {
                            GameObject ghost = Instantiate(ghostPrefab, position, Quaternion.identity);
                            ghost.name = $"Ghost_{x}_{y}";
                            // ゴーストも迷路生成器の子オブジェクトにしない
                        }
                        break;
                        
                    // case 0: 空のスペースは何も生成しない
                }
            }
        }
    }
    
    // ===============================
    // 迷路クリアメソッド
    // ===============================
    
    // 既存の迷路オブジェクトをクリアします
    void ClearMaze()
    {
        // 迷路生成器の子オブジェクトを削除
        // foreach文を使用して、全ての子オブジェクトを処理
        foreach (Transform child in transform)
        {
            // Application.isPlayingは、ゲームが実行中かどうかを判定
            if (Application.isPlaying)
            {
                // ゲーム実行中はDestroy()を使用
                Destroy(child.gameObject);
            }
            else
            {
                // エディター内ではDestroyImmediate()を使用
                DestroyImmediate(child.gameObject);
            }
        }
        
        // 既存のパックマンオブジェクトを削除
        // FindGameObjectsWithTagは、指定されたタグを持つ全オブジェクトを取得
        GameObject[] pacmans = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject pacman in pacmans)
        {
            if (Application.isPlaying)
            {
                Destroy(pacman);
            }
            else
            {
                DestroyImmediate(pacman);
            }
        }
        
        // 既存のゴーストオブジェクトを削除
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in ghosts)
        {
            if (Application.isPlaying)
            {
                Destroy(ghost);
            }
            else
            {
                DestroyImmediate(ghost);
            }
        }
    }
    
    // ===============================
    // エディター用メソッド
    // ===============================
    // [ContextMenu]属性を使用すると、Unityエディターでコンポーネントを右クリックした時に
    // メニューにメソッドが表示され、手動で実行できます
    
    [ContextMenu("Generate Maze")]
    public void GenerateMazeInEditor()
    {
        GenerateMaze();
    }
    
    [ContextMenu("Clear Maze")]
    public void ClearMazeInEditor()
    {
        ClearMaze();
    }
}