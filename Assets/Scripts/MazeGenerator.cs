// ===============================
// ���H������
// ===============================
// ���̃X�N���v�g�́A�p�b�N�}���Q�[���̖��H�������I�ɐ������܂��B
// ���O�ɒ�`���ꂽ���H���C�A�E�g���g�p���āA�ǁA�h�b�g�A�p���[�y���b�g�A
// �p�b�N�}���A�S�[�X�g�Ȃǂ̃I�u�W�F�N�g��K�؂Ȉʒu�ɔz�u���܂��B

using UnityEngine;

// MonoBehaviour���p�����邱�ƂŁAUnity��GameObject�ɃA�^�b�`�ł���R���|�[�l���g�ɂȂ�܂�
public class MazeGenerator : MonoBehaviour
{
    // ===============================
    // ���H�ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Maze Settings")]
    public int mazeWidth = 19;     // ���H�̕��i�Z�����j
    public int mazeHeight = 21;    // ���H�̍����i�Z�����j
    public float cellSize = 1f;    // �e�Z���̃T�C�Y�iUnity�P�ʁj
    
    // ===============================
    // �v���n�u�ݒ�iUnity�G�f�B�^�[�Őݒ肪�K�v�j
    // ===============================
    // �v���n�u�Ƃ́A���O�ɍ쐬���ꂽGameObject�̃e���v���[�g�ł�
    
    [Header("Prefabs")]
    public GameObject wallPrefab;         // �ǂ̃v���n�u
    public GameObject dotPrefab;          // �h�b�g�̃v���n�u
    public GameObject powerPelletPrefab;  // �p���[�y���b�g�̃v���n�u
    public GameObject pacmanPrefab;       // �p�b�N�}���̃v���n�u
    public GameObject ghostPrefab;        // �S�[�X�g�̃v���n�u
    
    // ===============================
    // �����ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Generation")]
    public bool generateOnStart = true;   // �Q�[���J�n���Ɏ����������邩�ǂ���
    
    // ===============================
    // ���H���C�A�E�g�̒�`
    // ===============================
    // 2�����z����g�p���Ė��H�̃��C�A�E�g���`���܂�
    // �e�����͈قȂ�I�u�W�F�N�g��\���܂��F
    // 0 = ��̃X�y�[�X
    // 1 = ��
    // 2 = �h�b�g
    // 3 = �p���[�y���b�g
    // 4 = �p�b�N�}��
    // 5 = �S�[�X�g
    
    private int[,] mazeLayout = new int[,]
    {
        // �s1: �㕔�̕�
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        // �s2: �h�b�g�ƃp���[�y���b�g
        {1,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,1},
        // �s3: �p���[�y���b�g�ƕǂ̔z�u
        {1,3,1,1,1,2,1,1,1,1,1,1,1,2,1,1,1,3,1},
        // �s4: �h�b�g�̔z�u
        {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        // �s5-6: ���H�̒��ԕ���
        {1,2,1,1,1,2,1,2,1,1,1,2,1,2,1,1,1,2,1},
        {1,2,2,2,2,2,1,2,2,1,2,2,1,2,2,2,2,2,1},
        // �s7-8: �S�[�X�g�̑��ւ̓����
        {1,1,1,1,1,2,1,1,0,1,0,1,1,2,1,1,1,1,1},
        {0,0,0,0,1,2,1,0,0,0,0,0,1,2,1,0,0,0,0},
        // �s9-11: �S�[�X�g�̑�
        {0,0,0,0,1,2,1,0,1,0,1,0,1,2,1,0,0,0,0},
        {1,1,1,1,1,2,0,0,1,4,1,0,0,2,1,1,1,1,1},  // 4�̓p�b�N�}���̏����ʒu
        {0,0,0,0,0,2,0,0,1,5,1,0,0,2,0,0,0,0,0},  // 5�̓S�[�X�g�̏����ʒu
        // �s12-14: �S�[�X�g�̑�����̏o��
        {1,1,1,1,1,2,1,0,1,1,1,0,1,2,1,1,1,1,1},
        {0,0,0,0,1,2,1,0,0,0,0,0,1,2,1,0,0,0,0},
        {0,0,0,0,1,2,1,1,0,1,0,1,1,2,1,0,0,0,0},
        // �s15-21: ���H�̉���
        {1,1,1,1,1,2,1,2,2,1,2,2,1,2,1,1,1,1,1},
        {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        {1,2,1,1,1,2,1,1,1,1,1,1,1,2,1,1,1,2,1},
        {1,3,2,2,1,2,2,2,2,1,2,2,2,2,1,2,2,3,1},  // 3�̓p���[�y���b�g
        {1,1,1,2,1,2,1,2,1,1,1,2,1,2,1,2,1,1,1},
        {1,2,2,2,2,2,1,2,2,1,2,2,1,2,2,2,2,2,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}   // �����̕�
    };
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    // Start()�́A�I�u�W�F�N�g���쐬���ꂽ�ŏ��̃t���[���ň�x�����Ăяo����܂�
    void Start()
    {
        // �����������L���ȏꍇ�A�Q�[���J�n���ɖ��H�𐶐�
        if (generateOnStart)
        {
            GenerateMaze();
        }
    }
    
    // ===============================
    // ���H�������\�b�h
    // ===============================
    
    // ���H�𐶐����܂��i�p�u���b�N���\�b�h�j
    public void GenerateMaze()
    {
        ClearMaze();  // �����̖��H���N���A
        
        // 2�����z����g�p���Ė��H�𐶐�
        // �O���̃��[�v�iy�j�͍s���A�����̃��[�v�ix�j�͗������
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                // �e�Z���̃��[���h���W���v�Z
                // x * cellSize�Ő����ʒu�A-y * cellSize�Ő����ʒu���v�Z
                // y�𕉂̒l�ɂ��邱�ƂŁA�z��̏ォ�牺�ւƐ������z�u
                Vector3 position = new Vector3(x * cellSize, -y * cellSize, 0);
                
                // ���݂̃Z���̃^�C�v���擾
                int cellType = mazeLayout[y, x];
                
                // �Z���^�C�v�ɉ����ăI�u�W�F�N�g�𐶐�
                switch (cellType)
                {
                    case 1: // �ǂ𐶐�
                        if (wallPrefab != null)
                        {
                            // Instantiate�́A�v���n�u����I�u�W�F�N�g�𐶐����܂�
                            GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
                            // ���������I�u�W�F�N�g���A���̖��H������̎q�I�u�W�F�N�g�ɐݒ�
                            wall.transform.SetParent(transform);
                            // �I�u�W�F�N�g�Ɏ��ʂ��₷�����O��t����
                            wall.name = $"Wall_{x}_{y}";
                        }
                        break;
                        
                    case 2: // �h�b�g�𐶐�
                        if (dotPrefab != null)
                        {
                            GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity);
                            dot.transform.SetParent(transform);
                            dot.name = $"Dot_{x}_{y}";
                        }
                        break;
                        
                    case 3: // �p���[�y���b�g�𐶐�
                        if (powerPelletPrefab != null)
                        {
                            GameObject powerPellet = Instantiate(powerPelletPrefab, position, Quaternion.identity);
                            powerPellet.transform.SetParent(transform);
                            powerPellet.name = $"PowerPellet_{x}_{y}";
                        }
                        break;
                        
                    case 4: // �p�b�N�}���𐶐�
                        if (pacmanPrefab != null)
                        {
                            GameObject pacman = Instantiate(pacmanPrefab, position, Quaternion.identity);
                            pacman.name = "Pacman";
                            // �p�b�N�}���͖��H������̎q�I�u�W�F�N�g�ɂ��Ȃ�
                            // �i�Ɨ������I�u�W�F�N�g�Ƃ��ĊǗ��j
                        }
                        break;
                        
                    case 5: // �S�[�X�g�𐶐�
                        if (ghostPrefab != null)
                        {
                            GameObject ghost = Instantiate(ghostPrefab, position, Quaternion.identity);
                            ghost.name = $"Ghost_{x}_{y}";
                            // �S�[�X�g�����H������̎q�I�u�W�F�N�g�ɂ��Ȃ�
                        }
                        break;
                        
                    // case 0: ��̃X�y�[�X�͉����������Ȃ�
                }
            }
        }
    }
    
    // ===============================
    // ���H�N���A���\�b�h
    // ===============================
    
    // �����̖��H�I�u�W�F�N�g���N���A���܂�
    void ClearMaze()
    {
        // ���H������̎q�I�u�W�F�N�g���폜
        // foreach�����g�p���āA�S�Ă̎q�I�u�W�F�N�g������
        foreach (Transform child in transform)
        {
            // Application.isPlaying�́A�Q�[�������s�����ǂ����𔻒�
            if (Application.isPlaying)
            {
                // �Q�[�����s����Destroy()���g�p
                Destroy(child.gameObject);
            }
            else
            {
                // �G�f�B�^�[���ł�DestroyImmediate()���g�p
                DestroyImmediate(child.gameObject);
            }
        }
        
        // �����̃p�b�N�}���I�u�W�F�N�g���폜
        // FindGameObjectsWithTag�́A�w�肳�ꂽ�^�O�����S�I�u�W�F�N�g���擾
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
        
        // �����̃S�[�X�g�I�u�W�F�N�g���폜
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
    // �G�f�B�^�[�p���\�b�h
    // ===============================
    // [ContextMenu]�������g�p����ƁAUnity�G�f�B�^�[�ŃR���|�[�l���g���E�N���b�N��������
    // ���j���[�Ƀ��\�b�h���\������A�蓮�Ŏ��s�ł��܂�
    
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