# �ۑ�02: �v���C���[�̊�{�ړ��V�X�e��

## ?? �w�K�ڕW
- Transform �R���|�[�l���g�̗���
- Vector3 ���g�������W�v�Z
- Input �V�X�e���ɂ��L�[���͏���
- Time.deltaTime ���g�������ԃx�[�X�̈ړ�
- ��{�I�Ȉړ�����̎���

## ? ���菊�v����
�� 45-60 ��

## ?? �O��m��
- �ۑ�01�̊���
- Vector3 �̊�{�T�O
- Update() ���\�b�h�̗���

## ?? �ۑ���e

### �X�e�b�v1: �v���C���[�I�u�W�F�N�g�̏���

1. Hierarchy �� Create �� 3D Object �� Cube ���쐬
2. ���O���uPlayer�v�ɕύX
3. Transform �� Position �� (0, 0, 0) �ɐݒ�
4. ���₷�����邽�߁AMaterial ���쐬���ĐF��ύX�i�I�v�V�����j

### �X�e�b�v2: ��{�ړ��X�N���v�g���쐬

�ȉ��̃X�N���v�g�� **�����œ���** ���Ă��������F

```csharp
using UnityEngine;

// �v���C���[�̊�{�ړ��𐧌䂷��X�N���v�g
public class BasicPlayerMovement : MonoBehaviour
{
    // ===============================
    // �p�u���b�N�ϐ��iUnity�G�f�B�^�[�Œ����\�j
    // ===============================
    
    [Header("�ړ��ݒ�")]
    public float moveSpeed = 5.0f;              // �ړ����x
    public float rotationSpeed = 100.0f;        // ��]���x
    public bool canMove = true;                 // �ړ��\���ǂ���
    
    [Header("�ړ�����")]
    public float maxDistance = 10.0f;           // ���_����̍ő�ړ�����
    public bool useMovementBounds = true;       // �ړ��������g�p���邩�ǂ���
    
    [Header("�f�o�b�O���")]
    public bool showDebugInfo = true;           // �f�o�b�O����\�����邩�ǂ���
    
    // ===============================
    // �v���C�x�[�g�ϐ��i���������p�j
    // ===============================
    
    private Vector3 startPosition;              // �J�n�ʒu���L�^
    private Vector3 lastPosition;               // �O�t���[���̈ʒu
    private float totalDistance = 0f;           // �ړ�����������
    private int frameCounter = 0;               // �t���[���J�E���^�[
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    // �Q�[���J�n���Ɉ�x�������s
    void Start()
    {
        // �J�n�ʒu���L�^
        startPosition = transform.position;
        lastPosition = transform.position;
        
        // �f�o�b�O�����o��
        Debug.Log("=== �v���C���[�ړ��V�X�e���J�n ===");
        Debug.Log("�J�n�ʒu: " + startPosition);
        Debug.Log("�ړ����x: " + moveSpeed);
        Debug.Log("��]���x: " + rotationSpeed);
        
        // ������@���R���\�[���ɕ\��
        ShowControlInfo();
    }
    
    // ���t���[�����s�����
    void Update()
    {
        // �ړ���������Ă���ꍇ�̂ݏ���
        if (canMove)
        {
            HandleMovement();      // �ړ�����
            HandleRotation();      // ��]����
            UpdateDebugInfo();     // �f�o�b�O���X�V
        }
        
        // ����L�[�̏���
        HandleSpecialKeys();
        
        frameCounter++;
    }
    
    // ===============================
    // �ړ��������\�b�h
    // ===============================
    
    // �L�[���͂ɂ��ړ�����
    void HandleMovement()
    {
        // �ړ��������擾�iWASD �܂��͖��L�[�j
        float horizontal = Input.GetAxis("Horizontal");  // A/D �܂��� ��/��
        float vertical = Input.GetAxis("Vertical");      // W/S �܂��� ��/��
        
        // �ړ��x�N�g�����쐬
        // Vector3��3�����̍��W��x�N�g����\���N���X
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        
        // �x�N�g���̐��K���i������1�ɂ���j
        // ����ɂ��A�΂߈ړ��������x�����ɂȂ�܂�
        moveDirection = moveDirection.normalized;
        
        // ���ۂ̈ړ��ʂ��v�Z
        // Time.deltaTime ���|���邱�ƂŁA�t���[�����[�g�Ɉˑ����Ȃ������ɂȂ�܂�
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        
        // �V�����ʒu���v�Z
        Vector3 newPosition = transform.position + movement;
        
        // �ړ������̃`�F�b�N
        if (useMovementBounds)
        {
            newPosition = ApplyMovementBounds(newPosition);
        }
        
        // �ʒu���X�V
        transform.position = newPosition;
        
        // �ړ��������v�Z���ėݐ�
        float distanceThisFrame = Vector3.Distance(lastPosition, transform.position);
        totalDistance += distanceThisFrame;
        lastPosition = transform.position;
    }
    
    // ��]����
    void HandleRotation()
    {
        // Q/E �L�[�ō��E��]
        float rotationInput = 0f;
        
        if (Input.GetKey(KeyCode.Q))
        {
            rotationInput = -1f;  // ����]
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotationInput = 1f;   // �E��]
        }
        
        // ��]��K�p
        if (rotationInput != 0f)
        {
            float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotationAmount, 0);
        }
    }
    
    // �ړ�������K�p
    Vector3 ApplyMovementBounds(Vector3 targetPosition)
    {
        // ���_����̋������v�Z
        float distanceFromStart = Vector3.Distance(startPosition, targetPosition);
        
        // �ő勗���𒴂���ꍇ
        if (distanceFromStart > maxDistance)
        {
            // �����x�N�g�����擾
            Vector3 direction = (targetPosition - startPosition).normalized;
            
            // �ő勗���̈ʒu�ɐ���
            targetPosition = startPosition + direction * maxDistance;
            
            if (showDebugInfo)
            {
                Debug.Log("�ړ������ɓ��B�I �ő勗��: " + maxDistance);
            }
        }
        
        return targetPosition;
    }
    
    // ===============================
    // ����L�[����
    // ===============================
    
    void HandleSpecialKeys()
    {
        // R �L�[: �ʒu���Z�b�g
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
        
        // T �L�[: �ړ���~/�ĊJ
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleMovement();
        }
        
        // I �L�[: ���\��
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowPlayerInfo();
        }
        
        // H �L�[: �w���v�\��
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowControlInfo();
        }
    }
    
    // ===============================
    // ���[�e�B���e�B���\�b�h
    // ===============================
    
    // �ʒu�����Z�b�g
    void ResetPosition()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;  // ��]�����Z�b�g
        totalDistance = 0f;
        
        Debug.Log("=== �ʒu���Z�b�g ===");
        Debug.Log("�ʒu: " + startPosition);
    }
    
    // �ړ��̗L��/������؂�ւ�
    void ToggleMovement()
    {
        canMove = !canMove;  // true/false �𔽓]
        
        string status = canMove ? "�L��" : "����";
        Debug.Log("�ړ�����: " + status);
    }
    
    // �v���C���[����\��
    void ShowPlayerInfo()
    {
        Debug.Log("=== �v���C���[��� ===");
        Debug.Log("���݈ʒu: " + transform.position);
        Debug.Log("���݉�]: " + transform.rotation.eulerAngles);
        Debug.Log("�J�n�ʒu����̋���: " + Vector3.Distance(startPosition, transform.position));
        Debug.Log("���ړ�����: " + totalDistance.ToString("F2"));
        Debug.Log("���s�t���[����: " + frameCounter);
    }
    
    // ������@��\��
    void ShowControlInfo()
    {
        Debug.Log("=== ������@ ===");
        Debug.Log("�ړ�: WASD �܂��� ���L�[");
        Debug.Log("��]: Q�i���j / E�i�E�j");
        Debug.Log("���Z�b�g: R");
        Debug.Log("�ړ���~/�ĊJ: T");
        Debug.Log("���\��: I");
        Debug.Log("�w���v: H");
    }
    
    // �f�o�b�O�����X�V
    void UpdateDebugInfo()
    {
        // 60�t���[�����Ƃɏ���\���i1�b�Ԋu�j
        if (showDebugInfo && frameCounter % 60 == 0)
        {
            float distanceFromStart = Vector3.Distance(startPosition, transform.position);
            Debug.Log($"[{Time.time:F1}s] �ʒu: {transform.position:F1}, ����: {distanceFromStart:F1}");
        }
    }
}
```

### �X�e�b�v3: �X�N���v�g���A�^�b�`���Đݒ�

1. Player �I�u�W�F�N�g�ɁuBasicPlayerMovement�v�X�N���v�g���A�^�b�`
2. Inspector �ňȉ����m�F�E�����F
   - Move Speed: 5.0
   - Rotation Speed: 100.0
   - Max Distance: 10.0
   - Can Move: �`�F�b�N
   - Use Movement Bounds: �`�F�b�N
   - Show Debug Info: �`�F�b�N

### �X�e�b�v4: ����e�X�g

�Q�[�������s���Ĉȉ����e�X�g���Ă��������F

#### ��{�ړ��e�X�g
- **WASD �L�[**: �O�㍶�E�ړ�
- **���L�[**: �O�㍶�E�ړ�
- **Q/E �L�[**: ���E��]

#### �@�\�e�X�g
- **R �L�[**: �ʒu���Z�b�g
- **T �L�[**: �ړ���~/�ĊJ
- **I �L�[**: ���\��
- **H �L�[**: �w���v�\��

## ?? �������Ă݂悤

### ����1: �p�����[�^�̒���
�ȉ��̒l��ύX���ē���̈Ⴂ���m�F���Ă��������F

```csharp
// �������ړ�
moveSpeed = 50.0f;

// ���ᑬ�ړ�
moveSpeed = 0.5f;

// ������]
rotationSpeed = 500.0f;

// �ړ��͈͂���������
maxDistance = 3.0f;
```

### ����2: �V�����ړ��p�^�[��

```csharp
// Update()���\�b�h�ɒǉ�
// Shift �L�[�ō����ړ�
if (Input.GetKey(KeyCode.LeftShift))
{
    moveSpeed = 10.0f;
}
else
{
    moveSpeed = 5.0f;
}

// �X�y�[�X�L�[�ŃW�����v�iY���ړ��j
if (Input.GetKeyDown(KeyCode.Space))
{
    Vector3 jumpPosition = transform.position;
    jumpPosition.y += 2.0f;
    transform.position = jumpPosition;
}
```

### ����3: ���o�I�t�B�[�h�o�b�N

```csharp
// �ړ����ɋO�Ղ�`��iLineRenderer�g�p�j
// �܂��͈ړ������ɃI�u�W�F�N�g�̐F��ύX
void UpdateMovementVisuals()
{
    // �ړ����͐F��ύX
    Renderer renderer = GetComponent<Renderer>();
    if (renderer != null)
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            renderer.material.color = Color.green;  // �ړ����͗�
        }
        else
        {
            renderer.material.color = Color.white;  // ��~���͔�
        }
    }
}
```

## ? �悭����G���[

### �G���[1: �v���C���[�������Ȃ�
**����**: Input Manager �̐ݒ�܂��� Can Move �̃`�F�b�N
**�������@**: 
- Can Move �Ƀ`�F�b�N�������Ă��邩�m�F
- Game �r���[���A�N�e�B�u���m�F

### �G���[2: �ړ����ُ�ɑ���/�x��
**����**: Time.deltaTime �̗���s��
**�������@**: moveSpeed �̒l�𒲐�

### �G���[3: �΂߈ړ�����������
**����**: �x�N�g���̐��K����Y��Ă���
**�������@**: `moveDirection.normalized` ���g�p

## ?? �w�K�|�C���g

### �d�v�ȊT�O
1. **Transform**: �I�u�W�F�N�g�̈ʒu�A��]�A�X�P�[��
2. **Vector3**: 3�����x�N�g���̑���
3. **Time.deltaTime**: �t���[�����[�g�Ɨ��̎��Ԍv�Z
4. **Input.GetAxis()**: ���炩�ȃA�i���O����
5. **Vector3.normalized**: �x�N�g���̐��K��

### ���w�I�T�O
- **�x�N�g��**: �����Ƒ傫��������
- **���K��**: �x�N�g���̒�����1�ɂ��鑀��
- **�����v�Z**: Vector3.Distance() �̎g�p
- **���`���**: ���炩�Ȉړ��̊�b

## ?? ���̃X�e�b�v

���̉ۑ芮����F
1. **����x�`�F�b�N**: �e���\�b�h�̖���������ł��邩
2. **����**: �p�����[�^��ύX���ē�����ώ@
3. **���̉ۑ�**: �ۑ�03�u�Փˌ��o�V�X�e���̎����v

## ?? �ǉ��`�������W

### �`�������W1: �����V�X�e��
```csharp
// �������������ړ��V�X�e��
private Vector3 velocity = Vector3.zero;
public float acceleration = 10.0f;
public float friction = 5.0f;

void HandleInertiaMovement()
{
    Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    
    // ����
    velocity += input * acceleration * Time.deltaTime;
    
    // ���C
    velocity = Vector3.Lerp(velocity, Vector3.zero, friction * Time.deltaTime);
    
    // �ړ�
    transform.position += velocity * Time.deltaTime;
}
```

### �`�������W2: �ړ��O�Ղ̋L�^
```csharp
// �ړ��o�H���L�^�E�\��
private List<Vector3> movementPath = new List<Vector3>();

void RecordMovement()
{
    if (Vector3.Distance(lastRecordedPosition, transform.position) > 0.5f)
    {
        movementPath.Add(transform.position);
        lastRecordedPosition = transform.position;
    }
}
```

### �`�������W3: �J�����Ǐ]�V�X�e��
```csharp
// ���C���J�������v���C���[�ɒǏ]������
void UpdateCameraFollow()
{
    Camera mainCamera = Camera.main;
    if (mainCamera != null)
    {
        Vector3 cameraPosition = transform.position + new Vector3(0, 5, -10);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraPosition, 2.0f * Time.deltaTime);
        mainCamera.transform.LookAt(transform);
    }
}
```

�f���炵���I?? 
�v���C���[�̊�{�ړ��V�X�e�������������܂����I