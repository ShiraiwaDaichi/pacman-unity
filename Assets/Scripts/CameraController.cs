// ===============================
// �J�����R���g���[���[
// ===============================
// ���̃X�N���v�g�́A�Q�[�����̃J�����̓���𐧌䂵�܂��B
// �v���C���[�i�p�b�N�}���j�����炩�ɒǏ]���A�Y�[���@�\��񋟂��A
// �J�����̈ړ��͈͂𐧌�����@�\���܂�ł��܂��B

using UnityEngine;

// MonoBehaviour���p�����邱�ƂŁAUnity��GameObject�ɃA�^�b�`�ł���R���|�[�l���g�ɂȂ�܂�
public class CameraController : MonoBehaviour
{
    // ===============================
    // �J�����ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Camera Settings")]
    public Transform target;                    // �Ǐ]����Ώہi�ʏ�̓p�b�N�}���j
    public float smoothSpeed = 0.125f;          // �J�����̈ړ��̊��炩���i0�`1�A�������قǊ��炩�j
    public Vector3 offset = new Vector3(0, 0, -10);  // �J�����ƑΏۂƂ̋����i�I�t�Z�b�g�j
    
    // ===============================
    // �ړ��͈͐����ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Bounds")]
    public bool useBounds = true;               // �ړ��͈͐������g�p���邩�ǂ���
    public float minX = -10f;                   // X���̍ŏ��l
    public float maxX = 10f;                    // X���̍ő�l
    public float minY = -10f;                   // Y���̍ŏ��l
    public float maxY = 10f;                    // Y���̍ő�l
    
    // ===============================
    // �Y�[���ݒ�iUnity�G�f�B�^�[�Őݒ�\�j
    // ===============================
    
    [Header("Zoom")]
    public float zoomSpeed = 2f;                // �Y�[���̑��x
    public float minZoom = 3f;                  // �ŏ��Y�[���l�i�ł��߂��j
    public float maxZoom = 10f;                 // �ő�Y�[���l�i�ł������j
    
    // ===============================
    // �v���C�x�[�g�ϐ��i������ԊǗ��j
    // ===============================
    