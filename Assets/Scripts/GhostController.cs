// ===============================
// �S�[�X�g�R���g���[���[
// ===============================
// ���̃X�N���v�g�́A�S�[�X�g�L�����N�^�[��AI�i�l�H�m�\�j�𐧌䂵�܂��B
// �S�[�X�g��4�̏�Ԃ������A���ꂼ��قȂ�s�������܂��F
// - Chase�i�ǐՁj�F�p�b�N�}����ǂ�������
// - Scatter�i�U���j�F�����_���Ɉړ�����
// - Scared�i���|�j�F�p�b�N�}�����瓦����
// - Eaten�i�H�ׂ�ꂽ�j�F�X�^�[�g�n�_�ɖ߂�

using UnityEngine;
using System.Collections;  // �R���[�`�����g�p���邽�߂ɕK�v

// MonoBehaviour���p�����邱�ƂŁAUnity��GameObject�ɃA�^�b�`�ł���R���|�[�l���g�ɂȂ�܂�
public class GhostController : MonoBehaviour
{
    // ===============================
    // �ړ��ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Movement Settings")]
    public float moveSpeed = 3f;            // �ʏ�̈ړ����x
    public float scaredSpeed = 1.5f;        // ���|��Ԏ��̈ړ����x�i�x���Ȃ�j
    public LayerMask obstacleLayer;         // ��Q���̃��C���[�i�ǂȂǁj
    
    // ===============================
    // AI�ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("AI Settings")]
    public float chaseDistance = 5f;              // �ǐՂ��J�n���鋗��
    public float directionChangeInterval = 2f;    // ������ύX����Ԋu�i�b�j
    
    // ===============================
    // ��Ԑݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("States")]
    public GhostState currentState = GhostState.Chase;  // ���݂̏��
    public Color normalColor = Color.red;               // �ʏ펞�̐F
    public Color scaredColor = Color.blue;              // ���|��Ԏ��̐F
    public Color eatenColor = Color.white;              // �H�ׂ�ꂽ���̐F
    
    // ===============================
    // �v���C�x�[�g�ϐ��i������ԊǗ��j
    // ===============================
    
    private Vector2 currentDirection = Vector2.right;   // ���݂̈ړ�����
    private Vector2 targetDirection;                    // �ڕW����
    private Transform player;                           // �v���C���[�i�p�b�N�}���j�ւ̎Q��
    private Vector2 startPosition;                      // �J�n�ʒu
    private SpriteRenderer spriteRenderer;              // �X�v���C�g�����_���[�i�F��ύX���邽�߁j
    private GameManager gameManager;                    // �Q�[���}�l�[�W���[�ւ̎Q��
    
    private float directionTimer = 0f;                  // �����ύX�^�C�}�[
    private bool isScared = false;                      // ���|��Ԃ��ǂ���
    
    // ===============================
    // �񋓌^�ienum�j- �S�[�X�g�̏�Ԃ��`
    // ===============================
    // enum�́A�֘A����萔���O���[�v�����邽�߂̃f�[�^�^�ł�
    public enum GhostState
    {
        Chase,   // �ǐՏ�ԁF�p�b�N�}����ǂ�������
        Scatter, // �U����ԁF�����_���Ɉړ�����
        Scared,  // ���|��ԁF�p�b�N�}�����瓦����
        Eaten    // �H�ׂ�ꂽ��ԁF�X�^�[�g�n�_�ɖ߂�
    }
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    // Start()�́A�I�u�W�F�N�g���쐬���ꂽ�ŏ��̃t���[���ň�x�����Ăяo����܂�
    void Start()
    {
        // �v���C���[�I�u�W�F�N�g��T���ĎQ�Ƃ��擾
        // ?. ���Z�q�́A�I�u�W�F�N�g��null�łȂ��ꍇ�̂݃v���p�e�B�ɃA�N�Z�X���܂�
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // �K�v�ȃR���|�[�l���g�̎Q�Ƃ��擾
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        
        // �J�n�ʒu���L�^
        startPosition = transform.position;
        
        // ��Ԑ���̃R���[�`�����J�n
        StartCoroutine(StateController());
    }
    
    // Update()�́A���t���[���Ăяo����܂�
    void Update()
    {
        Move();  // �ړ�����
        
        // �����ύX�^�C�}�[���X�V
        directionTimer += Time.deltaTime;
        
        // ��莞�Ԍo�߂����������ύX
        if (directionTimer >= directionChangeInterval)
        {
            ChooseDirection();
            directionTimer = 0f;  // �^�C�}�[�����Z�b�g
        }
    }
    
    // ===============================
    // �ړ��������\�b�h
    // ===============================
    
    // �S�[�X�g�̈ړ����������܂�
    void Move()
    {
        float currentSpeed = GetCurrentSpeed();  // ���݂̑��x���擾
        
        // ���݂̕����Ɉړ��\���`�F�b�N
        if (CanMove(currentDirection))
        {
            // �ړ������s
            transform.Translate(currentDirection * currentSpeed * Time.deltaTime);
        }
        else
        {
            // �ړ��ł��Ȃ��ꍇ�͐V����������I��
            ChooseDirection();
        }
    }
    
    // ���݂̏�Ԃɉ������ړ����x���擾���܂�
    float GetCurrentSpeed()
    {
        // switch�����g�p���āA��Ԃɉ��������x��Ԃ�
        return currentState switch
        {
            GhostState.Scared => scaredSpeed,        // ���|��Ԏ��͒x��
            GhostState.Eaten => moveSpeed * 2f,      // �H�ׂ�ꂽ��Ԏ��͑���
            _ => moveSpeed                           // ���̑��̏�Ԃł͒ʏ푬�x
        };
    }
    
    // �w�肳�ꂽ�����Ɉړ��\���ǂ����𔻒肵�܂�
    bool CanMove(Vector2 direction)
    {
        // ���C�L���X�g���g�p���āA�ړ������ɏ�Q�������邩�`�F�b�N
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.6f, obstacleLayer);
        return hit.collider == null;  // ���ɂ��Ԃ���Ȃ��ꍇ�͈ړ��\
    }
    
    // ===============================
    // �����I�����\�b�h
    // ===============================
    
    // ���݂̏�Ԃɉ����Ĉړ�������I�����܂�
    void ChooseDirection()
    {
        // �\�Ȉړ��������`
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 bestDirection = currentDirection;  // �f�t�H���g�͌��݂̕���
        
        // ���݂̏�Ԃɉ����čœK�ȕ�����I��
        switch (currentState)
        {
            case GhostState.Chase:
                bestDirection = GetChaseDirection(directions);      // �ǐՕ������擾
                break;
            case GhostState.Scatter:
                bestDirection = GetRandomDirection(directions);     // �����_���������擾
                break;
            case GhostState.Scared:
                bestDirection = GetScaredDirection(directions);     // �����������擾
                break;
            case GhostState.Eaten:
                bestDirection = GetReturnDirection(directions);     // �A�ҕ������擾
                break;
        }
        
        // �I�����������Ɉړ��\�ȏꍇ�̂݁A�������X�V
        if (CanMove(bestDirection))
        {
            currentDirection = bestDirection;
        }
    }
    
    // �p�b�N�}����ǐՂ���������擾���܂�
    Vector2 GetChaseDirection(Vector2[] directions)
    {
        // �v���C���[�����݂��Ȃ��ꍇ�̓����_��������Ԃ�
        if (player == null) return GetRandomDirection(directions);
        
        // �v���C���[�ւ̕����x�N�g�����v�Z
        Vector2 playerDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        
        Vector2 bestDirection = currentDirection;
        float bestDot = -1f;  // ���ς̍ő�l�i-1�`1�͈̔́j
        
        // �e�����ɂ��āA�v���C���[�ւ̕����Ƃ̓��ς��v�Z
        foreach (Vector2 dir in directions)
        {
            if (CanMove(dir))
            {
                // Vector2.Dot�͓��ς��v�Z�i�����̗ގ��x�𑪂�j
                float dot = Vector2.Dot(dir, playerDirection);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDirection = dir;
                }
            }
        }
        
        return bestDirection;
    }
    
    // �p�b�N�}�����瓦����������擾���܂�
    Vector2 GetScaredDirection(Vector2[] directions)
    {
        // �v���C���[�����݂��Ȃ��ꍇ�̓����_��������Ԃ�
        if (player == null) return GetRandomDirection(directions);
        
        // �v���C���[�ւ̕����x�N�g�����v�Z
        Vector2 playerDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        
        Vector2 bestDirection = currentDirection;
        float bestDot = 1f;  // ���ς̍ŏ��l��T��
        
        // �e�����ɂ��āA�v���C���[�ւ̕����Ƃ̓��ς��v�Z
        foreach (Vector2 dir in directions)
        {
            if (CanMove(dir))
            {
                float dot = Vector2.Dot(dir, playerDirection);
                // ���ς��������i�t�����ɋ߂��j������I��
                if (dot < bestDot)
                {
                    bestDot = dot;
                    bestDirection = dir;
                }
            }
        }
        
        return bestDirection;
    }
    
    // �X�^�[�g�n�_�ɖ߂�������擾���܂�
    Vector2 GetReturnDirection(Vector2[] directions)
    {
        // �X�^�[�g�n�_�ւ̕����x�N�g�����v�Z
        Vector2 homeDirection = (startPosition - (Vector2)transform.position).normalized;
        
        Vector2 bestDirection = currentDirection;
        float bestDot = -1f;
        
        // �e�����ɂ��āA�X�^�[�g�n�_�ւ̕����Ƃ̓��ς��v�Z
        foreach (Vector2 dir in directions)
        {
            if (CanMove(dir))
            {
                float dot = Vector2.Dot(dir, homeDirection);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDirection = dir;
                }
            }
        }
        
        return bestDirection;
    }
    
    // �����_���ȕ������擾���܂�
    Vector2 GetRandomDirection(Vector2[] directions)
    {
        // �ړ��\�ȕ����݂̂𒊏o
        Vector2[] availableDirections = System.Array.FindAll(directions, CanMove);
        
        // �ړ��\�ȕ������Ȃ��ꍇ�͌��݂̕������ێ�
        if (availableDirections.Length == 0) return currentDirection;
        
        // �����_���ɕ�����I��
        return availableDirections[Random.Range(0, availableDirections.Length)];
    }
    
    // ===============================
    // ��ԊǗ����\�b�h
    // ===============================
    
    // �S�[�X�g�̏�Ԃ������I�ɐ؂�ւ���R���[�`��
    IEnumerator StateController()
    {
        // �������[�v�ŏ�Ԃ��Ǘ�
        while (true)
        {
            if (currentState == GhostState.Chase)
            {
                // �ǐՏ�Ԃ�5�`10�b�Ԉێ�
                yield return new WaitForSeconds(Random.Range(5f, 10f));
                if (currentState == GhostState.Chase)
                {
                    SetState(GhostState.Scatter);  // �U����ԂɕύX
                }
            }
            else if (currentState == GhostState.Scatter)
            {
                // �U����Ԃ�3�`7�b�Ԉێ�
                yield return new WaitForSeconds(Random.Range(3f, 7f));
                if (currentState == GhostState.Scatter)
                {
                    SetState(GhostState.Chase);  // �ǐՏ�ԂɕύX
                }
            }
            else
            {
                // ���̑��̏�Ԃł�1�b�ҋ@
                yield return new WaitForSeconds(1f);
            }
        }
    }
    
    // �S�[�X�g�̏�Ԃ�ݒ肵�܂��i�p�u���b�N���\�b�h�j
    public void SetState(GhostState newState)
    {
        currentState = newState;
        UpdateVisual();  // �����ڂ��X�V
    }
    
    // ���݂̏�Ԃɉ����Č����ڂ��X�V���܂�
    void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            // ��Ԃɉ����ĐF��ύX
            spriteRenderer.color = currentState switch
            {
                GhostState.Scared => scaredColor,  // ���|��Ԃł͐F
                GhostState.Eaten => eatenColor,    // �H�ׂ�ꂽ��Ԃł͔��F
                _ => normalColor                   // ���̑��ł͒ʏ�F
            };
        }
    }
    
    // ===============================
    // �p�u���b�N���\�b�h�i���̃X�N���v�g����Ăяo���\�j
    // ===============================
    
    // �S�[�X�g���H�ׂ�ꂽ���̏���
    public void GetEaten()
    {
        SetState(GhostState.Eaten);           // �H�ׂ�ꂽ��ԂɕύX
        StartCoroutine(ReturnToStart());      // �X�^�[�g�n�_�ɖ߂�R���[�`�����J�n
    }
    
    // �X�^�[�g�n�_�ɖ߂鏈���̃R���[�`��
    IEnumerator ReturnToStart()
    {
        // �X�^�[�g�n�_�ɓ�������܂őҋ@
        while (Vector2.Distance(transform.position, startPosition) > 0.5f)
        {
            yield return null;  // 1�t���[���ҋ@
        }
        
        // �X�^�[�g�n�_�ɐ��m�ɔz�u
        transform.position = startPosition;
        
        // 3�b�ҋ@���Ă���ʏ��Ԃɖ߂�
        yield return new WaitForSeconds(3f);
        SetState(GhostState.Chase);
    }
    
    // ���|��Ԃ̐ݒ�
    public void SetScared(bool scared)
    {
        isScared = scared;
        
        if (scared && currentState != GhostState.Eaten)
        {
            // ���|��Ԃɂ���i�������A�H�ׂ�ꂽ��ԂłȂ��ꍇ�̂݁j
            SetState(GhostState.Scared);
        }
        else if (!scared && currentState == GhostState.Scared)
        {
            // ���|��Ԃ��������ĒǐՏ�Ԃɖ߂�
            SetState(GhostState.Chase);
        }
    }
    
    // �S�[�X�g�̈ʒu�Ə�Ԃ����Z�b�g
    public void ResetPosition()
    {
        transform.position = startPosition;     // �X�^�[�g�ʒu�ɖ߂�
        currentDirection = Vector2.right;       // �ړ����������Z�b�g
        SetState(GhostState.Chase);            // �ǐՏ�ԂɃ��Z�b�g
    }
}