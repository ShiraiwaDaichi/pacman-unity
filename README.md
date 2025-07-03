# Pacman Unity Game

Unity �ō쐬���ꂽ�p�b�N�}���Q�[���ł��B

## �@�\

- �v���C���[�̃L�[�{�[�h����ɂ��ړ�
- �S�[�X�g��AI�i�ǐՁA�U���A������ԁj
- �h�b�g�ƃp���[�y���b�g�̎��W
- �X�R�A�V�X�e��
- ���C�t�V�X�e��
- �p���[���[�h�i�S�[�X�g��H�ׂ邱�Ƃ��ł���j
- �����E�s�k����
- �I�[�f�B�I�V�X�e��
- UI�Ǘ�

## �v���O���~���O���S�Ҍ������

### �ڍׂȃR�����g

�S�ẴX�N���v�g�t�@�C���ɁA�v���O���~���O���S�҂ł������ł���悤�ڍׂȃR�����g��ǉ����܂����F

- **��b�T�O�̐���**: �N���X�A���\�b�h�A�ϐ��A�p���Ȃǂ̊�{�I�ȃv���O���~���O�T�O
- **Unity���L�̋@�\**: MonoBehaviour�ATransform�AGameObject�A�R���|�[�l���g�Ȃ�
- **�Q�[���J���̊T�O**: �����蔻��A��ԊǗ��AUI����A�����V�X�e���Ȃ�
- **���w�I�T�O**: �x�N�g���v�Z�A�O�p�֐��A���ςȂǃQ�[���Ŏg�p����鐔�w
- **�݌v�p�^�[��**: �V���O���g���p�^�[���Ȃǂ̎��p�I�Ȑ݌v��@

### �w�K�̃|�C���g

1. **PacmanController.cs**: �v���C���[����̊�{
   - ���͏���
   - �ړ�����
   - �Փˌ��o
   - ��������

2. **GhostController.cs**: AI�i�l�H�m�\�j�̎���
   - ��ԊǗ��ienum�g�p�j
   - �x�N�g���v�Z�ɂ���������
   - �R���[�`���ɂ�鎞�ԊǗ�
   - ���I�ȍs���ω�

3. **GameManager.cs**: �Q�[���S�̂̊Ǘ�
   - �X�R�A�E���C�t�Ǘ�
   - �����E�s�k����
   - �p���[���[�h�̎���
   - �V�[���Ǘ�

4. **MazeGenerator.cs**: 2�����z����g�������H����
   - 2�����z��̊��p
   - �I�u�W�F�N�g�̓��I����
   - ���W�v�Z
   - �v���n�u�V�X�e��

5. **AudioManager.cs**: �����V�X�e���ƃV���O���g���p�^�[��
   - �݌v�p�^�[���̎���
   - ��������
   - �C���X�^���X�Ǘ�

6. **UIManager.cs**: ���[�U�[�C���^�[�t�F�[�X�Ǘ�
   - �C�x���g����
   - UI�v�f�̐���
   - ���Ԑ���i�|�[�Y�@�\�j

7. **CameraController.cs**: �J��������V�X�e��
   - ���炩�ȒǏ]
   - ���W����
   - �Y�[���@�\

## �g�p���@

### �v���C���[����
- **�ړ�**: ���L�[ �܂��� WASD �L�[
- **�|�[�Y**: Esc�L�[
- **���X�^�[�g**: R�L�[

### �Q�[���ݒ�

1. **Unity�Ńv���W�F�N�g���J��**
   - Unity Hub ����uOpen�v��I��
   - �v���W�F�N�g�t�H���_��I��

2. **�v���n�u�̍쐬**
   
   �K�v�ȃv���n�u���쐬���Ă��������F
   
   - **Wall**: �ǂ̃v���n�u
     - Box Collider 2D
     - Sprite Renderer�i�F�̐����`�j
     - Tag: "Wall"
     - Layer: "Wall"
   
   - **Dot**: �h�b�g�̃v���n�u
     - Circle Collider 2D (Is Trigger: true)
     - Sprite Renderer�i���F�̏������~�j
     - Tag: "Dot"
     - Dot�X�N���v�g
   
   - **PowerPellet**: �p���[�y���b�g�̃v���n�u
     - Circle Collider 2D (Is Trigger: true)
     - Sprite Renderer�i���F�̑傫���~�j
     - Tag: "PowerPellet"
     - PowerPellet�X�N���v�g
   
   - **Pacman**: �p�b�N�}���̃v���n�u
     - Circle Collider 2D (Is Trigger: true)
     - Sprite Renderer�i���F�̉~�j
     - Tag: "Player"
     - PacmanController�X�N���v�g
     - AudioSource
   
   - **Ghost**: �S�[�X�g�̃v���n�u
     - Box Collider 2D (Is Trigger: true)
     - Sprite Renderer�i�F�t���̐����`�j
     - Tag: "Ghost"
     - GhostController�X�N���v�g

3. **�V�[���ݒ�**
   - MainScene���J��
   - MazeGenerator�I�u�W�F�N�g���쐬
   - �e�v���n�u��MazeGenerator�Ɋ��蓖��
   - GameManager�I�u�W�F�N�g��UI�v�f�����蓖��

## �R���|�[�l���g����

### �X�N���v�g

- **PacmanController**: �v���C���[�̈ړ��ƃA�C�e�����W�𐧌�
- **GhostController**: �S�[�X�g��AI�Ə�ԊǗ�
- **GameManager**: �Q�[���S�̂̊Ǘ��i�X�R�A�A���C�t�A���������j
- **MazeGenerator**: ���H�̐���
- **UIManager**: UI�v�f�̊Ǘ�
- **AudioManager**: �I�[�f�B�I�̊Ǘ�
- **CameraController**: �J�����̒Ǐ]�ƃY�[��
- **Dot**: �h�b�g�̊�{����
- **PowerPellet**: �p���[�y���b�g�̊�{����

### �ݒ�

- **Tags**: Player, Ghost, Dot, PowerPellet, Wall
- **Layers**: Wall (8�Ԗڂ̃��C���[)
- **Sorting Layers**: Background, Maze, Items, Characters, UI

## �w�K���\�[�X

### �v���O���~���O��b���w�Ԃ��߂�

1. **C#�̊�{���@**: �N���X�A���\�b�h�A�ϐ��A��������A���[�v
2. **Unity��b**: GameObject�ATransform�A�R���|�[�l���g�A�V�[��
3. **�Q�[���J���T�O**: �����蔻��A��ԊǗ��A�C�x���g����
4. **���w**: �x�N�g���A���W�n�A�O�p�֐�

### �R�[�h�̓ǂݕ�

1. **�܂��̓R�����g��ǂ�**: �e�Z�N�V�����̐����𗝉�
2. **���\�b�h���Ƃɗ���**: ����̋@�\��c��
3. **�ϐ��̖�����c��**: �f�[�^�̗����ǐ�
4. **���ۂɓ������Ċm�F**: �ύX���Ă݂Č��ʂ��ώ@

## �l�܂����|�C���g

1. **�v���n�u�̍쐬**: �蓮�Ńv���n�u���쐬����K�v������܂��B
2. **�X�v���C�g�̐ݒ�**: �e�I�u�W�F�N�g�̃X�v���C�g��ݒ肷��K�v������܂��B
3. **�I�[�f�B�I�t�@�C��**: �����t�@�C����p�ӂ��Ċ��蓖�Ă�K�v������܂��B
4. **UI�v�f**: Canvas���UI�v�f��z�u���AGameManager�Ɋ��蓖�Ă�K�v������܂��B

## ����̉��Ǔ_

- �A�j���[�V�����̒ǉ�
- ��蕡�G�Ȗ��H���C�A�E�g
- �������x���̎���
- ���X�R�A�ۑ��@�\
- ���ڍׂȃG�t�F�N�g
- ���o�C���Ή�

## ���ӎ���

- ���̃v���W�F�N�g�͊�{�I�ȃt���[�����[�N�ł�
- ���ۂɃQ�[���𓮍삳����ɂ́A�v���n�u�̍쐬�ƃX�v���C�g�̐ݒ肪�K�v�ł�
- Unity 2022.3.11f1 �œ���m�F�ς�
- �S�ẴX�N���v�g�ɂ͏ڍׂȃR�����g���ǉ�����Ă���A�v���O���~���O���S�҂̊w�K�ɓK���Ă��܂�