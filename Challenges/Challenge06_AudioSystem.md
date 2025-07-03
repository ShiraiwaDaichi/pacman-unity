# �ۑ�06: �����V�X�e���̎���

## ?? �w�K�ڕW
- Unity Audio System �̗����Ǝ���
- AudioSource �� AudioClip �̎g�p���@
- �������ʂ�BGM�̊Ǘ��V�X�e��
- 3D������Spatial Audio �̊�b
- ���ʐ���ƃ~�L�V���O�Z�p

## ? ���菊�v����
�� 45-60 ��

## ?? �O��m��
- �ۑ�05�̊���
- Audio �R���|�[�l���g�̊�{�m��
- �R���[�`���̗���

## ?? �ۑ���e

### �X�e�b�v1: �������\�[�X�̏���

#### 1.1 �t�H���_�\��
```
Assets/
������ Audio/
��   ������ BGM/
��   ������ SFX/
��   ������ Voice/
������ Scripts/
    ������ Audio/
```

#### 1.2 �I�[�f�B�I�t�@�C���̏���
- BGM�p: .mp3 �܂��� .wav �t�@�C��
- ���ʉ��p: .wav �t�@�C���i�Z���ԁj
- �ݒ�: Import Settings �� Audio �ɐݒ�

### �X�e�b�v2: �����Ǘ��V�X�e���̍쐬

�ȉ��̃X�N���v�g�� **�����œ���** ���Ă��������F

```csharp
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// �����V�X�e���̓����Ǘ��N���X
public class AudioManager : MonoBehaviour
{
    // ===============================
    // �V���O���g���p�^�[��
    // ===============================
    
    public static AudioManager Instance { get; private set; }
    
    // ===============================
    // �����ݒ�
    // ===============================
    
    [Header("�����ݒ�")]
    public float masterVolume = 1.0f;           // �}�X�^�[����
    public float musicVolume = 0.8f;            // BGM����
    public float sfxVolume = 1.0f;              // ���ʉ�����
    public float voiceVolume = 1.0f;            // �{�C�X����
    
    [Header("BGM�ݒ�")]
    public AudioClip[] backgroundMusic;         // BGM�z��
    public bool loopBGM = true;                 // BGM���[�v�ݒ�
    public float bgmFadeTime = 2.0f;            // BGM�t�F�[�h����
    
    [Header("���ʉ��ݒ�")]
    public AudioClip[] soundEffects;           // ���ʉ��z��
    public int maxSFXSources = 10;             // �ő���ʉ������Đ���
    
    [Header("3D�����ݒ�")]
    public bool use3DAudio = true;              // 3D�����g�p
    public float maxDistance = 20f;             // �ő啷�����鋗��
    public float minDistance = 1f;              // �ŏ�����
    
    // ===============================
    // �I�[�f�B�I�\�[�X
    // ===============================
    
    private AudioSource bgmSource;              // BGM��pAudioSource
    private AudioSource[] sfxSources;           // ���ʉ��pAudioSource�z��
    private AudioSource voiceSource;            // �{�C�X��pAudioSource
    
    // ===============================
    // �����Ǘ��ϐ�
    // ===============================
    
    private Dictionary<string, AudioClip> audioClips;  // �����N���b�v����
    private Dictionary<string, float> lastPlayTime;    // �ŏI�Đ����ԋL�^
    private int currentSFXIndex = 0;                   // ���݂̌��ʉ��C���f�b�N�X
    private bool isMuted = false;                      // �~���[�g���
    
    // �t�F�[�h�����p
    private Coroutine bgmFadeCoroutine;
    private bool isFading = false;
    
    // ===============================
    // �������ʂ̒�`
    // ===============================
    
    public enum SoundType
    {
        BGM,
        SFX,
        Voice
    }
    
    public enum SFXType
    {
        ButtonClick,
        ItemCollect,
        EnemyHit,
        PlayerHurt,
        PowerUp,
        GameOver,
        Victory,
        Jump,
        Footstep,
        Explosion
    }
    
    public enum BGMType
    {
        MainMenu,
        GamePlay,
        GameOver,
        Victory,
        Boss
    }
    
    // ===============================
    // Unity�W�����\�b�h
    // ===============================
    
    void Awake()
    {
        // �V���O���g���p�^�[���̎���
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        LoadAudioSettings();
        Debug.Log("=== �����V�X�e���J�n ===");
    }
    
    void Update()
    {
        // �f�o�b�O�p�L�[����
        HandleDebugInput();
    }
    
    // ===============================
    // ���������\�b�h
    // ===============================
    
    void InitializeAudioSystem()
    {
        // BGM�\�[�X�̍쐬
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = loopBGM;
        bgmSource.playOnAwake = false;
        bgmSource.volume = musicVolume * masterVolume;
        
        // ���ʉ��\�[�X�̍쐬
        sfxSources = new AudioSource[maxSFXSources];
        for (int i = 0; i < maxSFXSources; i++)
        {
            sfxSources[i] = gameObject.AddComponent<AudioSource>();
            sfxSources[i].playOnAwake = false;
            sfxSources[i].loop = false;
            sfxSources[i].volume = sfxVolume * masterVolume;
        }
        
        // �{�C�X�\�[�X�̍쐬
        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.playOnAwake = false;
        voiceSource.volume = voiceVolume * masterVolume;
        
        // �����N���b�v�����̏�����
        audioClips = new Dictionary<string, AudioClip>();
        lastPlayTime = new Dictionary<string, float>();
        
        // �����N���b�v�̓o�^
        RegisterAudioClips();
    }
    
    void RegisterAudioClips()
    {
        // BGM�̓o�^
        for (int i = 0; i < backgroundMusic.Length; i++)
        {
            if (backgroundMusic[i] != null)
            {
                audioClips[backgroundMusic[i].name] = backgroundMusic[i];
            }
        }
        
        // ���ʉ��̓o�^
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundEffects[i] != null)
            {
                audioClips[soundEffects[i].name] = soundEffects[i];
            }
        }
        
        Debug.Log($"�����N���b�v�o�^����: {audioClips.Count}��");
    }
    
    // ===============================
    // BGM���䃁�\�b�h
    // ===============================
    
    public void PlayBGM(string clipName, bool fadeIn = true)
    {
        if (audioClips.ContainsKey(clipName))
        {
            PlayBGM(audioClips[clipName], fadeIn);
        }
        else
        {
            Debug.LogWarning($"BGM '{clipName}' ��������܂���");
        }
    }
    
    public void PlayBGM(AudioClip clip, bool fadeIn = true)
    {
        if (clip == null || bgmSource == null) return;
        
        if (fadeIn && bgmSource.isPlaying)
        {
            // �t�F�[�h�A�E�g���t�F�[�h�C��
            StartCoroutine(FadeBGM(clip));
        }
        else
        {
            // ���ڍĐ�
            bgmSource.clip = clip;
            bgmSource.Play();
            Debug.Log($"BGM�Đ�: {clip.name}");
        }
    }
    
    public void PlayBGM(BGMType bgmType, bool fadeIn = true)
    {
        string clipName = GetBGMClipName(bgmType);
        PlayBGM(clipName, fadeIn);
    }
    
    public void StopBGM(bool fadeOut = true)
    {
        if (bgmSource == null) return;
        
        if (fadeOut)
        {
            StartCoroutine(FadeOutBGM());
        }
        else
        {
            bgmSource.Stop();
            Debug.Log("BGM��~");
        }
    }
    
    public void PauseBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Pause();
            Debug.Log("BGM�ꎞ��~");
        }
    }
    
    public void ResumeBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.UnPause();
            Debug.Log("BGM�ĊJ");
        }
    }
    
    // ===============================
    // ���ʉ����䃁�\�b�h
    // ===============================
    
    public void PlaySFX(string clipName, float volume = 1.0f, float pitch = 1.0f)
    {
        if (audioClips.ContainsKey(clipName))
        {
            PlaySFX(audioClips[clipName], volume, pitch);
        }
        else
        {
            Debug.LogWarning($"���ʉ� '{clipName}' ��������܂���");
        }
    }
    
    public void PlaySFX(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clip == null || sfxSources == null) return;
        
        // ��������Z���ԂŘA���Đ�����̂�h��
        if (lastPlayTime.ContainsKey(clip.name))
        {
            if (Time.time - lastPlayTime[clip.name] < 0.1f)
            {
                return;
            }
        }
        
        // ���p�\��AudioSource��T��
        AudioSource availableSource = GetAvailableSFXSource();
        if (availableSource != null)
        {
            availableSource.clip = clip;
            availableSource.volume = volume * sfxVolume * masterVolume;
            availableSource.pitch = pitch;
            availableSource.Play();
            
            lastPlayTime[clip.name] = Time.time;
            Debug.Log($"���ʉ��Đ�: {clip.name}");
        }
        else
        {
            Debug.LogWarning("���p�\�Ȍ��ʉ��\�[�X������܂���");
        }
    }
    
    public void PlaySFX(SFXType sfxType, float volume = 1.0f, float pitch = 1.0f)
    {
        string clipName = GetSFXClipName(sfxType);
        PlaySFX(clipName, volume, pitch);
    }
    
    public void PlaySFX3D(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        if (clip == null || !use3DAudio) return;
        
        // 3D�����p��AudioSource���쐬
        GameObject tempAudioGO = new GameObject("TempAudio");
        tempAudioGO.transform.position = position;
        
        AudioSource tempAudioSource = tempAudioGO.AddComponent<AudioSource>();
        tempAudioSource.clip = clip;
        tempAudioSource.volume = volume * sfxVolume * masterVolume;
        tempAudioSource.spatialBlend = 1.0f;  // 3D����
        tempAudioSource.rolloffMode = AudioRolloffMode.Linear;
        tempAudioSource.minDistance = minDistance;
        tempAudioSource.maxDistance = maxDistance;
        tempAudioSource.Play();
        
        // �Đ��I����ɃI�u�W�F�N�g���폜
        Destroy(tempAudioGO, clip.length);
        
        Debug.Log($"3D���ʉ��Đ�: {clip.name} at {position}");
    }
    
    // ===============================
    // �{�C�X���䃁�\�b�h
    // ===============================
    
    public void PlayVoice(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null || voiceSource == null) return;
        
        voiceSource.clip = clip;
        voiceSource.volume = volume * voiceVolume * masterVolume;
        voiceSource.Play();
        
        Debug.Log($"�{�C�X�Đ�: {clip.name}");
    }
    
    public void StopVoice()
    {
        if (voiceSource != null)
        {
            voiceSource.Stop();
            Debug.Log("�{�C�X��~");
        }
    }
    
    // ===============================
    // ���ʐ��䃁�\�b�h
    // ===============================
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        Debug.Log($"�}�X�^�[����: {masterVolume:F2}");
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (bgmSource != null)
        {
            bgmSource.volume = musicVolume * masterVolume;
        }
        Debug.Log($"BGM����: {musicVolume:F2}");
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        for (int i = 0; i < sfxSources.Length; i++)
        {
            if (sfxSources[i] != null)
            {
                sfxSources[i].volume = sfxVolume * masterVolume;
            }
        }
        Debug.Log($"���ʉ�����: {sfxVolume:F2}");
    }
    
    public void SetVoiceVolume(float volume)
    {
        voiceVolume = Mathf.Clamp01(volume);
        if (voiceSource != null)
        {
            voiceSource.volume = voiceVolume * masterVolume;
        }
        Debug.Log($"�{�C�X����: {voiceVolume:F2}");
    }
    
    public void ToggleMute()
    {
        isMuted = !isMuted;
        UpdateAllVolumes();
        Debug.Log($"�~���[�g: {isMuted}");
    }
    
    void UpdateAllVolumes()
    {
        float multiplier = isMuted ? 0f : 1f;
        
        if (bgmSource != null)
        {
            bgmSource.volume = musicVolume * masterVolume * multiplier;
        }
        
        if (voiceSource != null)
        {
            voiceSource.volume = voiceVolume * masterVolume * multiplier;
        }
        
        for (int i = 0; i < sfxSources.Length; i++)
        {
            if (sfxSources[i] != null)
            {
                sfxSources[i].volume = sfxVolume * masterVolume * multiplier;
            }
        }
    }
    
    // ===============================
    // �t�F�[�h����
    // ===============================
    
    IEnumerator FadeBGM(AudioClip newClip)
    {
        if (isFading) yield break;
        
        isFading = true;
        
        // �t�F�[�h�A�E�g
        float startVolume = bgmSource.volume;
        for (float t = 0; t < bgmFadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / bgmFadeTime);
            yield return null;
        }
        
        // �V�����N���b�v��ݒ�
        bgmSource.clip = newClip;
        bgmSource.Play();
        
        // �t�F�[�h�C��
        float targetVolume = musicVolume * masterVolume;
        for (float t = 0; t < bgmFadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, targetVolume, t / bgmFadeTime);
            yield return null;
        }
        
        bgmSource.volume = targetVolume;
        isFading = false;
        
        Debug.Log($"BGM�t�F�[�h����: {newClip.name}");
    }
    
    IEnumerator FadeOutBGM()
    {
        if (isFading) yield break;
        
        isFading = true;
        float startVolume = bgmSource.volume;
        
        for (float t = 0; t < bgmFadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / bgmFadeTime);
            yield return null;
        }
        
        bgmSource.Stop();
        bgmSource.volume = musicVolume * masterVolume;
        isFading = false;
        
        Debug.Log("BGM�t�F�[�h�A�E�g����");
    }
    
    // ===============================
    // ���[�e�B���e�B���\�b�h
    // ===============================
    
    AudioSource GetAvailableSFXSource()
    {
        // �Đ����łȂ��\�[�X��T��
        for (int i = 0; i < sfxSources.Length; i++)
        {
            if (sfxSources[i] != null && !sfxSources[i].isPlaying)
            {
                return sfxSources[i];
            }
        }
        
        // �S�čĐ����̏ꍇ�A�ł��Â����̂��g�p
        currentSFXIndex = (currentSFXIndex + 1) % sfxSources.Length;
        return sfxSources[currentSFXIndex];
    }
    
    string GetSFXClipName(SFXType sfxType)
    {
        return sfxType switch
        {
            SFXType.ButtonClick => "ButtonClick",
            SFXType.ItemCollect => "ItemCollect",
            SFXType.EnemyHit => "EnemyHit",
            SFXType.PlayerHurt => "PlayerHurt",
            SFXType.PowerUp => "PowerUp",
            SFXType.GameOver => "GameOver",
            SFXType.Victory => "Victory",
            SFXType.Jump => "Jump",
            SFXType.Footstep => "Footstep",
            SFXType.Explosion => "Explosion",
            _ => "DefaultSFX"
        };
    }
    
    string GetBGMClipName(BGMType bgmType)
    {
        return bgmType switch
        {
            BGMType.MainMenu => "MainMenuBGM",
            BGMType.GamePlay => "GamePlayBGM",
            BGMType.GameOver => "GameOverBGM",
            BGMType.Victory => "VictoryBGM",
            BGMType.Boss => "BossBGM",
            _ => "DefaultBGM"
        };
    }
    
    void LoadAudioSettings()
    {
        // �ۑ����ꂽ�����ݒ��ǂݍ���
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 1.0f);
        
        UpdateAllVolumes();
    }
    
    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("VoiceVolume", voiceVolume);
        PlayerPrefs.Save();
        
        Debug.Log("�����ݒ�ۑ�����");
    }
    
    void HandleDebugInput()
    {
        // �f�o�b�O�p�L�[����
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayBGM(BGMType.MainMenu);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayBGM(BGMType.GamePlay);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlaySFX(SFXType.ButtonClick);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlaySFX(SFXType.ItemCollect);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StopBGM();
        }
    }
    
    // ===============================
    // �p�u���b�N���\�b�h
    // ===============================
    
    public bool IsBGMPlaying()
    {
        return bgmSource != null && bgmSource.isPlaying;
    }
    
    public float GetBGMPosition()
    {
        return bgmSource != null ? bgmSource.time : 0f;
    }
    
    public void SetBGMPosition(float time)
    {
        if (bgmSource != null)
        {
            bgmSource.time = time;
        }
    }
    
    public AudioClip GetCurrentBGM()
    {
        return bgmSource?.clip;
    }
    
    public void AddAudioClip(string name, AudioClip clip)
    {
        if (clip != null)
        {
            audioClips[name] = clip;
            Debug.Log($"�����N���b�v�ǉ�: {name}");
        }
    }
    
    public void RemoveAudioClip(string name)
    {
        if (audioClips.ContainsKey(name))
        {
            audioClips.Remove(name);
            Debug.Log($"�����N���b�v�폜: {name}");
        }
    }
    
    // ===============================
    // �I������
    // ===============================
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // �A�v���P�[�V��������A�N�e�B�u�ɂȂ������̏���
            PauseBGM();
        }
        else
        {
            // �A�v���P�[�V�������A�N�e�B�u�ɂȂ������̏���
            ResumeBGM();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseBGM();
        }
        else
        {
            ResumeBGM();
        }
    }
    
    void OnDestroy()
    {
        SaveAudioSettings();
    }
}
```

### �X�e�b�v3: �����A�g�X�N���v�g

```csharp
using UnityEngine;

// ���̃X�N���v�g����̉����Ăяo����
public class AudioIntegrationExample : MonoBehaviour
{
    void Start()
    {
        // �Q�[���J�n����BGM�Đ�
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.BGMType.GamePlay);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �A�C�e�����W���̍Đ�
            AudioManager.Instance?.PlaySFX(AudioManager.SFXType.ItemCollect);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 3D�����Ńq�b�g�����Đ�
            AudioManager.Instance?.PlaySFX3D(
                AudioManager.Instance.audioClips["EnemyHit"], 
                transform.position
            );
        }
    }
}
```

### �X�e�b�v4: �ݒ�ƃe�X�g

1. AudioManager �I�u�W�F�N�g���쐬
2. �����t�@�C�����C���|�[�g
3. Inspector �ŉ����N���b�v��ݒ�
4. �e�X�g�X�N���v�g�œ���m�F

## ?? �������Ă݂悤

### ����1: �����̃����_����

```csharp
public AudioClip[] footstepSounds;

public void PlayRandomFootstep()
{
    if (footstepSounds.Length > 0)
    {
        int randomIndex = Random.Range(0, footstepSounds.Length);
        PlaySFX(footstepSounds[randomIndex]);
    }
}
```

### ����2: �����V�[�P���X

```csharp
public class AudioSequence : MonoBehaviour
{
    public AudioClip[] sequenceClips;
    public float[] delays;
    
    public void PlaySequence()
    {
        StartCoroutine(PlaySequenceCoroutine());
    }
    
    IEnumerator PlaySequenceCoroutine()
    {
        for (int i = 0; i < sequenceClips.Length; i++)
        {
            AudioManager.Instance?.PlaySFX(sequenceClips[i]);
            yield return new WaitForSeconds(delays[i]);
        }
    }
}
```

### ����3: ��������

```csharp
public class AudioVisualizer : MonoBehaviour
{
    public RectTransform[] visualBars;
    public AudioSource audioSource;
    
    void Update()
    {
        if (audioSource.isPlaying)
        {
            float[] spectrum = new float[256];
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
            
            for (int i = 0; i < visualBars.Length; i++)
            {
                float height = spectrum[i] * 1000f;
                visualBars[i].sizeDelta = new Vector2(visualBars[i].sizeDelta.x, height);
            }
        }
    }
}
```

## ? �悭����G���[

### �G���[1: �����Đ�����Ȃ�
**����**: AudioListener ���Ȃ��A�܂��͉��ʂ�0
**�������@**: MainCamera ��AudioListener �����邩�m�F

### �G���[2: �����d���Đ������
**����**: ��������Z���Ԃŕ�����Đ�
**�������@**: �Đ��Ԋu�����̎���

### �G���[3: 3D�������@�\���Ȃ�
**����**: spatialBlend�ݒ� �܂��� AudioListener�ʒu
**�������@**: spatialBlend = 1.0f �ɐݒ�

## ?? �w�K�|�C���g

### �d�v�ȊT�O
1. **AudioSource**: �����Đ��R���|�[�l���g
2. **AudioClip**: �����f�[�^
3. **AudioListener**: ������M
4. **Spatial Audio**: 3D����
5. **Audio Mixing**: �����~�L�V���O

### �����݌v�̃|�C���g
- **Performance**: �����Đ����̐���
- **Memory**: �����t�@�C���̍œK��
- **User Experience**: �K�؂ȉ��ʃo�����X
- **Accessibility**: �����ݒ�̒�

## ?? ���̃X�e�b�v

���̉ۑ芮����F
1. **����x�`�F�b�N**: �����V�X�e���̍\������
2. **����**: 3D�����̎���
3. **���̉ۑ�**: �ۑ�07�u�A�j���[�V�����V�X�e���v

## ?? �ǉ��`�������W

### �`�������W1: �I�[�f�B�I�v�[��
```csharp
public class AudioPool : MonoBehaviour
{
    public int poolSize = 20;
    private Queue<AudioSource> audioPool;
    
    void Start()
    {
        audioPool = new Queue<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateAudioSource();
        }
    }
    
    void CreateAudioSource()
    {
        GameObject audioGO = new GameObject("PooledAudio");
        audioGO.transform.SetParent(transform);
        AudioSource audioSource = audioGO.AddComponent<AudioSource>();
        audioPool.Enqueue(audioSource);
    }
    
    public AudioSource GetAudioSource()
    {
        if (audioPool.Count > 0)
        {
            return audioPool.Dequeue();
        }
        else
        {
            CreateAudioSource();
            return audioPool.Dequeue();
        }
    }
}
```

### �`�������W2: ���I��������
```csharp
public class ProceduralAudio : MonoBehaviour
{
    public float frequency = 440f;
    public float amplitude = 0.5f;
    public int sampleRate = 44100;
    
    void Start()
    {
        AudioClip clip = GenerateTone(frequency, 2f);
        AudioManager.Instance?.PlaySFX(clip);
    }
    
    AudioClip GenerateTone(float freq, float duration)
    {
        int samples = (int)(sampleRate * duration);
        AudioClip clip = AudioClip.Create("GeneratedTone", samples, 1, sampleRate, false);
        
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate) * amplitude;
        }
        
        clip.SetData(data, 0);
        return clip;
    }
}
```

### �`�������W3: �����C�x���g�V�X�e��
```csharp
public class AudioEventSystem : MonoBehaviour
{
    public static System.Action<string, Vector3> OnSFXRequested;
    public static System.Action<string> OnBGMRequested;
    
    void Start()
    {
        OnSFXRequested += PlaySFXAtPosition;
        OnBGMRequested += PlayBGMByName;
    }
    
    void PlaySFXAtPosition(string sfxName, Vector3 position)
    {
        // 3D�����ōĐ�
        AudioManager.Instance?.PlaySFX3D(
            AudioManager.Instance.audioClips[sfxName], 
            position
        );
    }
    
    void PlayBGMByName(string bgmName)
    {
        AudioManager.Instance?.PlayBGM(bgmName);
    }
}
```

�����ł��I?? 
��I�ȉ����V�X�e�������������܂����I