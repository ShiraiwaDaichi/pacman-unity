using UnityEngine;

// ����m�F�p�̍ŏ�����MazeGenerator
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
        Debug.Log("=== SimpleMazeGenerator �J�n ===");
        GenerateSimpleMaze();
        
        if (autoTest)
        {
            Debug.Log("�����e�X�g���[�h�œ���m�F���J�n���܂�");
        }
    }
    
    void GenerateSimpleMaze()
    {
        Debug.Log("���H�����J�n...");
        
        CreateWalls();
        CreatePlayer();
        CreateDots();
        SetupCamera();
        
        Debug.Log("���H���������I");
    }
    
    void CreateWalls()
    {
        Debug.Log("�ǂ��쐬��...");
        
        // �O�ǂ��쐬
        for (int x = 0; x < 19; x++)
        {
            CreateWall(x, 0);    // ��̕�
            CreateWall(x, 20);   // ���̕�
        }
        
        for (int y = 0; y < 21; y++)
        {
            CreateWall(0, y);    // ���̕�
            CreateWall(18, y);   // �E�̕�
        }
        
        // �����̕ǂ��������쐬
        CreateWall(5, 5);
        CreateWall(6, 5);
        CreateWall(7, 5);
        CreateWall(13, 5);
        CreateWall(14, 5);
        CreateWall(15, 5);
        
        Debug.Log("�ǂ̍쐬����");
    }
    
    void CreateWall(int x, int y)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = new Vector3(x, 0, -y);
        wall.transform.SetParent(transform);
        wall.name = $"Wall_{x}_{y}";
        wall.tag = "Untagged"; // �f�t�H���g�^�O���g�p
        
        if (wallMaterial != null)
        {
            wall.GetComponent<Renderer>().material = wallMaterial;
        }
        else
        {
            // �f�t�H���g�̐��}�e���A�����쐬
            Material defaultWall = new Material(Shader.Find("Standard"));
            defaultWall.color = Color.blue;
            wall.GetComponent<Renderer>().material = defaultWall;
        }
    }
    
    void CreatePlayer()
    {
        Debug.Log("�v���C���[���쐬��...");
        
        player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.transform.position = new Vector3(9, 0, -10);
        player.name = "Player";
        player.tag = "Player";
        player.transform.localScale = Vector3.one * 0.8f;
        
        // �R���C�_�[���g���K�[�ɐݒ�
        SphereCollider collider = player.GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        
        // �ȒP�Ȉړ�����X�N���v�g��ǉ�
        SimplePlayerController controller = player.AddComponent<SimplePlayerController>();
        controller.moveSpeed = testMoveSpeed;
        
        // �}�e���A���ݒ�
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
        
        Debug.Log("�v���C���[�̍쐬����");
    }
    
    void CreateDots()
    {
        Debug.Log("�h�b�g���쐬��...");
        
        // �������̃h�b�g��z�u
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
        
        Debug.Log($"�h�b�g�̍쐬����: {dotCount}��");
    }
    
    void CreateDot(int x, int y)
    {
        GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dot.transform.position = new Vector3(x, 0, -y);
        dot.transform.SetParent(transform);
        dot.transform.localScale = Vector3.one * 0.2f;
        dot.name = $"Dot_{x}_{y}";
        dot.tag = "Untagged"; // �f�t�H���g�^�O���g�p
        
        // �R���C�_�[���g���K�[�ɐݒ�
        SphereCollider collider = dot.GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        
        // �ȒP��Dot�X�N���v�g��ǉ�
        dot.AddComponent<SimpleDot>();
        
        // �}�e���A���ݒ�
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
        Debug.Log("�J������ݒ蒆...");
        
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(9, 15, -10);
            mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            Debug.Log("�J�����̐ݒ芮��");
        }
        else
        {
            Debug.LogWarning("Main Camera��������܂���");
        }
    }
    
    void Update()
    {
        // �e�X�g�p�̃f�o�b�O���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("=== ����m�F��� ===");
            Debug.Log($"�v���C���[�ʒu: {player.transform.position}");
            Debug.Log($"�c��h�b�g��: {GameObject.FindObjectsOfType<SimpleDot>().Length}");
        }
    }
}

// �ŏ����̃v���C���[����
public class SimplePlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private int dotsCollected = 0;
    
    void Update()
    {
        // ��{�I�Ȉړ�����
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
            Debug.Log($"�h�b�g���W! ���v: {dotsCollected}��");
            Destroy(other.gameObject);
        }
    }
}

// �ŏ�����Dot�A�j���[�V����
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
        // �ȒP�ȃA�j���[�V����
        float scale = 1f + Mathf.Sin(Time.time * animationSpeed) * scaleRange;
        transform.localScale = originalScale * scale;
    }
}