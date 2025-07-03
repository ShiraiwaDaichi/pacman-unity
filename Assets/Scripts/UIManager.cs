// ===============================
// UI�}�l�[�W���[
// ===============================
// ���̃X�N���v�g�́A�Q�[���̃��[�U�[�C���^�[�t�F�[�X�iUI�j���Ǘ����܂��B
// �X�R�A�\���A���C�t�\���A�Q�[���I�[�o�[��ʁA������ʁA�|�[�Y�@�\�Ȃǂ𐧌䂵�A
// �v���C���[�ɕK�v�ȏ���񋟂��܂��B

using UnityEngine;
using UnityEngine.UI;  // UI�v�f�iText�AButton�APanel�Ȃǁj���g�p���邽�߂ɕK�v

// MonoBehaviour���p�����邱�ƂŁAUnity��GameObject�ɃA�^�b�`�ł���R���|�[�l���g�ɂȂ�܂�
public class UIManager : MonoBehaviour
{
    // ===============================
    // UI�v�f�ւ̎Q�ƁiUnity�G�f�B�^�[�Őݒ肪�K�v�j
    // ===============================
    
    [Header("UI Elements")]
    public Text scoreText;          // �X�R�A��\������e�L�X�g
    public Text livesText;          // �c�胉�C�t��\������e�L�X�g
    public Text levelText;          // ���݂̃��x����\������e�L�X�g
    public Text gameOverText;       // �Q�[���I�[�o�[���ɕ\������e�L�X�g
    public Text winText;            // �������ɕ\������e�L�X�g
    public Button restartButton;    // �Q�[���ĊJ�{�^��
    public Button pauseButton;      // �|�[�Y�{�^��
    
    // ===============================
    // �p�l���v�f�ւ̎Q�ƁiUnity�G�f�B�^�[�Őݒ肪�K�v�j
    // ===============================
    // �p�l���́A������UI�v�f���O���[�v�����邽�߂̃R���e�i�ł�
    
    [Header("Panels")]
    public GameObject gameOverPanel;  // �Q�[���I�[�o�[��ʂ̃p�l��
    public GameObject winPanel;       // ������ʂ̃p�l��
    public GameObject pausePanel;     // �|�[�Y��ʂ̃p�l��
    
    // ===============================
    // �v���C�x�[�g�ϐ��i������ԊǗ��j
    // ===============================
    
    private GameManager gameManager;  // �Q�[���}�l�[�W���[�ւ̎Q��
    private bool isPaused = false;    // �Q�[�����|�[�Y�����ǂ���
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    // Start()�́A�I�u�W�F�N�g���쐬���ꂽ�ŏ��̃t���[���ň�x�����Ăяo����܂�
    void Start()
    {
        // GameManager�X�N���v�g��T���ĎQ�Ƃ��擾
        gameManager = FindObjectOfType<GameManager>();
        
        // ���X�^�[�g�{�^���̃N���b�N�C�x���g��ݒ�
        if (restartButton != null)
        {
            // AddListener�ŁA�{�^�����N���b�N���ꂽ���̏�����ݒ�
            restartButton.onClick.AddListener(RestartGame);
        }
        
        // �|�[�Y�{�^���̃N���b�N�C�x���g��ݒ�
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
        
        // �Q�[���J�n���͊e�p�l�����\���ɂ���
        // SetActive(false)�ŁAGameObject���A�N�e�B�u�i��\���j�ɂ��܂�
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        if (winPanel != null)
            winPanel.SetActive(false);
            
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
    
    // Update()�́A���t���[���Ăяo����܂�
    void Update()
    {
        // Escape�L�[�������ꂽ���Ƀ|�[�Y��؂�ւ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    // ===============================
    // UI�X�V���\�b�h�i�p�u���b�N - ���̃X�N���v�g����Ăяo���\�j
    // ===============================
    
    // �X�R�A�\�����X�V���܂�
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            // ToString()�ŁA���l�𕶎���ɕϊ�
            scoreText.text = "Score: " + score.ToString();
        }
    }
    
    // ���C�t�\�����X�V���܂�
    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives.ToString();
        }
    }
    
    // ���x���\�����X�V���܂�
    public void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level.ToString();
        }
    }
    
    // ===============================
    // �Q�[����ԕ\�����\�b�h
    // ===============================
    
    // �Q�[���I�[�o�[��ʂ�\�����܂�
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);  // �p�l����\��
        }
        
        if (gameOverText != null)
        {
            gameOverText.text = "Game Over!";
        }
    }
    
    // ������ʂ�\�����܂�
    public void ShowWin()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);  // �p�l����\��
        }
        
        if (winText != null)
        {
            winText.text = "You Win!";
        }
    }
    
    // ===============================
    // �|�[�Y�@�\���\�b�h
    // ===============================
    
    // �|�[�Y��Ԃ�؂�ւ��܂�
    public void TogglePause()
    {
        // !���Z�q�ŁAboolean�l�𔽓]������itrue �� false, false �� true�j
        isPaused = !isPaused;
        
        if (isPaused)
        {
            // �|�[�Y��Ԃɂ���
            Time.timeScale = 0f;  // �Q�[���̎��Ԃ��~�i0�{�ɂ���j
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);  // �|�[�Y�p�l����\��
            }
        }
        else
        {
            // �|�[�Y����������
            Time.timeScale = 1f;  // �Q�[���̎��Ԃ�ʏ�ɖ߂��i1�{�ɂ���j
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);  // �|�[�Y�p�l�����\��
            }
        }
    }
    
    // ===============================
    // �Q�[�����䃁�\�b�h
    // ===============================
    
    // �Q�[�����ĊJ���܂��i�p�u���b�N���\�b�h�j
    public void RestartGame()
    {
        Time.timeScale = 1f;  // ���ԃX�P�[���𐳏�ɖ߂�
        
        // GameManager�����݂���ꍇ�A���X�^�[�g���������s
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
    
    // �Q�[�����I�����܂��i�p�u���b�N���\�b�h�j
    public void QuitGame()
    {
        // Application.Quit()�ŁA�Q�[���A�v���P�[�V�������I��
        // ���ӁFUnity�G�f�B�^�[�ł͓��삵�܂���i�r���h���ꂽ�Q�[���ł̂ݗL���j
        Application.Quit();
        
        // Unity�G�f�B�^�[�Ńe�X�g����ꍇ�́A�ȉ��̃R�[�h��ǉ��ł��܂��F
        // #if UNITY_EDITOR
        // UnityEditor.EditorApplication.isPlaying = false;
        // #endif
    }
}