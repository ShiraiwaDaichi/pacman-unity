# �ۑ�01: ��{�I��GameObject�ƃX�N���v�g�̍쐬

## ?? �w�K�ڕW
- Unity �̊�{�I�ȑ���𗝉�����
- C# �X�N���v�g�̍쐬�Ɗ�{�\�����w��
- MonoBehaviour �̊T�O�𗝉�����
- �R���\�[���o�͂Ńf�o�b�O�̊�b���w��

## ? ���菊�v����
�� 30-45 ��

## ?? �O��m��
- Unity �G�f�B�^�[�̊�{����
- C# �̕ϐ��ƃ��\�b�h�̊�{�T�O

## ?? �ۑ���e

### �X�e�b�v1: �V����GameObject���쐬����

1. Unity �G�f�B�^�[�ŃV�[�����J��
2. Hierarchy �E�B���h�E�ŉE�N���b�N �� Create Empty
3. �쐬���ꂽGameObject�̖��O���uLearningObject�v�ɕύX

### �X�e�b�v2: �ŏ��̃X�N���v�g���쐬����

Assets/Scripts/Challenges �t�H���_���쐬���A�ȉ��̃X�N���v�g��**�����œ���**���Ă��������F

```csharp
using UnityEngine;

// ���Ȃ��̍ŏ���Unity�X�N���v�g�ł��I
// MonoBehaviour���p�����邱�ƂŁAUnity�̋@�\���g�p�ł��܂�
public class MyFirstScript : MonoBehaviour
{
    // ===============================
    // �p�u���b�N�ϐ��iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("�w�K�p�ݒ�")]
    public string playerName = "���S�҃v���C���[";    // �v���C���[�̖��O
    public int playerLevel = 1;                    // �v���C���[�̃��x��
    public float moveSpeed = 5.0f;                 // �ړ����x
    public bool isActive = true;                   // �A�N�e�B�u���ǂ���
    
    // ===============================
    // �v���C�x�[�g�ϐ��i���̃X�N���v�g���ł̂ݎg�p�j
    // ===============================
    
    private int score = 0;                         // �X�R�A
    private float startTime;                       // �Q�[���J�n����
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    // Start()�́A�I�u�W�F�N�g���쐬���ꂽ�ŏ��̃t���[���ň�x�����Ăяo����܂�
    void Start()
    {
        // �Q�[���J�n���Ԃ��L�^
        startTime = Time.time;
        
        // �R���\�[���Ƀ��b�Z�[�W���o�́iConsole�E�B���h�E�Ŋm�F�ł��܂��j
        Debug.Log("=== �Q�[���J�n ===");
        Debug.Log("�v���C���[��: " + playerName);
        Debug.Log("���x��: " + playerLevel);
        Debug.Log("�ړ����x: " + moveSpeed);
        Debug.Log("�J�n����: " + startTime);
        
        // �v���C���[���ݒ肳��Ă��邩�`�F�b�N
        if (isActive)
        {
            Debug.Log(playerName + " ���Q�[���ɎQ�����܂����I");
        }
        else
        {
            Debug.Log("�v���C���[�͔�A�N�e�B�u�ł�");
        }
        
        // �����X�R�A��ݒ�
        AddScore(100);  // �Q�[���J�n�{�[�i�X
    }
    
    // Update()�́A���t���[���i�ʏ�1�b�Ԃ�60��j�Ăяo����܂�
    void Update()
    {
        // �Q�[���J�n����̌o�ߎ��Ԃ��v�Z
        float elapsedTime = Time.time - startTime;
        
        // 10�b���ƂɃ��b�Z�[�W��\���i% �͗]�艉�Z�q�j
        if (elapsedTime > 0 && (int)elapsedTime % 10 == 0 && Time.frameCount % 60 == 0)
        {
            Debug.Log("�o�ߎ���: " + (int)elapsedTime + " �b");
            Debug.Log("���݂̃X�R�A: " + score);
        }
        
        // �X�y�[�X�L�[�������ꂽ��{�[�i�X�X�R�A
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddScore(50);
            Debug.Log("�X�y�[�X�L�[�{�[�i�X�I (+50�_)");
        }
        
        // Enter�L�[�������ꂽ�烌�x���A�b�v
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LevelUp();
        }
    }
    
    // ===============================
    // �J�X�^�����\�b�h�i�����ō��@�\�j
    // ===============================
    
    // �X�R�A��ǉ����郁�\�b�h
    void AddScore(int points)
    {
        score += points;  // score = score + points �Ɠ����Ӗ�
        Debug.Log("�X�R�A�ǉ�: +" + points + " (���v: " + score + ")");
    }
    
    // ���x���A�b�v���郁�\�b�h
    void LevelUp()
    {
        playerLevel++;  // playerLevel = playerLevel + 1 �Ɠ����Ӗ�
        moveSpeed += 1.0f;  // �ړ����x�������㏸
        
        Debug.Log("=== ���x���A�b�v�I ===");
        Debug.Log("�V�������x��: " + playerLevel);
        Debug.Log("�V�����ړ����x: " + moveSpeed);
        
        // ���x���A�b�v�{�[�i�X
        AddScore(playerLevel * 100);
    }
    
    // �Q�[������\�����郁�\�b�h
    void ShowGameInfo()
    {
        Debug.Log("=== �Q�[����� ===");
        Debug.Log("�v���C���[: " + playerName);
        Debug.Log("���x��: " + playerLevel);
        Debug.Log("�X�R�A: " + score);
        Debug.Log("�ړ����x: " + moveSpeed);
        Debug.Log("�o�ߎ���: " + (Time.time - startTime) + " �b");
    }
}
```

### �X�e�b�v3: �X�N���v�g��GameObject�ɃA�^�b�`����

1. �쐬�����uLearningObject�v��I��
2. Inspector �E�B���h�E�ŁuAdd Component�v���N���b�N
3. �uMyFirstScript�v���������Ēǉ�

### �X�e�b�v4: �p�����[�^�𒲐����Ď��s����

1. Inspector �ňȉ��̒l��ύX���Ă݂Ă��������F
   - Player Name: ���Ȃ��̖��O
   - Player Level: 5
   - Move Speed: 10.0
   - Is Active: �`�F�b�N������

2. �Q�[�������s�iPlay �{�^���j
3. Console �E�B���h�E���J���ă��b�Z�[�W���m�F

### �X�e�b�v5: �L�[���͂��e�X�g����

�Q�[�����s���Ɉȉ��������Ă��������F
- **�X�y�[�X�L�[**: �{�[�i�X�X�R�A�l��
- **Enter�L�[**: ���x���A�b�v

## ?? �������Ă݂悤

### ����1: �p�����[�^�̕ύX
- Move Speed �� 100 �ɂ��Ă݂�
- Player Level �� 0 �ɂ��Ă݂�
- Is Active �̃`�F�b�N���O���Ă݂�

### ����2: �R�[�h�̉���
�ȉ��̋@�\��ǉ����Ă݂Ă��������F

```csharp
// Update()���\�b�h���ɒǉ�
if (Input.GetKeyDown(KeyCode.R))
{
    ResetGame();
}

// �V�������\�b�h��ǉ�
void ResetGame()
{
    score = 0;
    playerLevel = 1;
    moveSpeed = 5.0f;
    startTime = Time.time;
    Debug.Log("=== �Q�[�����Z�b�g ===");
}
```

### ����3: �f�o�b�O���b�Z�[�W�̉���
```csharp
// ���ڍׂȏ���\��
Debug.Log($"[{Time.time:F1}s] {playerName} (Lv.{playerLevel}) Score: {score}");
```

## ? �悭����G���[

### �G���[1: �X�N���v�g��������Ȃ�
**����**: �t�@�C�����ƃN���X������v���Ă��Ȃ�
**�������@**: �t�@�C�������uMyFirstScript.cs�v�ɂ���

### �G���[2: Console �Ƀ��b�Z�[�W���\������Ȃ�
**����**: Console �E�B���h�E���J���Ă��Ȃ�
**�������@**: Window �� General �� Console ���J��

### �G���[3: �L�[���͂��������Ȃ�
**����**: Game �r���[�Ƀt�H�[�J�X���������Ă��Ȃ�
**�������@**: Game �r���[���N���b�N���Ă���L�[������

## ?? �w�K�|�C���g

### �������ׂ��T�O
1. **MonoBehaviour**: Unity�̃R���|�[�l���g�V�X�e��
2. **Start() vs Update()**: ���s�^�C�~���O�̈Ⴂ
3. **public vs private**: �ϐ��̃A�N�Z�X����
4. **Debug.Log()**: �f�o�b�O�o�͂̏d�v��
5. **Input �N���X**: ���[�U�[���͂̏���

### �o���Ă����ׂ��p�^�[��
- �ϐ��̐錾�Ə�����
- ���\�b�h�̒�`�ƌĂяo��
- �������� (if��)
- Unity �̑��� ([Header], [SerializeField])

## ?? ���̃X�e�b�v

���̉ۑ肪����������F
1. **���K**: �R�[�h�̊e�s���������Ă��邩�����ł��邩�m�F
2. **����**: �p�����[�^��l��ύX���ē���̈Ⴂ���ώ@
3. **���̉ۑ�**: �ۑ�02�u�v���C���[�̊�{�ړ��V�X�e���v�ɐi��

## ?? �ǉ��`�������W

�]�T������ꍇ�͈ȉ��ɂ����킵�Ă݂Ă��������F

### �`�������W1: �V�����L�[���͂�ǉ�
- Q �L�[�ŃX�R�A��\��
- E �L�[�ŃQ�[������\��

### �`�������W2: �����t���{�[�i�X
- ���x����10�ȏ�̎������X�y�V�����{�[�i�X
- �X�R�A��1000�ȏ�œ��ʃ��b�Z�[�W

### �`�������W3: �����_���v�f�̒ǉ�
```csharp
// �����_���{�[�i�X�@�\
void GiveRandomBonus()
{
    int randomBonus = Random.Range(10, 100);
    AddScore(randomBonus);
    Debug.Log("�����_���{�[�i�X: " + randomBonus);
}
```

���߂łƂ��������܂��I?? 
���Ȃ��͍ŏ���Unity�X�N���v�g�����������܂����I