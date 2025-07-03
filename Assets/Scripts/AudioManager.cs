// ===============================
// �I�[�f�B�I�}�l�[�W���[
// ===============================
// ���̃X�N���v�g�́A�Q�[�����̑S�Ẳ������ʂ��Ǘ����܂��B
// BGM�i�w�i���y�j�ƌ��ʉ��𕪂��Đ��䂵�A���ʒ�����~���[�g�@�\���񋟂��܂��B
// �V���O���g���p�^�[�����g�p���āA�Q�[���S�̂ň�̃C���X�^���X�̂ݑ��݂���悤�ɂ��܂��B

using UnityEngine;

// MonoBehaviour���p�����邱�ƂŁAUnity��GameObject�ɃA�^�b�`�ł���R���|�[�l���g�ɂȂ�܂�
public class AudioManager : MonoBehaviour
{
    // ===============================
    // �I�[�f�B�I�\�[�X�ݒ�iUnity�G�f�B�^�[�Őݒ肪�K�v�j
    // ===============================
    // AudioSource�́A�������Đ����邽�߂�Unity�R���|�[�l���g�ł�
    
    [Header("Audio Sources")]
    public AudioSource musicAudioSource;    // BGM�p�̃I�[�f�B�I�\�[�X
    public AudioSource sfxAudioSource;      // ���ʉ��p�̃I�[�f�B�I�\�[�X
    
    // BGM�ƌ��ʉ��𕪂��邱�ƂŁA���ꂼ��Ɨ����ĉ��ʐ����~���[�g���\�ɂȂ�܂�
    
    // ===============================
    // ���y�t�@�C���ݒ�iUnity�G�f�B�^�[�Őݒ肪�K�v�j
    // ===============================
    
    [Header("Music")]
    public AudioClip backgroundMusic;       // �Q�[������BGM
    public AudioClip gameOverMusic;         // �Q�[���I�[�o�[���̉��y
    public AudioClip winMusic;              // �������̉��y
    
    // ===============================
    // ���ʉ��t�@�C���ݒ�iUnity�G�f�B�^�[�Őݒ肪�K�v�j
    // ===============================
    
    [Header("Sound Effects")]
    public AudioClip dotSound;              // �h�b�g��H�ׂ����̉�
    public AudioClip powerPelletSound;      // �p���[�y���b�g��H�ׂ����̉�
    public AudioClip ghostEatenSound;       // �S�[�X�g��H�ׂ����̉�
    public AudioClip pacmanDeathSound;      // �p�b�N�}�������񂾎��̉�
    public AudioClip powerModeSound;        // �p���[���[�h�J�n���̉�
    
    // ===============================
    // ���ʐݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Settings")]
    public float musicVolume = 0.5f;        // BGM�̉��ʁi0.0�`1.0�j
    public float sfxVolume = 0.7f;          // ���ʉ��̉��ʁi0.0�`1.0�j
    
    // ===============================
    // �V���O���g���p�^�[���̎���
    // ===============================
    // �V���O���g���p�^�[���́A�N���X�̃C���X�^���X��1�������݂��邱�Ƃ�ۏ؂���݌v�p�^�[���ł�
    
    private static AudioManager instance;   // �B��̃C���X�^���X
    
    // �v���p�e�B���g�p���āA�O������C���X�^���X�ɃA�N�Z�X�ł���悤�ɂ��܂�
    public static AudioManager Instance
    {
        get
        {
            // �C���X�^���X�����݂��Ȃ��ꍇ�A�V�[��������T���܂�
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }
            return instance;
        }
    }
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    // Awake()�́AStart()���������Ăяo����܂�
    // �V���O���g���̏��������s���̂ɓK���Ă��܂�
    void Awake()
    {
        // �C���X�^���X�����ݒ�̏ꍇ�A���̃I�u�W�F�N�g���C���X�^���X�ɐݒ�
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad�ŁA�V�[�����ς���Ă��I�u�W�F�N�g��ێ�
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ���ɃC���X�^���X�����݂���ꍇ�A���̃I�u�W�F�N�g���폜
            // ����ɂ��A��Ɉ��AudioManager�݂̂����݂��邱�Ƃ�ۏ�
            Destroy(gameObject);
            return;
        }
    }
    
    // Start()�́A�I�u�W�F�N�g���쐬���ꂽ�ŏ��̃t���[���ň�x�����Ăяo����܂�
    void Start()
    {
        // BGM�p�I�[�f�B�I�\�[�X�̉��ʂ�ݒ�
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolume;
        }
        
        // ���ʉ��p�I�[�f�B�I�\�[�X�̉��ʂ�ݒ�
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = sfxVolume;
        }
        
        // �Q�[���J�n����BGM���Đ�
        PlayBackgroundMusic();
    }
    
    // ===============================
    // ���y�Đ����\�b�h�i�p�u���b�N�j
    // ===============================
    
    // �w�i���y���Đ����܂�
    public void PlayBackgroundMusic()
    {
        if (musicAudioSource != null && backgroundMusic != null)
        {
            musicAudioSource.clip = backgroundMusic;  // �Đ����鉹�y��ݒ�
            musicAudioSource.loop = true;             // ���[�v�Đ���L���ɂ���
            musicAudioSource.Play();                  // �Đ��J�n
        }
    }
    
    // �Q�[���I�[�o�[���y���Đ����܂�
    public void PlayGameOverMusic()
    {
        if (musicAudioSource != null && gameOverMusic != null)
        {
            musicAudioSource.clip = gameOverMusic;
            musicAudioSource.loop = false;            // ���[�v���Ȃ��i��x�����Đ��j
            musicAudioSource.Play();
        }
    }
    
    // �������y���Đ����܂�
    public void PlayWinMusic()
    {
        if (musicAudioSource != null && winMusic != null)
        {
            musicAudioSource.clip = winMusic;
            musicAudioSource.loop = false;            // ���[�v���Ȃ��i��x�����Đ��j
            musicAudioSource.Play();
        }
    }
    
    // ===============================
    // ���ʉ��Đ����\�b�h�i�p�u���b�N�j
    // ===============================
    
    // �h�b�g��H�ׂ����̉����Đ�
    public void PlayDotSound()
    {
        PlaySFX(dotSound);
    }
    
    // �p���[�y���b�g��H�ׂ����̉����Đ�
    public void PlayPowerPelletSound()
    {
        PlaySFX(powerPelletSound);
    }
    
    // �S�[�X�g��H�ׂ����̉����Đ�
    public void PlayGhostEatenSound()
    {
        PlaySFX(ghostEatenSound);
    }
    
    // �p