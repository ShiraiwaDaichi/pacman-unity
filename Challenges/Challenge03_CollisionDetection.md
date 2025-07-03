# �ۑ�03: �Փˌ��o�V�X�e���̎���

## ?? �w�K�ڕW
- Collider �R���|�[�l���g�̗����Ǝg�p
- Trigger �� Collision �̈Ⴂ���w��
- OnTriggerEnter/Exit �C�x���g�̎���
- �^�O�V�X�e�����g�����I�u�W�F�N�g����
- �Փˎ��̏����ƃQ�[�����W�b�N

## ? ���菊�v����
�� 60-75 ��

## ?? �O��m��
- �ۑ�02�̊���
- GameObject �� Component �̊T�O
- ���\�b�h�ƈ����̗���

## ?? �ۑ���e

### �X�e�b�v1: �Փˌ��o�p�I�u�W�F�N�g�̏���

#### 1.1 �v���C���[�̐ݒ�
1. �ۑ�02�ō쐬����Player�I�u�W�F�N�g��I��
2. Box Collider �R���|�[�l���g��ǉ�
3. Is Trigger �Ƀ`�F�b�N������
4. Tag �� "Player" �ɐݒ�

#### 1.2 ���W�A�C�e���̍쐬
1. Create �� 3D Object �� Sphere �ŋ��̂��쐬
2. ���O�� "CollectibleItem" �ɕύX
3. �ʒu�� (3, 0, 0) �ɐݒ�
4. Sphere Collider �� Is Trigger �Ƀ`�F�b�N
5. �V�����^�O "Collectible" ���쐬���Đݒ�

#### 1.3 ��Q���̍쐬
1. Create �� 3D Object �� Cube �ŗ����̂��쐬
2. ���O�� "Obstacle" �ɕύX
3. �ʒu�� (-3, 0, 0) �ɐݒ�
4. Box Collider �� Is Trigger �� **�`�F�b�N���Ȃ�**
5. �V�����^�O "Obstacle" ���쐬���Đݒ�

### �X�e�b�v2: �Փˌ��o�X�N���v�g�̍쐬

�ȉ��̃X�N���v�g�� **�����œ���** ���Ă��������F

```csharp
using UnityEngine;

// �Փˌ��o�Ə������s���X�N���v�g
public class CollisionDetector : MonoBehaviour
{
    // ===============================
    // �p�u���b�N�ϐ��i�ݒ�\�j
    // ===============================
    
    [Header("�Փˌ��o�ݒ�")]
    public bool enableCollisionDetection = true;    // �Փˌ��o��L���ɂ��邩
    public bool showDebugMessages = true;           // �f�o�b�O���b�Z�[�W��\�����邩
    public float collisionCooldown = 1.0f;          // �Փˌ��o�̃N�[���_�E������
    
    [Header("�Q�[���ݒ�")]
    public int score = 0;                          // �v���C���[�̃X�R�A
    public int health = 100;                       // �v���C���[�̃w���X
    public int maxHealth = 100;                    // �ő�w���X
    
    [Header("�A�C�e���ݒ�")]
    public int itemValue = 10;                     // �A�C�e���̉��l
    public int healthRecovery = 20;                // �w���X�񕜗�
    public int obstacleDamage = 25;                // ��Q���̃_���[�W
    
    [Header("�G�t�F�N�g�ݒ�")]
    public Color normalColor = Color.white;         // �ʏ펞�̐F
    public Color damageColor = Color.red;           // �_���[�W���̐F
    public Color healColor = Color.green;           // �񕜎��̐F
    
    // ===============================
    // �v���C�x�[�g�ϐ��i���������p�j
    // ===============================
    
    private float lastCollisionTime = 0f;           // �Ō�̏Փˎ���
    private int collisionCount = 0;                 // �Փˉ�
    private Renderer objectRenderer;                // �I�u�W�F�N�g��Renderer
    private Color originalColor;                    // ���̐F
    
    // �A�C�e�����W�̓��v
    private int totalItemsCollected = 0;
    private int totalDamageTaken = 0;
    private int totalHealthRecovered = 0;
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    void Start()
    {
        // �����ݒ�
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        
        // ������Ԃ̕\��
        Debug.Log("=== �Փˌ��o�V�X�e���J�n ===");
        Debug.Log("�����X�R�A: " + score);
        Debug.Log("�����w���X: " + health + "/" + maxHealth);
        
        // ������@��\��
        ShowControlInfo();
    }
    
    void Update()
    {
        // ����L�[�̏���
        HandleSpecialKeys();
        
        // �F�̎������A
        RestoreOriginalColor();
    }
    
    // ===============================
    // �Փˌ��o���\�b�h�iTrigger�^�C�v�j
    // ===============================
    
    // ���̃I�u�W�F�N�g��Trigger�ɓ�������
    void OnTriggerEnter(Collider other)
    {
        // �Փˌ��o�������ȏꍇ�͏������Ȃ�
        if (!enableCollisionDetection) return;
        
        // �N�[���_�E�����Ԓ��͏������Ȃ�
        if (Time.time - lastCollisionTime < collisionCooldown) return;
        
        // �Փ˂����I�u�W�F�N�g�̃^�O�ɂ���ď����𕪊�
        switch (other.tag)
        {
            case "Collectible":
                HandleCollectibleCollision(other);
                break;
            case "Obstacle":
                HandleObstacleCollision(other);
                break;
            case "Player":
                HandlePlayerCollision(other);
                break;
            default:
                HandleUnknownCollision(other);
                break;
        }
        
        // �Փˎ��Ԃ��L�^
        lastCollisionTime = Time.time;
        collisionCount++;
        
        // ���v�����X�V
        UpdateStatistics();
    }
    
    // ���̃I�u�W�F�N�g��Trigger����o����
    void OnTriggerExit(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log("�I�u�W�F�N�g���E: " + other.name + " (�^�O: " + other.tag + ")");
        }
    }
    
    // ���̃I�u�W�F�N�g��Trigger���ɂ���ԁi���t���[���j
    void OnTriggerStay(Collider other)
    {
        // �p���I�Ȍ��ʂ��K�v�ȏꍇ�ɂ����ɏ���������
        // ��F�Ń_���[�W�A�񕜃G���A�Ȃ�
    }
    
    // ===============================
    // �Փˌ��o���\�b�h�iCollision�^�C�v�j
    // ===============================
    
    // �����I�ȏՓ˂�����������
    void OnCollisionEnter(Collision collision)
    {
        if (showDebugMessages)
        {
            Debug.Log("�����Փ˔���: " + collision.gameObject.name);
            Debug.Log("�Փ˗�: " + collision.relativeVelocity.magnitude);
        }
        
        // �Փ˂̋����ɉ���������
        float impactForce = collision.relativeVelocity.magnitude;
        if (impactForce > 5.0f)
        {
            Debug.Log("�����ՓˁI�_���[�W���󂯂܂���");
            TakeDamage((int)(impactForce * 2));
        }
    }
    
    // ===============================
    // �ʂ̏Փˏ������\�b�h
    // ===============================
    
    // ���W�A�C�e���Ƃ̏Փˏ���
    void HandleCollectibleCollision(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log("�A�C�e�����W: " + other.name);
        }
        
        // �X�R�A�𑝉�
        score += itemValue;
        totalItemsCollected++;
        
        // �w���X����
        RecoverHealth(healthRecovery);
        
        // ���o�I�t�B�[�h�o�b�N
        ChangeColorTemporarily(healColor);
        
        // �A�C�e�����폜
        Destroy(other.gameObject);
        
        Debug.Log("�A�C�e�����W�I �X�R�A: " + score + " (+" + itemValue + ")");
    }
    
    // ��Q���Ƃ̏Փˏ���
    void HandleObstacleCollision(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log("��Q���ɏՓ�: " + other.name);
        }
        
        // �_���[�W���󂯂�
        TakeDamage(obstacleDamage);
        
        // ���o�I�t�B�[�h�o�b�N
        ChangeColorTemporarily(damageColor);
        
        Debug.Log("��Q���ɏՓˁI�_���[�W: " + obstacleDamage);
    }
    
    // �v���C���[�Ƃ̏Փˏ���
    void HandlePlayerCollision(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log("�v���C���[�ƐڐG: " + other.name);
        }
        
        // �v���C���[���m�̓��ꏈ��������΂����ɋL�q
    }
    
    // �s���ȃI�u�W�F�N�g�Ƃ̏Փˏ���
    void HandleUnknownCollision(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log("�s���ȃI�u�W�F�N�g�ƏՓ�: " + other.name + " (�^�O: " + other.tag + ")");
        }
    }
    
    // ===============================
    // �Q�[�����W�b�N�������\�b�h
    // ===============================
    
    // �_���[�W���󂯂鏈��
    void TakeDamage(int damage)
    {
        health -= damage;
        totalDamageTaken += damage;
        
        // �w���X��0�ȉ��ɂȂ�Ȃ��悤�ɂ���
        health = Mathf.Max(0, health);
        
        Debug.Log("�_���[�W: " + damage + " (�c��w���X: " + health + ")");
        
        // �w���X��0�ɂȂ�����
        if (health <= 0)
        {
            HandleGameOver();
        }
    }
    
    // �w���X���񕜂��鏈��
    void RecoverHealth(int recovery)
    {
        health += recovery;
        totalHealthRecovered += recovery;
        
        // �ő�w���X�𒴂��Ȃ��悤�ɂ���
        health = Mathf.Min(maxHealth, health);
        
        Debug.Log("�w���X��: " + recovery + " (���݃w���X: " + health + ")");
    }
    
    // �Q�[���I�[�o�[����
    void HandleGameOver()
    {
        Debug.Log("=== �Q�[���I�[�o�[ ===");
        Debug.Log("�ŏI�X�R�A: " + score);
        Debug.Log("���W�A�C�e����: " + totalItemsCollected);
        Debug.Log("���_���[�W: " + totalDamageTaken);
        
        // �Q�[���I�[�o�[���̏���
        enableCollisionDetection = false;
        
        // �F��ύX
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.black;
        }
    }
    
    // ===============================
    // ���o�I�t�B�[�h�o�b�N����
    // ===============================
    
    // �ꎞ�I�ɐF��ύX
    void ChangeColorTemporarily(Color newColor)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = newColor;
        }
    }
    
    // ���̐F�ɖ߂�
    void RestoreOriginalColor()
    {
        if (objectRenderer != null && Time.time - lastCollisionTime > 0.5f)
        {
            objectRenderer.material.color = originalColor;
        }
    }
    
    // ===============================
    // ���v�Ə��\��
    // ===============================
    
    // ���v�����X�V
    void UpdateStatistics()
    {
        if (collisionCount % 5 == 0)  // 5��Փ˂��Ƃɕ\��
        {
            ShowStatistics();
        }
    }
    
    // ���v����\��
    void ShowStatistics()
    {
        Debug.Log("=== ���v��� ===");
        Debug.Log("�Փˉ�: " + collisionCount);
        Debug.Log("���݃X�R�A: " + score);
        Debug.Log("���݃w���X: " + health + "/" + maxHealth);
        Debug.Log("�A�C�e�����W��: " + totalItemsCollected);
        Debug.Log("���_���[�W: " + totalDamageTaken);
        Debug.Log("���񕜗�: " + totalHealthRecovered);
    }
    
    // ������@��\��
    void ShowControlInfo()
    {
        Debug.Log("=== ������@ ===");
        Debug.Log("�ړ�: WASD");
        Debug.Log("���\��: I");
        Debug.Log("���Z�b�g: R");
        Debug.Log("�Փˌ��oON/OFF: C");
        Debug.Log("�΂̋�: �A�C�e���i�X�R�A+�񕜁j");
        Debug.Log("������: ��Q���i�_���[�W�j");
    }
    
    // ===============================
    // ����L�[����
    // ===============================
    
    void HandleSpecialKeys()
    {
        // I �L�[: ���\��
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowStatistics();
        }
        
        // R �L�[: ���v���Z�b�g
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetStatistics();
        }
        
        // C �L�[: �Փˌ��oON/OFF
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCollisionDetection();
        }
        
        // H �L�[: �w���v
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowControlInfo();
        }
    }
    
    // ���v�����Z�b�g
    void ResetStatistics()
    {
        score = 0;
        health = maxHealth;
        collisionCount = 0;
        totalItemsCollected = 0;
        totalDamageTaken = 0;
        totalHealthRecovered = 0;
        enableCollisionDetection = true;
        
        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
        
        Debug.Log("=== ���v���Z�b�g���� ===");
    }
    
    // �Փˌ��o�̗L��/������؂�ւ�
    void ToggleCollisionDetection()
    {
        enableCollisionDetection = !enableCollisionDetection;
        string status = enableCollisionDetection ? "�L��" : "����";
        Debug.Log("�Փˌ��o: " + status);
    }
}
```

### �X�e�b�v3: �X�N���v�g�̓K�p�ƃe�X�g

1. Player�I�u�W�F�N�g��CollisionDetector�X�N���v�g���A�^�b�`
2. Inspector �Őݒ���m�F
3. �Q�[�������s���ăe�X�g�F
   - �΂̋��́i�A�C�e���j�ɋ߂Â�
   - �����́i��Q���j�ɏՓ˂���
   - �e��L�[������

### �X�e�b�v4: �����̃A�C�e�����쐬

1. CollectibleItem�𕡐�����3-4�z�u
2. �قȂ�ʒu�ɔz�u
3. ���ꂼ��قȂ�F�̃}�e���A����ݒ�i�I�v�V�����j

## ?? �������Ă݂悤

### ����1: �Փːݒ�̕ύX

```csharp
// �قȂ�Փːݒ������
public class CollisionExperiment : MonoBehaviour
{
    void Start()
    {
        // Is Trigger �� ON/OFF ��؂�ւ��ē�����m�F
        Collider col = GetComponent<Collider>();
        
        // ����1: Trigger�Ƃ��Ďg�p
        col.isTrigger = true;
        
        // ����2: �����Փ˂Ƃ��Ďg�p
        col.isTrigger = false;
    }
}
```

### ����2: �قȂ�A�C�e���^�C�v

```csharp
// �A�C�e���^�C�v�ɉ���������
void HandleCollectibleCollision(Collider other)
{
    // �A�C�e���̖��O�ɂ���ď�����ς���
    switch (other.name)
    {
        case "HealthPotion":
            RecoverHealth(50);
            break;
        case "ScoreGem":
            score += 100;
            break;
        case "SpeedBoost":
            // ���x�A�b�v����
            StartCoroutine(SpeedBoostEffect());
            break;
    }
}

IEnumerator SpeedBoostEffect()
{
    // 5�b�ԑ��x�A�b�v
    GetComponent<BasicPlayerMovement>().moveSpeed *= 2;
    yield return new WaitForSeconds(5f);
    GetComponent<BasicPlayerMovement>().moveSpeed /= 2;
}
```

### ����3: �͈͌��o�V�X�e��

```csharp
// �͈͓��̃I�u�W�F�N�g�����o
void DetectObjectsInRange()
{
    Collider[] objectsInRange = Physics.OverlapSphere(transform.position, 5.0f);
    
    foreach (Collider obj in objectsInRange)
    {
        if (obj.CompareTag("Collectible"))
        {
            Debug.Log("�͈͓��ɃA�C�e������: " + obj.name);
        }
    }
}
```

## ? �悭����G���[

### �G���[1: �Փ˂����o����Ȃ�
**����**: 
- Collider �R���|�[�l���g���s��
- �^�O���������ݒ肳��Ă��Ȃ�
- Is Trigger �̐ݒ�~�X

**�������@**:
- �����̃I�u�W�F�N�g��Collider�����邩�m�F
- �^�O�̐��m�Ȑݒ���m�F
- OnTriggerEnter�g�p����Is Trigger�Ƀ`�F�b�N

### �G���[2: ������Փ˂��Ă��܂�
**����**: OnTriggerStay ���A���ŌĂ΂��
**�������@**: �N�[���_�E�����Ԃ�ݒ�

### �G���[3: �����Փ˂��@�\���Ȃ�
**����**: Rigidbody �R���|�[�l���g���s��
**�������@**: ���Ȃ��Ƃ��Е��̃I�u�W�F�N�g��Rigidbody��ǉ�

## ?? �w�K�|�C���g

### �d�v�ȊT�O
1. **Collider**: �Փˌ��o�̋��E
2. **Trigger vs Collision**: ���o���@�̈Ⴂ
3. **Tag System**: �I�u�W�F�N�g�̕���
4. **Event-Driven Programming**: �C�x���g�x�[�X�̏���
5. **Object State Management**: �I�u�W�F�N�g�̏�ԊǗ�

### �Փˌ��o�̎��
- **OnTriggerEnter**: Trigger�̈�ɓ�������
- **OnTriggerExit**: Trigger�̈悩��o����
- **OnTriggerStay**: Trigger�̈���ɂ����
- **OnCollisionEnter**: �����I�Փ˔�����
- **OnCollisionExit**: �����I�ՓˏI����
- **OnCollisionStay**: �����I�Փˌp����

## ?? ���̃X�e�b�v

���̉ۑ芮����F
1. **����x�`�F�b�N**: �e�Փ˃C�x���g�̎g������������ł��邩
2. **����**: �قȂ�Փːݒ�ł̓���m�F
3. **���̉ۑ�**: �ۑ�04�u�Q�[����ԊǗ��V�X�e���v

## ?? �ǉ��`�������W

### �`�������W1: ���G�ȃA�C�e���V�X�e��
```csharp
[System.Serializable]
public class Item
{
    public string name;
    public int value;
    public int healthEffect;
    public float duration;
    public Color effectColor;
}

public Item[] availableItems;
```

### �`�������W2: �p�[�e�B�N���G�t�F�N�g
```csharp
public ParticleSystem collectEffect;
public ParticleSystem damageEffect;

void PlayCollectEffect()
{
    if (collectEffect != null)
    {
        collectEffect.Play();
    }
}
```

### �`�������W3: ��������
```csharp
public AudioClip collectSound;
public AudioClip damageSound;
private AudioSource audioSource;

void PlaySound(AudioClip clip)
{
    if (audioSource != null && clip != null)
    {
        audioSource.PlayOneShot(clip);
    }
}
```

�����ł��I?? 
�Փˌ��o�V�X�e�����}�X�^�[���܂����I