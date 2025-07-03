// ===============================
// �h�b�g
// ===============================
// ���̃X�N���v�g�́A�p�b�N�}�������W����h�b�g�̓���𐧌䂵�܂��B
// �h�b�g�͎��o�I�Ȍ��ʁi�A�j���[�V�����j��񋟂��A
// �p�b�N�}�����h�b�g�ɐG�ꂽ���̌��o���s���܂��B

using UnityEngine;

// MonoBehaviour���p�����邱�ƂŁAUnity��GameObject�ɃA�^�b�`�ł���R���|�[�l���g�ɂȂ�܂�
public class Dot : MonoBehaviour
{
    // ===============================
    // �A�j���[�V�����ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Animation")]
    public bool animateScale = true;        // �X�P�[���A�j���[�V������L���ɂ��邩�ǂ���
    public float animationSpeed = 2f;       // �A�j���[�V�����̑��x
    public float scaleRange = 0.2f;         // �X�P�[���ω��͈̔́i0�`1�̒l�j
    
    // ===============================
    // �v���C�x�[�g�ϐ��i������ԊǗ��j
    // ===============================
    
    private Vector3 originalScale;          // ���̃X�P�[���i�傫���j��ۑ�
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    // Start()�́A�I�u�W�F�N�g���쐬���ꂽ�ŏ��̃t���[���ň�x�����Ăяo����܂�
    void Start()
    {
        // �I�u�W�F�N�g�̌��̃X�P�[�����L�^
        // transform.localScale�́A�I�u�W�F�N�g�̌��݂̑傫����\���܂�
        originalScale = transform.localScale;
    }
    
    // Update()�́A���t���[���i�ʏ�1�b�Ԃ�60��j�Ăяo����܂�
    void Update()
    {
        // �A�j���[�V�������L���ȏꍇ�̂ݎ��s
        if (animateScale)
        {
            // ���Ԃ���ɂ��������I�ȕω����v�Z
            // Time.time�́A�Q�[���J�n����̌o�ߎ��ԁi�b�j
            // Mathf.Sin()�́A�T�C���֐����g�p����-1����1�̊ԂŊ��炩�ɕω�����l�𐶐�
            // ����ɂ��A�h�b�g���傫���Ȃ����菬�����Ȃ����肷����ʂ��쐬
            float scale = 1f + Mathf.Sin(Time.time * animationSpeed) * scaleRange;
            
            // �v�Z�����X�P�[���l�����ۂ̃I�u�W�F�N�g�ɓK�p
            // originalScale * scale�ŁA���̃T�C�Y����ɕω�������
            transform.localScale = originalScale * scale;
        }
    }
    
    // ===============================
    // �Փˌ��o���\�b�h
    // ===============================
    
    // ���̃I�u�W�F�N�g���g���K�[�R���C�_�[�ɐG�ꂽ���ɌĂяo����܂�
    // OnTriggerEnter2D�́A2D�Q�[���ł̏Փˌ��o���\�b�h�ł�
    void OnTriggerEnter2D(Collider2D other)
    {
        // �Փ˂������肪�v���C���[�i�p�b�N�}���j���ǂ������m�F
        if (other.CompareTag("Player"))
        {
            // ���ۂ̃h�b�g���W�����́APacmanController�X�N���v�g�ōs���܂�
            // ���̃X�N���v�g�ł́A�Փ˂̌��o�݂̂��s���A
            // ��̓I�ȏ����i���̍Đ��A�X�R�A���Z�A�I�u�W�F�N�g�̍폜�Ȃǁj��
            // PacmanController��CollectDot()���\�b�h�Ŏ��s����܂�
            
            // ���̐݌v�ɂ��A�Q�[�����W�b�N����ӏ��ɏW�񂵁A
            // �Ǘ����₷���R�[�h���쐬�ł��܂�
        }
    }
}