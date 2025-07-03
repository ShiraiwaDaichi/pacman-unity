using UnityEngine;

// �ȒP�ȓ���m�F�p��MazeGenerator
public class SimpleMazeGenerator : MonoBehaviour
{
    [Header("Simple Setup")]
    public Material wallMaterial;
    public Material dotMaterial;
    public Material playerMaterial;
    
    void Start()
    {
        GenerateSimpleMaze();
    }
    
    void GenerateSimpleMaze()
    {
        // �ȒP�Ȗ��H�𓮓I�ɐ���
        CreateWalls();
        CreatePlayer();
        CreateDots();
    }
    
    void CreateWalls()
    {
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
    }
    
    void CreateWall(int x, int y)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = new Vector3(x, 0, -y);
        wall.transform.SetParent(transform);
        wall.name = $"Wall_{x}_{y}";
        wall.tag = "Wall";
        
        if (wallMaterial != null)
        {
            wall.GetComponent<Renderer>().material = wallMaterial;
        }
    }
    
    void CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.transform.position = new Vector3(9, 0, -10);
        player.name = "Player";
        player.tag = "Player";
        player.transform.localScale = Vector3.one * 0.8f;
        
        // Collider �� Trigger �ɐݒ�
        player.GetComponent<SphereCollider>().isTrigger = true;
        
        // PacmanController ���A�^�b�`
        player.AddComponent<PacmanController>();
        
        if (playerMaterial != null)
        {
            player.GetComponent<Renderer>().material = playerMaterial;
        }
    }
    
    void CreateDots()
    {
        // �������̃h�b�g��z�u
        int[] dotPositions = {2, 3, 4, 10, 11, 12, 16, 17};
        
        foreach (int x in dotPositions)
        {
            for (int y = 2; y < 19; y += 3)
            {
                CreateDot(x, y);
            }
        }
    }
    
    void CreateDot(int x, int y)
    {
        GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dot.transform.position = new Vector3(x, 0, -y);
        dot.transform.SetParent(transform);
        dot.transform.localScale = Vector3.one * 0.2f;
        dot.name = $"Dot_{x}_{y}";
        dot.tag = "Dot";
        
        // Collider �� Trigger �ɐݒ�
        dot.GetComponent<SphereCollider>().isTrigger = true;
        
        // Dot �X�N���v�g���A�^�b�`
        dot.AddComponent<Dot>();
        
        if (dotMaterial != null)
        {
            dot.GetComponent<Renderer>().material = dotMaterial;
        }
    }
}