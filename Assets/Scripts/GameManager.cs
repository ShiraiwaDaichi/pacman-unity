// ===============================
// �Q�[���}�l�[�W���[
// ===============================
// ���̃X�N���v�g�́A�p�b�N�}���Q�[���S�̂̊Ǘ����s���܂��B
// �X�R�A�A���C�t�A�p���[���[�h�A���������A�s�k�����Ȃǂ��Ǘ����A
// �Q�[���̏�Ԃ𐧌䂵�܂��B

using UnityEngine;
using UnityEngine.UI;           // UI�v�f�iText�AButton�Ȃǁj���g�p���邽�߂ɕK�v
using System.Collections;       // �R���[�`���iIEnumerator�j���g�p���邽�߂ɕK�v

// MonoBehaviour���p�����邱�ƂŁAUnity��GameObject�ɃA�^�b�`�ł���R���|�[�l���g�ɂȂ�܂�
public class GameManager : MonoBehaviour
{
    // ===============================
    // �Q�[���ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Game Settings")]
    public int score = 0;                    // �v���C���[�̃X�R�A
    public int lives = 3;                    // �v���C���[�̎c�胉�C�t�i���j
    public float powerModeTime = 10f;        // �p���[���[�h�̎������ԁi�b�j
    
    // ===============================
    // UI�v�f�ւ̎Q�ƁiUnity�G�f�B�^�[�Őݒ�j
    // ===============================
    
    [Header("UI References")]
    public Text scoreText;                   // �X�R�A��\������e�L�X�g
    public Text livesText;                   // ���C�t��\������e�L�X�g
    public Text gameOverText;                // �Q�[���I�[�o�[���ɕ\������e�L�X�g
    public Text winText;                     // �������ɕ\������e�L�X�g
    public Button restartButton;             // ���X�^�[�g�{�^��
    
    // ===============================
    // �I�[�f�B�I�ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Audio")]
    public AudioSource gameAudioSource;      // �Q�[���������Đ�����R���|�[�l���g
    public AudioClip gameOverSound;          // �Q�[���I�[�o�[���̉�
    public AudioClip winSound;               // �������̉�
    public AudioClip powerModeSound;         // �p���[���[�h�J�n���̉�
    
    // ===============================
    // �v���C�x�[�g�ϐ��i������ԊǗ��j
    // ===============================
    
    private bool isPowerModeActive = false;  // �p���[���[�h���L�����ǂ���
    private bool isGameOver = false;         // �Q�[���I�[�o�[��Ԃ��ǂ���
    private bool isGameWon = false;          // �Q�[���ɏ����������ǂ���
    
    // ���̃X�N���v�g�ւ̎Q��
    private PacmanController pacmanController;  // �p�b�N�}���R���g���[���[�ւ̎Q��
    private GhostController[] ghosts;          // �S�ẴS�[�X�g�ւ̎Q�Ɓi�z��j
    
    // �h�b�g���W�̊Ǘ�
    private int totalDots;                     // �Q�[�����̑��h�b�g��
    private int collectedDots = 0;             // ���W�����h�b�g��
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    // Start()�́A�I�u�W�F�N�g���쐬���ꂽ�ŏ��̃t���[���ň�x�����Ăяo����܂�
    void Start()
    {
        // �K�v�ȃR���|�[�l���g�̎Q�Ƃ��擾
        pacmanController = FindObjectOfType<PacmanController>();  // PacmanController�X�N���v�g��T��
        ghosts = FindObjectsOfType<GhostController>();           // �S�Ă�GhostController�X�N���v�g��T��
        
        CountTotalDots();  // �Q�[�����̑��h�b�g���𐔂���
        UpdateUI();        // UI�\�����X�V
        
        // �Q�[���J�n���́A�Q�[���I�[�o�[�e�L�X�g�Ə����e�L�X�g���\���ɂ���
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
            
        if (winText != null)
            winText.gameObject.SetActive(false);
            
        // ���X�^�[�g�{�^���̐ݒ�
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);           // �ŏ��͔�\��
            restartButton.onClick.AddListener(RestartGame);      // �{�^���N���b�N���̏�����ݒ�
        }
    }
    
    // Update()�́A���t���[���Ăяo����܂�
    void Update()
    {
        // R�L�[�������ꂽ��Q�[�������X�^�[�g
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    
    // ===============================
    // �h�b�g�Ǘ����\�b�h
    // ===============================
    
    // �Q�[�����̑��h�b�g���𐔂��܂�
    void CountTotalDots()
    {
        // GameObject.FindGameObjectsWithTag�ŁA�w�肳�ꂽ�^�O�̑S�I�u�W�F�N�g���擾
        GameObject[] dots = GameObject.FindGameObjectsWithTag("Dot");
        GameObject[] powerPellets = GameObject.FindGameObjectsWithTag("PowerPellet");
        
        // �ʏ�̃h�b�g�ƃp���[�y���b�g�̍��v�����L�^
        totalDots = dots.Length + powerPellets.Length;
    }
    
    // ===============================
    // �X�R�A�Ǘ����\�b�h
    // ===============================
    
    // �X�R�A��ǉ����܂��i�p�u���b�N���\�b�h - ���̃X�N���v�g����Ăяo���\�j
    public void AddScore(int points)
    {
        score += points;  // �X�R�A�ɓ_����ǉ�
        UpdateUI();       // UI�\�����X�V
    }
    
    // ===============================
    // �p���[���[�h�Ǘ����\�b�h
    // ===============================
    
    // �p���[���[�h���J�n���܂��i�p�u���b�N���\�b�h�j
    public void ActivatePowerMode()
    {
        // ���Ƀp���[���[�h���L���ȏꍇ�͉������Ȃ�
        if (isPowerModeActive) return;
        
        // �R���[�`�����J�n�i���Ԍo�߂̏����j
        StartCoroutine(PowerModeCoroutine());
    }
    
    // �p���[���[�h�̎��ԊǗ����s���R���[�`��
    // �R���[�`���́A������r���ňꎞ��~���A��ōĊJ�ł������ȃ��\�b�h�ł�
    IEnumerator PowerModeCoroutine()
    {
        isPowerModeActive = true;  // �p���[���[�h��L���ɂ���
        
        // �p���[���[�h�J�n�����Đ�
        if (gameAudioSource != null && powerModeSound != null)
        {
            gameAudioSource.PlayOneShot(powerModeSound);
        }
        
        // �S�ẴS�[�X�g��|���点��i������Ԃɂ���j
        foreach (GhostController ghost in ghosts)
        {
            if (ghost != null)  // �S�[�X�g�����݂���ꍇ�̂�
            {
                ghost.SetScared(true);  // �|��������Ԃɂ���
            }
        }
        
        // �w�肳�ꂽ���Ԃ����ҋ@�i�p���[���[�h�������ԁj
        yield return new WaitForSeconds(powerModeTime);
        
        isPowerModeActive = false;  // �p���[���[�h���I��
        
        // �S�ẴS�[�X�g��ʏ��Ԃɖ߂�
        foreach (GhostController ghost in ghosts)
        {
            if (ghost != null)
            {
                ghost.SetScared(false);  // �ʏ��Ԃɖ߂�
            }
        }
    }
    
    // �p���[���[�h���L�����ǂ������m�F���܂��i�p�u���b�N���\�b�h�j
    public bool IsPowerModeActive()
    {
        return isPowerModeActive;
    }
    
    // ===============================
    // �Q�[���I�[�o�[�������\�b�h
    // ===============================
    
    // �Q�[���I�[�o�[�������s���܂��i�p�u���b�N���\�b�h�j
    public void GameOver()
    {
        // ���ɃQ�[���I�[�o�[��Ԃ̏ꍇ�͉������Ȃ�
        if (isGameOver) return;
        
        isGameOver = true;  // �Q�[���I�[�o�[��Ԃɂ���
        lives--;           // ���C�t��1���炷
        
        // ���C�t��0�ȉ��ɂȂ����ꍇ
        if (lives <= 0)
        {
            // ���S�ȃQ�[���I�[�o�[
            if (gameAudioSource != null && gameOverSound != null)
            {
                gameAudioSource.PlayOneShot(gameOverSound);  // �Q�[���I�[�o�[�����Đ�
            }
            
            // �Q�[���I�[�o�[�e�L�X�g��\��
            if (gameOverText != null)
            {
                gameOverText.gameObject.SetActive(true);
            }
            
            // ���X�^�[�g�{�^����\��
            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }
            
            // �Q�[���̎��Ԃ��~�iTime.timeScale = 0�őS�Ă̎��ԏ������~�܂�j
            Time.timeScale = 0f;
        }
        else
        {
            // ���C�t���c���Ă���ꍇ�́A���x�����ĊJ
            StartCoroutine(RestartLevel());
        }
        
        UpdateUI();  // UI�\�����X�V
    }
    
    // ���x�����ĊJ����R���[�`��
    IEnumerator RestartLevel()
    {
        // 2�b�ҋ@�i�v���C���[���󋵂𗝉����鎞�Ԃ�^����j
        yield return new WaitForSeconds(2f);
        
        // �p�b�N�}���ƃS�[�X�g�̈ʒu�����Z�b�g
        if (pacmanController != null)
        {
            pacmanController.ResetPosition();
        }
        
        foreach (GhostController ghost in ghosts)
        {
            if (ghost != null)
            {
                ghost.ResetPosition();
            }
        }
        
        isGameOver = false;  // �Q�[���I�[�o�[��Ԃ�����
    }
    
    // ===============================
    // ���������`�F�b�N���\�b�h
    // ===============================
    
    // �����������`�F�b�N���܂��i�p�u���b�N���\�b�h�j
    public void CheckWinCondition()
    {
        collectedDots++;  // ���W�����h�b�g���𑝂₷
        
        // �S�Ẵh�b�g�����W�����ꍇ
        if (collectedDots >= totalDots)
        {
            WinGame();  // �������������s
        }
    }
    
    // �����������s���܂�
    void WinGame()
    {
        // ���ɏ�����Ԃ̏ꍇ�͉������Ȃ�
        if (isGameWon) return;
        
        isGameWon = true;  // ������Ԃɂ���
        
        // ���������Đ�
        if (gameAudioSource != null && winSound != null)
        {
            gameAudioSource.PlayOneShot(winSound);
        }
        
        // �����e�L�X�g��\��
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
        }
        
        // ���X�^�[�g�{�^����\��
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
        }
        
        // �Q�[���̎��Ԃ��~
        Time.timeScale = 0f;
    }
    
    // ===============================
    // UI�X�V���\�b�h
    // ===============================
    
    // UI�\�����X�V���܂�
    void UpdateUI()
    {
        // �X�R�A�e�L�X�g���X�V
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        
        // ���C�t�e�L�X�g���X�V
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
    }
    
    // ===============================
    // �Q�[�����X�^�[�g���\�b�h
    // ===============================
    
    // �Q�[�����ĊJ���܂��i�p�u���b�N���\�b�h�j
    public void RestartGame()
    {
        Time.timeScale = 1f;  // �Q�[���̎��Ԃ𐳏�ɖ߂�
        
        // ���݂̃V�[�����ēǂݍ��݁i�Q�[�����ŏ������蒼���j
        // SceneManager���g�p���ăV�[���������[�h
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}