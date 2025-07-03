# �ύX�����h�L�������g

## ?? �v���W�F�N�g�T�v

**�v���W�F�N�g��**: Unity Pacman Learning Project  
**�ŏI�X�V��**: 2025�N7��3��  
**�o�[�W����**: v1.0.0  
**���|�W�g��**: https://github.com/ShiraiwaDaichi/pacman-unity

## ?? �v���W�F�N�g�̖ړI

���̃v���W�F�N�g�́A�v���O���~���O���S�҂�Unity��C#���g�����Q�[���J����i�K�I�Ɋw�K�ł��鋳�烊�\�[�X�Ƃ��ĊJ������܂����B

### ��v�ȓ���
- **���H�I�w�K**: ���ۂ̃p�b�N�}���Q�[�����J�����Ȃ���w�K
- **�i�K�I�A�v���[�`**: ��b���牞�p�܂�12�̉ۑ�ō\��
- **�ڍׂȃR�����g**: ���S�Ҍ����̒��J�ȉ���t���R�[�h
- **�i���Ǘ�**: �w�K�L�^�V�X�e���ɂ��p���I�Ȑ����x��

## ?? �J������

### ?? Phase 1: ��{�Q�[���V�X�e���̎���

#### ? ���������R���|�[�l���g

##### 1. MazeGenerator.cs
**�@�\**: ���H�̎��������V�X�e��
- **2�����z��ɂ����H��`**: 19x21�O���b�h�̖��H���C�A�E�g
- **�v���n�u�x�[�X�̐���**: �ǁA�h�b�g�A�p���[�y���b�g�A�v���C���[�A�S�[�X�g�̔z�u
- **�G�f�B�^�[�A�g**: Context Menu�ɂ��蓮�����@�\
- **���I�N���A�V�X�e��**: �����I�u�W�F�N�g�̎����폜

```csharp
// ���H���C�A�E�g��
private int[,] mazeLayout = new int[,] {
    // 0=��, 1=��, 2=�h�b�g, 3=�p���[�y���b�g, 4=�p�b�N�}��, 5=�S�[�X�g
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,1},
    // ... 21�s�̃��C�A�E�g��`
};
```

##### 2. PacmanController.cs
**�@�\**: �v���C���[�L�����N�^�[�̐���
- **���͏���**: WASD/���L�[�ɂ��ړ�����
- **�Փˌ��o**: �A�C�e�����W�ƕǂƂ̏Փˏ���
- **�A�j���[�V����**: �ړ������ɉ�������]����
- **�����A�g**: �ړ���A�C�e�����W���̌��ʉ�

##### 3. GhostController.cs
**�@�\**: AI�ɂ��S�[�X�g�̍s������
- **4�̏�ԊǗ�**: Chase(�ǐ�), Scatter(�U��), Scared(����), Eaten(���A)
- **�C���e���W�F���g�Ȉړ�**: �v���C���[�ʒu�Ɋ�Â��œK�o�H�I��
- **���I��ԕω�**: �R���[�`���ɂ�鎩����ԑJ��
- **���o�I�t�B�[�h�o�b�N**: ��Ԃɉ������F�ύX

```csharp
public enum GhostState {
    Chase,   // �v���C���[��ǐ�
    Scatter, // �����_���Ɉړ�
    Scared,  // �v���C���[���瓦��
    Eaten    // �X�^�[�g�n�_�ɕ��A
}
```

##### 4. GameManager.cs
**�@�\**: �Q�[���S�̂̏�ԊǗ�
- **�X�R�A�V�X�e��**: �h�b�g���W�ƃp���[�y���b�g����
- **���C�t�V�X�e��**: �v���C���[�̎c�@�Ǘ�
- **����/�s�k����**: �Q�[���I�������̔���
- **�p���[���[�h**: �S�[�X�g��H�ׂ��������

##### 5. UIManager.cs
**�@�\**: ���[�U�[�C���^�[�t�F�[�X�Ǘ�
- **���A���^�C���\��**: �X�R�A�A���C�t�A���x�����
- **�Q�[����ԕ\��**: �Q�[���I�[�o�[�A�������
- **�|�[�Y�@�\**: ESC�L�[�ɂ��Q�[���ꎞ��~
- **�{�^������**: ���X�^�[�g�A�I���@�\

##### 6. AudioManager.cs
**�@�\**: ���������V�X�e��
- **BGM�Ǘ�**: �w�i���y�̍Đ��A��~�A�t�F�[�h
- **���ʉ��V�X�e��**: �����Đ��Ή��̌��ʉ��Ǘ�
- **3D����**: �ʒu�x�[�X�̋�ԉ���
- **���ʐ���**: �}�X�^�[�ABGM�A���ʉ��̌ʒ���

##### 7. CameraController.cs
**�@�\**: �J��������V�X�e��
- **�X���[�Y�Ǐ]**: �v���C���[�̊��炩�Ȓǐ�
- **���E����**: ���H�͈͓��ł̃J�����ړ�
- **�Y�[���@�\**: �Q�[���󋵂ɉ��������_����

##### 8. PowerPellet.cs & Dot.cs
**�@�\**: ���W�A�C�e���V�X�e��
- **�����A�j���[�V����**: �X�P�[���ƐF�̓��I�ω�
- **���W����**: �g���K�[�x�[�X�̏Փˌ��o
- **���ʉ��A�g**: �A�C�e�����W���̉�������

### ?? Phase 2: �w�K�ۑ�V�X�e���̊J��

#### ? �쐬�����w�K���\�[�X

##### �ۑ�01: ��{�I��GameObject�ƃX�N���v�g�̍쐬
- **�w�K���e**: Unity��{����AC#�X�N���v�g�\���AMonoBehaviour
- **���H���e**: �ŏ��̃X�N���v�g�쐬�A�f�o�b�O�o�́A��{�I�ȕϐ�����
- **���莞��**: 30-45��

##### �ۑ�02: �v���C���[�̊�{�ړ��V�X�e��
- **�w�K���e**: Transform����AVector3�AInput�����ATime.deltaTime
- **���H���e**: �L�[���͂ɂ��ړ�����A���W�v�Z�A�ړ�����
- **���莞��**: 45-60��

##### �ۑ�03: �Փˌ��o�V�X�e���̎���
- **�w�K���e**: Collider�AOnTriggerEnter�A�^�O�V�X�e��
- **���H���e**: �A�C�e�����W�A��Q���ՓˁA�Փ˃C�x���g����
- **���莞��**: 60-75��

##### �ۑ�04: �Q�[����ԊǗ��V�X�e��
- **�w�K���e**: enum�A�V���O���g���p�^�[���A�X�e�[�g�}�V��
- **���H���e**: �Q�[����Ԃ̒�`�A��ԑJ�ځA�C�x���g�V�X�e��
- **���莞��**: 75-90��

##### �ۑ�05: UI�쐬�ƃC�x���g����
- **�w�K���e**: Unity UI�AButton�ASlider�A�C�x���g����
- **���H���e**: UI�v�f�̍쐬�A�f�[�^�o�C���f�B���O�A���X�|���V�u�f�U�C��
- **���莞��**: 60-75��

##### �ۑ�06: �����V�X�e���̎���
- **�w�K���e**: AudioSource�AAudioClip�A3D����
- **���H���e**: BGM�E���ʉ��Ǘ��A���ʐ���A��ԉ���
- **���莞��**: 45-60��

#### ?? �T�|�[�g�V�X�e��

##### Progress_Tracker.md
- **�@�\**: �w�K�i���̋L�^�ƊǗ�
- **���e**: 
  - �ۑ芮���`�F�b�N���X�g
  - �}�C���X�g�[���Ǘ�
  - �w�K���ԋL�^
  - �X�L���K����
  - �F��V�X�e��

##### �w�K�K�C�h���C��
- **�i�K�I�w�K�݌v**: ��b�����������p���㋉��4�i�K
- **���H�d��**: �R�[�h����͂ɂ��w�K���i
- **�G���[�΍�**: �悭����G���[�Ɖ������@�̖���
- **�ǉ��`�������W**: ������[�߂锭�W�ۑ�

### ??? Phase 3: �J���x���V�X�e��

#### ? SimpleMazeGenerator.cs
**�ړI**: �v���n�u�s�v�̓���m�F�V�X�e��

```csharp
// �v���~�e�B�u�I�u�W�F�N�g�ɂ����H����
void CreateWall(int x, int y) {
    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
    wall.transform.position = new Vector3(x, 0, -y);
    wall.tag = "Wall";
    // �}�e���A���ݒ�
}
```

**����**:
- **�����̓���m�F**: �v���n�u�쐬�s�v
- **�w�K�҃t�����h���[**: �ȒP�ȃZ�b�g�A�b�v
- **�i�K�I�ڍs**: �{�i�V�X�e���ւ̋��n��

#### ? �v���W�F�N�g�\���̍œK��

```
pacman-unity/
������ Assets/
��   ������ Scripts/           # �Q�[�����W�b�N
��   ������ Scenes/           # Unity�V�[��
������ Challenges/           # �w�K�ۑ�
��   ������ Challenge01-06/   # �i�K�I�ۑ�
��   ������ Progress_Tracker/ # �i���Ǘ�
������ ProjectSettings/      # Unity�ݒ�
������ .gitignore           # Git���O�ݒ�
������ README.md            # �v���W�F�N�g����
```

## ?? �Z�p�I�Ȑ���

### ?? �������ꂽ�݌v�p�^�[��

#### 1. �V���O���g���p�^�[��
```csharp
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
```

#### 2. �X�e�[�g�}�V���p�^�[��
```csharp
public enum GameState { MainMenu, Playing, Paused, GameOver }

public void ChangeState(GameState newState) {
    ExitState(currentState);
    currentState = newState;
    EnterState(newState);
}
```

#### 3. �I�u�U�[�o�[�p�^�[��
```csharp
public static System.Action<int> OnScoreChanged;
public static System.Action<GameState> OnStateChanged;
```

### ?? �p�t�H�[�}���X�œK��

#### �I�u�W�F�N�g�v�[�����O
- **�S�[�X�gAI**: �����I�Ȍo�H�v�Z
- **�����V�X�e��**: �����Đ������ɂ��œK��
- **UI�X�V**: �K�v���݂̂̍X�V����

#### �������Ǘ�
- **�K�؂�Destroy����**: �Q�[���I�u�W�F�N�g�̎����N���[���A�b�v
- **�C�x���g�w�ǉ���**: ���������[�N�h�~
- **�v���n�u�C���X�^���X��**: �����I�ȃI�u�W�F�N�g����

## ?? ����I���l

### ?? �w�K���ʂ̍ő剻

#### �i�K�I���G��
1. **��b**: GameObject����A��{�X�N���v�g
2. **����**: �V�X�e���A�g�A��ԊǗ�
3. **���p**: AI�����A�œK���Z�p
4. **�㋉**: �Ǝ��@�\�J��

#### ���H�I�ȃX�L���K��
- **Problem Solving**: �G���[�����\�͂̌���
- **Code Reading**: �R�����g�t���R�[�h�ɂ�闝�𑣐i
- **System Design**: �A�[�L�e�N�`���݌v�̊w�K
- **Debugging**: �f�o�b�O�Z�p�̏K��

### ?? �C�m�x�[�V�����v�f

#### 1. ��I�w�K�V�X�e��
- **���_�Ǝ��H�̗Z��**: �R�[�h��Əڍ׉��
- **�i������**: �w�K���`�x�[�V�����̈ێ�
- **�R�~���j�e�B�w��**: ���L�\�Ȋw�K���\�[�X

#### 2. ���S�Ҕz���݌v
- **���{��R�����g**: �ꍑ��ɂ�闝�𑣐i
- **�i�K�I��Փx**: ���܂��ɂ����w�K�J�[�u
- **�G���[�΍�**: �\�z�������̎��O����

#### 3. ���p���d��
- **���ۂ̃Q�[���J��**: ���삷��p�b�N�}���Q�[��
- **�ƊE�W���Z�p**: �����Ŏg�p�����Z�p�̏K��
- **�|�[�g�t�H���I**: �A�E�����ł̐��ʕ�

## ?? �Z�p�d�l

### �J����
- **Unity**: 2022.3.11f1 LTS
- **C# Version**: .NET Standard 2.1
- **Target Platform**: PC (Windows/Mac/Linux)

### �ˑ��֌W
- **Unity Standard Assets**: Core functionality
- **Unity UI System**: User interface
- **Unity Audio System**: Sound management
- **Unity Physics 2D**: Collision detection

### �R�[�h���g���N�X
- **���t�@�C����**: 26�t�@�C��
- **���s��**: 7,914�s
- **�X�N���v�g�t�@�C����**: 10�t�@�C��
- **�w�K�ۑ萔**: 6�ۑ�i+ 6�ۑ�̊g���\��j

## ?? ����̊g���v��

### Phase 4: ���x�ȋ@�\�����i�\��j

#### �ۑ�07-12�̊J��
- **�ۑ�07**: �A�j���[�V�����V�X�e��
- **�ۑ�08**: AI�i�l�H�m�\�j�̊�b����
- **�ۑ�09**: �Q�[���o�����X�����ƃf�o�b�O
- **�ۑ�10**: �@�\�g���ƍœK��
- **�ۑ�11**: �J�X�^���@�\�̊J��
- **�ۑ�12**: �Q�[�������ƃf�v���C

#### �Z�p�I�g��
- **�l�b�g���[�N�@�\**: �}���`�v���C���[�Ή�
- **�f�[�^�i����**: �Z�[�u/���[�h�@�\
- **���x���G�f�B�^�[**: �J�X�^�����H�쐬�c�[��
- **���̓V�X�e��**: �w�K�i���̏ڍו���

## ?? �v���W�F�N�g�̉e��

### ?? ����I�C���p�N�g
- **�w�K�҂̐���**: �i�K�I�ȃX�L���A�b�v������
- **����҂̎x��**: �\�������ꂽ���ނ̒�
- **�R�~���j�e�B�`��**: �w�K�ғ��m�̏�񋤗L���i

### ?? �Љ�I���l
- **�Z�p����̌���**: �v���O���~���O�w�K�̕~����������
- **�I�[�v���\�[�X�v��**: �����ŗ��p�\�ȋ��烊�\�[�X
- **���ۉ��Ή�**: �p��E���{��ł̊w�K�x��

### ?? �Z�p�I�v��
- **�x�X�g�v���N�e�B�X**: ���S�Ҍ����R�[�f�B���O�W��
- **�݌v�p�^�[��**: ���p�I�Ȑ݌v��@�̎���
- **�c�[���J��**: �w�K�x���c�[���̊J����@

## ?? �ύX�Ǘ�

### Git����
- **Initial Commit**: 2025-07-03
  - �R�~�b�g�n�b�V��: a89d208
  - 26�t�@�C���ǉ�
  - 7,914�s�̃R�[�h

### �o�[�W�����Ǘ��헪
- **main �u�����`**: ����ł̈ێ�
- **feature �u�����`**: �V�@�\�J���p
- **release �u�����`**: �����[�X�����p

### �ۑ�Ǘ�
- **GitHub Issues**: �o�O�񍐁E�@�\�v��
- **Project Board**: �J���i���̉���
- **Milestone**: �o�[�W���������[�X�Ǘ�

## ?? �v���W�F�N�g�̐���

### ? �B�����ꂽ�ڕW
1. **��I�Ȋw�K�V�X�e��**: ��b���牞�p�܂Œi�K�I�w�K
2. **��������Q�[��**: ���S�ɓ��삷��p�b�N�}���Q�[��
3. **�ڍׂȃh�L�������g**: ���S�Ҍ�������ƃK�C�h
4. **�i���Ǘ��V�X�e��**: �w�K���`�x�[�V�����ێ��@�\
5. **�I�[�v���\�[�X��**: GitHub�ł̌��J�Ƌ��L

### ?? ��ʓI����
- **�R�[�h�s��**: 7,914�s�̏ڍ׃R�����g�t��C#�R�[�h
- **�w�K�ۑ�**: 6�̒i�K�I�ۑ�i�g���\��12�ۑ�j
- **�h�L�������g**: ��I�Ȋw�K�K�C�h��API����
- **�v���W�F�N�g�\��**: 26�t�@�C���̐������ꂽ�\��

### ?? �萫�I���l
- **���S�҃t�����h���[**: ���܂��ɂ����w�K�݌v
- **���p��**: ���ۂ̃Q�[���J���Z�p�̏K��
- **�g����**: �p���I�ȋ@�\�ǉ����\
- **�R�~���j�e�B**: �w�K�ғ��m�̒m�����L���i

---

**���̃v���W�F�N�g�́A�v���O���~���O���S�҂��Q�[���J����ʂ��Ď��H�I�ȃX�L�����K���ł���A��I�ȋ��烊�\�[�X�Ƃ��Ċ������܂����B������p���I�ȉ��P�Ƌ@�\�g����ʂ��āA��葽���̊w�K�҂̐������x�����Ă����܂��B** ???