# �܂����|�C���g�L�^�h�L�������g

## ?? �T�v
���̃h�L�������g�́AUnity Pacman Learning Project�J�����ɔ��������܂����|�C���g�Ɖ����ߒ����L�^�������̂ł��B
�w�K�҂����l�̖��ɑ��������ۂ̎Q�l�����Ƃ��Ċ��p�ł��܂��B

**�L�^��**: 2025�N7��3��  
**�v���W�F�N�g**: Unity Pacman Learning Project  
**���݂̏�**: SimpleMazeGenerator����m�F��

---

## ?? ���݂̂܂����|�C���g

### ���1: SimpleMazeGenerator�̃r���h�G���[

#### ?? ����
- `SimpleMazeGenerator.cs`���R���p�C�����悤�Ƃ���ƃr���h�����s����
- ��̓I�ȃG���[���b�Z�[�W���擾�ł��Ȃ���
- `run_build`�R�}���h�Łu�r���h�Ɏ��s���܂����v�ƕ\��

#### ?? �z�肳��錴��
1. **2D/3D�����V�X�e���̍���**
   - ������`PacmanController.cs`��2D�����i`Physics2D`�A`Collider2D`�j���g�p
   - `SimpleMazeGenerator.cs`��3D�����i`Physics`�A`Collider`�j���g�p
   - �݊����̖�肪�������Ă���\��

2. **�^�O�V�X�e���̕s����**
   - �����V�X�e���͓���̃^�O�i"Player", "Dot", "Ghost"�j��O��
   - `SimpleMazeGenerator`�ł�"Untagged"���g�p
   - �^�O�̕s��v�ɂ��Q�ƃG���[�̉\��

3. **Unity �v���W�F�N�g�ݒ�**
   - �v���W�F�N�g��2D�ݒ�ɂȂ��Ă���\��
   - Physics2D�ݒ�Ƃ̋���

#### ?? ���{�����΍�

##### �΍�1: 3D�݊����Ή�
```csharp
// ���̎����i��肠��j
player.AddComponent<PacmanController>();  // 2D�p�̃R���g���[���[

// �C����i3D�Ή��j
player.AddComponent<SimplePlayerController>();  // 3D�p�̓Ǝ��R���g���[���[
```

##### �΍�2: �}�e���A���t�H�[���o�b�N�ǉ�
```csharp
// �}�e���A�����ݒ肳��Ă��Ȃ��ꍇ�̑Ώ�
if (wallMaterial != null) {
    wall.GetComponent<Renderer>().material = wallMaterial;
} else {
    // �f�t�H���g�}�e���A���𓮓I����
    Material defaultWall = new Material(Shader.Find("Standard"));
    defaultWall.color = Color.blue;
    wall.GetComponent<Renderer>().material = defaultWall;
}
```

##### �΍�3: �����R���g���[���[�̎���
```csharp
public class SimplePlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    
    void Update() {
        // ��{�I�Ȉړ�����i3D�Ή��j
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical);
        
        if (direction.magnitude > 0.1f) {
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }
    
    void OnTriggerEnter(Collider other) {
        // 3D Collider�Ή��̏Փˌ��o
        if (other.GetComponent<SimpleDot>() != null) {
            Debug.Log("Dot collected!");
            Destroy(other.gameObject);
        }
    }
}
```

#### ?? ���݂̏�
- �C������r���h�G���[���p��
- �ڍׂȃG���[���O���擾����K�v������
- Unity �G�f�B�^�[��ł̒��ڊm�F���K�v

---

## ?? �ߋ��̂܂����|�C���g�Ɖ�����

### ���2: Git ���|�W�g��URL�̖��

#### ?? ����
```
remote: Repository not found.
fatal: repository 'https://github.com/shiraiwa-dev/pacman-unity.git/' not found
```

#### ?? ������
```bash
# �Ԉ���������[�g���폜
git remote remove origin

# �����������[�g��ǉ�
git remote add origin https://github.com/ShiraiwaDaichi/pacman-unity.git
```

#### ?? �w�K�|�C���g
- GitHub�̃��[�U�[���ƃ��|�W�g�����̐��m�����d�v
- �����[�gURL�̊m�F�R�}���h: `git remote -v`

### ���3: �v���n�u�ˑ��ɂ��w�K�̏��

#### ?? ����
- �w�K�҂�MazeGenerator���g�p���邽�߂Ƀv���n�u�쐬���K�v
- �v���n�u�쐬�̕��G���ō��܂���w�K�҂��\�z�����

#### ?? ������
- `SimpleMazeGenerator.cs`�̊J��
- �v���~�e�B�u�I�u�W�F�N�g�ɂ�铮�I����
- �i�K�I�w�K�̎���

#### ?? �w�K�|�C���g
- �w�K�҂̗�����l�������݌v�̏d�v��
- ���G�ȋ@�\��i�K�I�ɓ������鋳���@

---

## ??? �g���u���V���[�e�B���O�菇

### Unity �r���h�G���[�̒����菇

#### �X�e�b�v1: Unity �G�f�B�^�[�ł̊m�F
1. Unity �G�f�B�^�[���J��
2. Console �E�B���h�E���m�F
3. �G���[���b�Z�[�W�̏ڍׂ��L�^
4. Warning ���܂߂Ċm�F

#### �X�e�b�v2: �X�N���v�g�P�̃e�X�g
1. �V������̃V�[�����쐬
2. SimpleMazeGenerator �X�N���v�g�݂̂��e�X�g
3. �i�K�I�ɃR���|�[�l���g��ǉ�

#### �X�e�b�v3: �ˑ��֌W�̊m�F
```csharp
// �ˑ����Ă���N���X�̊m�F
- PacmanController.cs (2D�n)
- Dot.cs (2D�n)
- SimplePlayerController (3D�n)
- SimpleDot (3D�n)
```

#### �X�e�b�v4: �v���W�F�N�g�ݒ�̊m�F
- Project Settings �� Player �� Configuration
- 2D/3D�ݒ�̊m�F
- Physics�ݒ�̊m�F

### ��ʓI��Unity�G���[�p�^�[��

#### �p�^�[��1: NullReferenceException
```csharp
// �΍�: null �`�F�b�N
if (component != null) {
    component.DoSomething();
}
```

#### �p�^�[��2: Missing Component
```csharp
// �΍�: GetComponent �� null �`�F�b�N
var component = GetComponent<SomeComponent>();
if (component == null) {
    component = gameObject.AddComponent<SomeComponent>();
}
```

#### �p�^�[��3: �R���p�C���G���[
- ���O��Ԃ̕s��v
- �A�N�Z�X�C���q�̖��
- �z�Q��

---

## ?? �w�K�Ҍ����A�h�o�C�X

### ?? �܂��������̑Ώ��@

#### 1. �G���[���b�Z�[�W��ǂ�
- Console �E�B���h�E��K���m�F
- �G���[�̍s�ԍ����`�F�b�N
- Warning ���������Ȃ�

#### 2. �i�K�I�Ƀe�X�g
- �����ȒP�ʂœ���m�F
- ��x�ɑ����̕ύX�����Ȃ�
- ���삷��o�[�W������ۑ�

#### 3. �R�~���j�e�B�����p
- Unity �t�H�[�����Ŏ���
- Stack Overflow �Ō���
- GitHub Issues ���m�F

#### 4. �h�L�������g���Q��
- Unity�����h�L�������g
- C# �����h�L�������g
- �v���W�F�N�g�� TECHNICAL_LOG.md

### ?? �������̎v�l�v���Z�X

#### �X�e�b�v1: ���̓���
- �������҂���铮�삩�H
- ���ۂɉ����N���Ă��邩�H
- �������肪�����������H

#### �X�e�b�v2: �����̗���
- �l�����錴�������X�g�A�b�v
- �ł��\���̍����������猟��
- �����̌����̑g�ݍ��킹���l��

#### �X�e�b�v3: ���؂ƏC��
- �������������
- �ύX�̉e�����L�^
- ��������������𕶏���

---

## ?? ����̗\�h��

### �J���v���Z�X�̉��P

#### 1. �i�K�I�J��
- �@�\�������ȒP�ʂɕ���
- �e�i�K�œ���m�F
- �ύX�����̏ڍ׋L�^

#### 2. �e�X�g���̐���
- �ȒP�ȓ���m�F�p�V�[���̍쐬
- �����e�X�g�̓�������
- �p���I�C���e�O���[�V�����̐ݒ�

#### 3. �h�L�������g����
- �g���u���V���[�e�B���O�K�C�h�̏[��
- FAQ �Z�N�V�����̒ǉ�
- �w�K�҂���̃t�B�[�h�o�b�N���W

### �Z�p�I�ȗ\�h��

#### 1. �G���[�n���h�����O�̋���
```csharp
try {
    // ���X�N�̂��鏈��
    DoSomethingRisky();
} catch (System.Exception e) {
    Debug.LogError($"�G���[���������܂���: {e.Message}");
    // �t�H�[���o�b�N����
}
```

#### 2. �ݒ�̊O����
```csharp
[Header("Debug Settings")]
public bool enableDebugMode = true;
public bool verboseLogging = false;

void DebugLog(string message) {
    if (enableDebugMode && verboseLogging) {
        Debug.Log($"[{GetType().Name}] {message}");
    }
}
```

#### 3. �o�[�W�����Ǘ��̊��p
- ����I�ȃR�~�b�g
- �@�\�u�����`�̊��p
- �^�O�ɂ��}�C���X�g�[���Ǘ�

---

## ?? �܂����|�C���g�̕���

### �����p�x�̍������

#### 1��: ���ݒ�֘A (40%)
- Unity �o�[�W�����̈Ⴂ
- �v���W�F�N�g�ݒ�̕s��v
- �ˑ��֌W�̖��

#### 2��: �R�[�f�B���O�G���[ (30%)
- NullReferenceException
- �R���p�C���G���[
- ���W�b�N�G���[

#### 3��: ����s�� (20%)
- Unity �̎d�g�݂̗���s��
- C# �̊�{�I�ȊT�O�̕s����
- �Q�[���J�����L�̊T�O

#### 4��: �c�[������ (10%)
- Git �̑���~�X
- Unity �G�f�B�^�[�̑���~�X
- �ݒ�t�@�C���̕ҏW�~�X

### �������Ԃ̌X��

#### �����ɉ��� (< 10��): 25%
- �^�C�|�A�ݒ�~�X�Ȃ�

#### �Z���Ԃŉ��� (10-60��): 45%
- ��{�I�ȃG���[�A�h�L�������g�Q�Ƃŉ���

#### �����x�̎��� (1-4����): 20%
- ���G�ȃ��W�b�N�G���[�A�݌v�̌�����

#### ������ (> 4����): 10%
- ���{�I�Ȑ݌v���A���\�z���

---

## ?? �p���I���P

### ����̊J�����Ɋ��p���ׂ����P

1. **�����i�K�ł̃e�X�g���\�z**
   - SimpleMazeGenerator �̂悤�ȓ���m�F�c�[�����ŏ��ɍ쐬
   - �i�K�I�ȋ@�\�ǉ��Ŗ��𑁊�����

2. **�ڍׂȃG���[���O�̎���**
   - Debug.Log ��ϋɓI�Ɋ��p
   - �G���[���̏󋵂��ڍׂɋL�^

3. **�݊������l�������݌v**
   - 2D/3D �̍��݂������
   - �����V�X�e���Ƃ̐�������ۂ�

4. **�w�K�Җڐ��̌���**
   - ���ۂ̊w�K�҂ɂ�铮��m�F
   - �t�B�[�h�o�b�N�̎��W�Ɣ��f

### ?? ���̃A�N�V��������

#### �ً} (������)
- [ ] Unity �G�f�B�^�[�ł̒��ړI�ȃG���[�m�F
- [ ] SimpleMazeGenerator �̓���e�X�g
- [ ] �ڍׂȃG���[���O�̎擾

#### �Z�� (���T��)
- [ ] 2D/3D����̌����Ǝ���
- [ ] ����m�F�菇���̍쐬
- [ ] �w�K�Ҍ����g���u���V���[�e�B���O�K�C�h�̍X�V

#### ���� (������)
- [ ] �����e�X�g�V�X�e���̓���
- [ ] CI/CD �p�C�v���C���̍\�z
- [ ] �ڍׂȃf�o�b�O�c�[���̊J��

---

**���̂܂����|�C���g�L�^�́A�������ō����Ă���w�K�҂�J���҂̎Q�l�ɂȂ邱�Ƃ�����č쐬����܂����B�������̃v���Z�X���̂���؂Ȋw�K�o���ł��B** ??

**�ŐV�X�V**: 2025�N7��3�� - SimpleMazeGenerator �r���h�G���[������