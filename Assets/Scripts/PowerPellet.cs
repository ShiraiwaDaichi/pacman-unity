// ===============================
// �p���[�y���b�g
// ===============================
// ���̃X�N���v�g�́A�p�b�N�}�������W����p���[�y���b�g�̓���𐧌䂵�܂��B
// �p���[�y���b�g�͒ʏ�̃h�b�g�����傫���A���ʂȎ��o���ʂ������܂��B
// �p�b�N�}�����p���[�y���b�g��H�ׂ�ƁA�S�[�X�g��H�ׂ���悤�ɂȂ�܂��B

using UnityEngine;

// MonoBehaviour���p�����邱�ƂŁAUnity��GameObject�ɃA�^�b�`�ł���R���|�[�l���g�ɂȂ�܂�
public class PowerPellet : MonoBehaviour
{
    // ===============================
    // �A�j���[�V�����ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Animation")]
    public bool animateScale = true;        // �X�P�[���A�j���[�V������L���ɂ��邩�ǂ���
    public float animationSpeed = 4f;       // �A�j���[�V�����̑��x�i�ʏ�̃h�b�g��葬���j
    public float scaleRange = 0.3f;         // �X�P�[���ω��͈̔́i�ʏ�̃h�b�g���傫���j
    
    // ===============================
    // �_�Ō��ʐݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Blinking")]
    public bool blinkingEffect = true;      // �_�Ō��ʂ�L���ɂ��邩�ǂ���
    public float blinkSpeed = 3f;           // �_�ł̑��x
    
    // ===============================
    // �v���C�x�[�g�ϐ��i������ԊǗ��j
    // ===============================
    
    private Vector3 originalScale;          // ���̃X�P�[���i�傫���j��ۑ�
    private SpriteRenderer spriteRenderer; // �X�v���C�g�i�摜�j��\������R���|�[�l���g
    private Color originalColor;            // ���̐F��ۑ�
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    // Start()�́A�I�u�W�F�N�g���쐬���ꂽ�ŏ��̃t���[���ň�x�����Ăяo����܂�
    void Start()
    {
        // �I�u�W�F�N�g�̌��̃X�P�[�����L�^
        originalScale = transform.localScale;
        
        // SpriteRenderer�R���|�[�l���g���擾
        // ���̃R���|�[�l���g�́A2D�Q�[���ŉ摜��\�����邽�߂Ɏg�p����܂�
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // �X�v���C�g�����_���[�����݂���ꍇ�A���̐F���L�^
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    // Update()�́A���t���[���i�ʏ�1�b�Ԃ�60��j�Ăяo����܂�
    void Update()
    {
        // �X�P�[���A�j���[�V�����̏���
        if (animateScale)
        {
            // ���Ԃ���ɂ��������I�ȕω����v�Z
            // Time.time�́A�Q�[���J�n����̌o�ߎ��ԁi�b�j
            // Mathf.Sin()�́A�T�C���֐����g�p����-1����1�̊ԂŊ��炩�ɕω�����l�𐶐�
            // �p���[�y���b�g�͒ʏ�̃h�b�g��葬���A�傫���ω����܂�
            float scale = 1f + Mathf.Sin(Time.time * animationSpeed) * scaleRange;
            
            // �v�Z�����X�P�[���l�����ۂ̃I�u�W�F�N�g�ɓK�p
            transform.localScale = originalScale * scale;
        }
        
        // �_�Ō��ʂ̏���
        if (blinkingEffect && spriteRenderer != null)
        {
            // �����x�i�A���t�@�l�j�����ԂɊ�Â��ĕω�������
            // 0.5f + Mathf.Sin() * 0.5f�ɂ��A0.0�`1.0�͈̔͂ŕω�
            // ����ɂ��A�p���[�y���b�g���_�ł��Č����܂�
            float alpha = 0.5f + Mathf.Sin(Time.time * blinkSpeed) * 0.5f;
            
            // �V�����F���쐬�iRGB�l�͌��̐F���ێ����A�A���t�@�l�̂ݕύX�j
            // Color(r, g, b, a)�ŐF���w��
            // r = ��, g = ��, b = ��, a = �����x�i0=���S����, 1=���S�s�����j
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
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
            // ���ۂ̃p���[�y���b�g���W�����́APacmanController�X�N���v�g�ōs���܂�
            // ���̃X�N���v�g�ł́A�Փ˂̌��o�݂̂��s���A
            // ��̓I�ȏ����i���̍Đ��A�X�R�A���Z�A�p���[���[�h�J�n�A�I�u�W�F�N�g�̍폜�Ȃǁj��
            // PacmanController��CollectPowerPellet()���\�b�h�Ŏ��s����܂�
            
            // ���̐݌v�ɂ��A�Q�[�����W�b�N����ӏ��ɏW�񂵁A
            // �Ǘ����₷���R�[�h���쐬�ł��܂�
            
            // �p���[�y���b�g�̓��ʂȌ��ʁF
            // - �ʏ�̃h�b�g��葽���̃X�R�A���l��
            // - �p���[���[�h���J�n�i�S�[�X�g��H�ׂ���悤�ɂȂ�j
            // - ���ʂȌ��ʉ����Đ�
        }
    }
}