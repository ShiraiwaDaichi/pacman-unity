using UnityEngine;

// 動作確認用の最小限のMazeGenerator
public class SimpleMazeGenerator : MonoBehaviour
{
    [Header("Simple Setup")]
    public Material wallMaterial;
    public Material dotMaterial;
    public Material playerMaterial;
    
    [Header("Test Settings")]
    public bool autoTest = true;
    public float testMoveSpeed = 5f;
    
    private GameObject player;
    
    void Start()
    {
        Debug.Log("=== SimpleMazeGenerator 開始 ===");
        GenerateSimpleMaze();
        
        if (autoTest)
        {
            Debug.Log("自動テストモードで動作確認を開始します");
        }
    }
    
    void GenerateSimpleMaze()
    {
        Debug.Log("迷路生成開始...");
        
        CreateWalls();
        CreatePlayer();
        CreateDots();
        SetupCamera();
        
        Debug.Log("迷路生成完了！");
    }
    
    void CreateWalls()
    {
        Debug.Log("壁を作成中...");
        
        // 外壁を作成
        for (int x = 0; x < 19; x++)
        {
            CreateWall(x, 0);    // 上の壁
            CreateWall(x, 20);   // 下の壁
        }
        
        for (int y = 0; y < 21; y++)
        {
            CreateWall(0, y);    // 左の壁
            CreateWall(18, y);   // 右の壁
        }
        
        // 内部の壁をいくつか作成
        CreateWall(5, 5);
        CreateWall(6, 5);
        CreateWall(7, 5);
        CreateWall(13, 5);
        CreateWall(14, 5);
        CreateWall(15, 5);
        
        Debug.Log("壁の作成完了");
    }
    
    void CreateWall(int x, int y)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = new Vector3(x, 0, -y);
        wall.transform.SetParent(transform);
        wall.name = $"Wall_{x}_{y}";
        wall.tag = "Untagged"; // デフォルトタグを使用
        
        if (wallMaterial != null)
        {
            wall.GetComponent<Renderer>().material = wallMaterial;
        }
        else
        {
            // デフォルトの青いマテリアルを作成
            Material defaultWall = new Material(Shader.Find("Standard"));
            defaultWall.color = Color.blue;
            wall.GetComponent<Renderer>().material = defaultWall;
        }
    }
    
    void CreatePlayer()
    {
        Debug.Log("プレイヤーを作成中...");
        
        player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.transform.position = new Vector3(9, 0, -10);
        player.name = "Player";
        player.tag = "Player";
        player.transform.localScale = Vector3.one * 0.8f;
        
        // コライダーをトリガーに設定
        SphereCollider collider = player.GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        
        // 簡単な移動制御スクリプトを追加
        SimplePlayerController controller = player.AddComponent<SimplePlayerController>();
        controller.moveSpeed = testMoveSpeed;
        
        // マテリアル設定
        if (playerMaterial != null)
        {
            player.GetComponent<Renderer>().material = playerMaterial;
        }
        else
        {
            Material defaultPlayer = new Material(Shader.Find("Standard"));
            defaultPlayer.color = Color.yellow;
            player.GetComponent<Renderer>().material = defaultPlayer;
        }
        
        Debug.Log("プレイヤーの作成完了");
    }
    
    void CreateDots()
    {
        Debug.Log("ドットを作成中...");
        
        // いくつかのドットを配置
        int[] dotPositions = {2, 3, 4, 10, 11, 12, 16, 17};
        int dotCount = 0;
        
        foreach (int x in dotPositions)
        {
            for (int y = 2; y < 19; y += 3)
            {
                CreateDot(x, y);
                dotCount++;
            }
        }
        
        Debug.Log($"ドットの作成完了: {dotCount}個");
    }
    
    void CreateDot(int x, int y)
    {
        GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dot.transform.position = new Vector3(x, 0, -y);
        dot.transform.SetParent(transform);
        dot.transform.localScale = Vector3.one * 0.2f;
        dot.name = $"Dot_{x}_{y}";
        dot.tag = "Untagged"; // デフォルトタグを使用
        
        // コライダーをトリガーに設定
        SphereCollider collider = dot.GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        
        // 簡単なDotスクリプトを追加
        dot.AddComponent<SimpleDot>();
        
        // マテリアル設定
        if (dotMaterial != null)
        {
            dot.GetComponent<Renderer>().material = dotMaterial;
        }
        else
        {
            Material defaultDot = new Material(Shader.Find("Standard"));
            defaultDot.color = Color.yellow;
            dot.GetComponent<Renderer>().material = defaultDot;
        }
    }
    
    void SetupCamera()
    {
        Debug.Log("カメラを設定中...");
        
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(9, 15, -10);
            mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            Debug.Log("カメラの設定完了");
        }
        else
        {
            Debug.LogWarning("Main Cameraが見つかりません");
        }
    }
    
    void Update()
    {
        // テスト用のデバッグ情報
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("=== 動作確認情報 ===");
            Debug.Log($"プレイヤー位置: {player.transform.position}");
            Debug.Log($"残りドット数: {GameObject.FindObjectsOfType<SimpleDot>().Length}");
        }
    }
}

// 最小限のプレイヤー制御
public class SimplePlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private int dotsCollected = 0;
    
    void Update()
    {
        // 基本的な移動制御
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(horizontal, 0, vertical);
        
        if (direction.magnitude > 0.1f)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SimpleDot>() != null)
        {
            dotsCollected++;
            Debug.Log($"ドット収集! 合計: {dotsCollected}個");
            Destroy(other.gameObject);
        }
    }
}

// 最小限のDotアニメーション
public class SimpleDot : MonoBehaviour
{
    public float animationSpeed = 2f;
    public float scaleRange = 0.2f;
    
    private Vector3 originalScale;
    
    void Start()
    {
        originalScale = transform.localScale;
    }
    
    void Update()
    {
        // 簡単なアニメーション
        float scale = 1f + Mathf.Sin(Time.time * animationSpeed) * scaleRange;
        transform.localScale = originalScale * scale;
    }
}