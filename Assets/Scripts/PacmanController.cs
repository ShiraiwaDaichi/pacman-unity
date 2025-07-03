// ===============================
// �p�b�N�}���R���g���[���[
// ===============================
// ���̃X�N���v�g�́A�v���C���[�����삷��p�b�N�}���L�����N�^�[�̓���𐧌䂵�܂��B
// �ړ��A��]�A�A�C�e���̎��W�A�S�[�X�g�Ƃ̏Փˏ����Ȃǂ��s���܂��B

using UnityEngine;

// MonoBehaviour���p�����邱�ƂŁAUnity��GameObject�ɃA�^�b�`�ł���R���|�[�l���g�ɂȂ�܂�
public class PacmanController : MonoBehaviour
{
    // ===============================
    // �p�u���b�N�ϐ��iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    // [Header] �������g�p����ƁAUnity�G�f�B�^�[�ŃO���[�v�����ĕ\������܂�
    [Header("Movement Settings")]
    public float moveSpeed = 5f;           // �p�b�N�}���̈ړ����x�i1�b�ԂɈړ����鋗���j
    public LayerMask obstacleLayer;        // ��Q���̃��C���[�i�ǂȂǁA�ړ���W����I�u�W�F�N�g�j
    
    [Header("Animation")]
    public Animator animator;              // �A�j���[�V�����𐧌䂷��Animator�R���|�[�l���g
    
    // ===============================
    // �v���C�x�[�g�ϐ��i���̃X�N���v�g���ł̂ݎg�p�j
    // ===============================
    
    // Vector2��2�����̃x�N�g���ix,y���W�j��\���܂�
    private Vector2 currentDirection = Vector2.zero;  // ���݂̈ړ������i0,0�͐Î~��ԁj
    private Vector2 nextDirection = Vector2.zero;     // ���Ɉړ�����������
    private Vector2 startPosition;                    // �Q�[���J�n���̈ʒu
    private bool isMoving = false;                    // �ړ������ǂ����̏��
    
    [Header("Audio")]
    public AudioSource audioSource;       // �������Đ�����R���|�[�l���g
    public AudioClip dotSound;           // �h�b�g��H�ׂ����̉�
    public AudioClip powerPelletSound;   // �p���[�y���b�g��H�ׂ����̉�
    
    // ���̃X�N���v�g�ւ̎Q��
    private GameManager gameManager;      // �Q�[���S�̂��Ǘ�����X�N���v�g
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    // Start()�́A�I�u�W�F�N�g���쐬���ꂽ�ŏ��̃t���[���ň�x�����Ăяo����܂�
    void Start()
    {
        // �����ݒ���s���܂�
        startPosition = transform.position;                    // �J�n�ʒu���L�^
        gameManager = FindObjectOfType<GameManager>();        // GameManager�X�N���v�g��T���ĎQ�Ƃ��擾
        
        // AudioSource�R���|�[�l���g���ݒ肳��Ă��Ȃ��ꍇ�A�����I�Ɏ擾�����݂܂�
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
            
        // Animator�R���|�[�l���g���ݒ肳��Ă��Ȃ��ꍇ�A�����I�Ɏ擾�����݂܂�
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    // Update()�́A���t���[���i�ʏ�1�b�Ԃ�60��j�Ăяo����܂�
    void Update()
    {
        HandleInput();  // �v���C���[�̓��͂�����
        Move();         // �p�b�N�}���̈ړ�������
        
        // �A�j���[�^�[���ݒ肳��Ă���ꍇ�A�ړ���Ԃ��X�V
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }
    }
    
    // ===============================
    // ���͏������\�b�h
    // ===============================
    
    // �v���C���[�̃L�[�{�[�h���͂��������܂�
    void HandleInput()
    {
        // Input.GetKeyDown()�́A�L�[�������ꂽ�u�Ԃ�true��Ԃ��܂�
        // ���L�[�܂���WASD�L�[�ňړ�������ݒ�
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            nextDirection = Vector2.up;      // ������̃x�N�g���i0, 1�j
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            nextDirection = Vector2.down;    // �������̃x�N�g���i0, -1�j
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            nextDirection = Vector2.left;    // �������̃x�N�g���i-1, 0�j
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            nextDirection = Vector2.right;   // �E�����̃x�N�g���i1, 0�j
        }
    }
    
    // ===============================
    // �ړ��������\�b�h
    // ===============================
    
    // �p�b�N�}���̈ړ����������܂�
    void Move()
    {
        // ���̕����Ɉړ��\���ǂ������`�F�b�N
        // Vector2.zero�́i0,0�j��\���A�������ݒ肳��Ă��Ȃ���Ԃ��Ӗ����܂�
        if (nextDirection != Vector2.zero && CanMove(nextDirection))
        {
            currentDirection = nextDirection;     // ���݂̕������X�V
            nextDirection = Vector2.zero;         // ���̕��������Z�b�g
        }
        
        // ���݂̕����Ɉړ��\���ǂ������`�F�b�N
        if (currentDirection != Vector2.zero && CanMove(currentDirection))
        {
            // transform.Translate�́A�I�u�W�F�N�g�̈ʒu���ړ������܂�
            // currentDirection * moveSpeed * Time.deltaTime�ŁA�����E���x�E���Ԃ��l�������ړ��ʂ��v�Z
            // Time.deltaTime�͑O�t���[������̌o�ߎ��ԁi�ʏ�1/60�b�j
            transform.Translate(currentDirection * moveSpeed * Time.deltaTime);
            isMoving = true;
            
            // �p�b�N�}���̌������ړ������ɍ��킹�ĉ�]
            UpdateRotation();
        }
        else
        {
            isMoving = false;  // �ړ��ł��Ȃ��ꍇ�͒�~��Ԃ�
        }
    }
    
    // �w�肳�ꂽ�����Ɉړ��\���ǂ����𔻒肵�܂�
    bool CanMove(Vector2 direction)
    {
        // Physics2D.Raycast�́A�w�肳�ꂽ�����Ɍ������āu�����Ȃ������v�𔭎˂��A
        // �����ɂԂ��邩�ǂ����𒲂ׂ܂��i���C�L���X�g�ƌĂ΂��Z�p�j
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,  // �����̊J�n�ʒu�i�p�b�N�}���̌��݈ʒu�j
            direction,          // �����̕���
            0.6f,              // �����̒����i0.6���j�b�g��܂Œ��ׂ�j
            obstacleLayer      // ���ׂ�Ώۂ̃��C���[�i�ǂȂǁj
        );
        
        // hit.collider��null�̏ꍇ�A���ɂ��Ԃ���Ȃ������Ƃ������Ƃňړ��\
        return hit.collider == null;
    }
    
    // �p�b�N�}���̌������ړ������ɍ��킹�ĉ�]�����܂�
    void UpdateRotation()
    {
        // Mathf.Atan2�́A�x�N�g���̊p�x�����W�A���Ŏ擾���܂�
        // Mathf.Rad2Deg�Ń��W�A����x���ɕϊ�
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        
        // Quaternion.AngleAxis�Ŏw�肳�ꂽ�p�x�̉�]���쐬���A�I�u�W�F�N�g�ɓK�p
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    // ===============================
    // �Փˏ������\�b�h
    // ===============================
    
    // ���̃I�u�W�F�N�g�Ƃ̏Փ˂����o���܂��i�g���K�[�R���C�_�[���K�v�j
    void OnTriggerEnter2D(Collider2D other)
    {
        // other.CompareTag�́A�Փ˂�������̃^�O�𒲂ׂ܂�
        if (other.CompareTag("Dot"))
        {
            CollectDot(other.gameObject);  // �h�b�g�����W
        }
        else if (other.CompareTag("PowerPellet"))
        {
            CollectPowerPellet(other.gameObject);  // �p���[�y���b�g�����W
        }
        else if (other.CompareTag("Ghost"))
        {
            // GetComponent�ŁA�Փ˂����S�[�X�g��GhostController�X�N���v�g���擾
            HandleGhostCollision(other.GetComponent<GhostController>());
        }
    }
    
    // �h�b�g�����W���鏈��
    void CollectDot(GameObject dot)
    {
        // �������ݒ肳��Ă���ꍇ�A�h�b�g��H�ׂ������Đ�
        if (audioSource != null && dotSound != null)
        {
            audioSource.PlayOneShot(dotSound);  // PlayOneShot�͈�x���������Đ�
        }
        
        // Destroy�Ńh�b�g�I�u�W�F�N�g���Q�[������폜
        Destroy(dot);
        
        // GameManager�����݂���ꍇ�A�X�R�A��ǉ��������������`�F�b�N
        if (gameManager != null)
        {
            gameManager.AddScore(10);              // 10�_�ǉ�
            gameManager.CheckWinCondition();      // ���������i�S�h�b�g���W�j���`�F�b�N
        }
    }
    
    // �p���[�y���b�g�����W���鏈��
    void CollectPowerPellet(GameObject powerPellet)
    {
        // �������ݒ肳��Ă���ꍇ�A�p���[�y���b�g��H�ׂ������Đ�
        if (audioSource != null && powerPelletSound != null)
        {
            audioSource.PlayOneShot(powerPelletSound);
        }
        
        // �p���[�y���b�g�I�u�W�F�N�g���Q�[������폜
        Destroy(powerPellet);
        
        // GameManager�����݂���ꍇ�A�X�R�A��ǉ����p���[���[�h���J�n
        if (gameManager != null)
        {
            gameManager.AddScore(50);                 // 50�_�ǉ�
            gameManager.ActivatePowerMode();          // �p���[���[�h�J�n
        }
    }
    
    // �S�[�X�g�Ƃ̏Փ˂�����
    void HandleGhostCollision(GhostController ghost)
    {
        if (gameManager != null)
        {
            // �p���[���[�h���L���ȏꍇ
            if (gameManager.IsPowerModeActive())
            {
                // �S�[�X�g��H�ׂ邱�Ƃ��ł���
                gameManager.AddScore(200);    // 200�_�ǉ�
                ghost.GetEaten();            // �S�[�X�g��H�ׂ�ꂽ��Ԃɂ���
            }
            else
            {
                // �p���[���[�h�łȂ��ꍇ�̓Q�[���I�[�o�[
                gameManager.GameOver();
            }
        }
    }
    
    // ===============================
    // �p�u���b�N���\�b�h�i���̃X�N���v�g����Ăяo���\�j
    // ===============================
    
    // �p�b�N�}���̈ʒu�Ə�Ԃ����Z�b�g���܂�
    public void ResetPosition()
    {
        transform.position = startPosition;        // �J�n�ʒu�ɖ߂�
        currentDirection = Vector2.zero;           // �ړ����������Z�b�g
        nextDirection = Vector2.zero;              // ���̈ړ����������Z�b�g
        isMoving = false;                         // �ړ���Ԃ����Z�b�g
    }
}